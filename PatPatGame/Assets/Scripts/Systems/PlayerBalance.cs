using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class PlayerBalance : MonoBehaviour
{
    [field: SerializeField] public double money { get; private set; }
    [SerializeField] private float MAX_AFK_HOURS = 15 * 24;
    public int gloveRarity { get; private set; } = 0;
    public event Action<int> OnGloveRarityChanged;
    private List<Animal> animals;

    void Awake()
    {
        animals = FindObjectsByType<Animal>(FindObjectsSortMode.None).ToList();

        // Online pajamų skaičiavimas: kas sekundę
        InvokeRepeating(nameof(CalculateOneSecondIncomeForPets), 0, 1);
    }

    // =============================
    // MONEY
    // =============================

    public void AddMoney(double amount)
    {
        money += amount;
    }

    public double AddPettingMoney(Animal animal)
    {
        double amount = animal.PettingIncome(gloveRarity);
        money += amount;
        return amount;
    }

    // =============================    
    // GLOVES
    // =============================

    // Set the rarity for gloves directly
    public void SetGloveRarity(int rarity)
    {
        rarity = Math.Clamp(rarity, 0, 5);
        this.gloveRarity = rarity;
        OnGloveRarityChanged?.Invoke(rarity);
    }

    // Increase glove rarity by one
    public void IncrementGloveRarity()
    {
        int rarity = Math.Clamp(gloveRarity + 1, 1, 5);
        if (rarity != this.gloveRarity)
        {
            this.gloveRarity = rarity;
            OnGloveRarityChanged?.Invoke(rarity);
        }
    }

    // =============================
    // ONLINE — pajamos per sekundę (visi gyvūnai)
    // =============================

    private void CalculateOneSecondIncomeForPets()
    {
        double total = 0;
        foreach (Animal animal in animals)
        {
            // Atnaujiname kintamuosius
            animal.UpdateHappiness(isOnline: true, dt: 1);
            animal.UpdateNeeds(dt: 1);
            animal.UpdateBond(dt: 1);

            total += animal.IncomeCalculation(isOnline: true);
        }
        money += total;
    }

    // =============================
    // OFFLINE — simuliacija
    // =============================

    private double SimulateIncomeForPet(Animal pet, double hours, bool isOnline, double @base = 0.01)
    {
        if (!isOnline)
            hours = Math.Min(hours, MAX_AFK_HOURS);

        double totalIncome    = 0;
        double dtHour         = 3600;
        int    fullHours      = (int)hours;
        double remainingHours = hours - fullHours;

        for (int t = 0; t < fullHours; t++)
        {
            double afkProgress = isOnline ? 0 : (double)t / MAX_AFK_HOURS;

            pet.UpdateHappiness(isOnline, afkProgress, dtHour);
            pet.UpdateNeeds(dtHour);
            pet.UpdateBond(dtHour);

            double inc = pet.IncomeCalculation(isOnline, @base, afkProgress);
            totalIncome += inc * dtHour;
        }

        // Pataisyta: veikia ir kai offline < 1 valanda
        if (remainingHours > 0)
        {
            double afkProgress = isOnline ? 0 : (double)fullHours / MAX_AFK_HOURS;
            double dtRemaining = remainingHours * 3600;

            pet.UpdateHappiness(isOnline, afkProgress, dtRemaining);
            pet.UpdateNeeds(dtRemaining);
            pet.UpdateBond(dtRemaining);

            double inc = pet.IncomeCalculation(isOnline, @base, afkProgress);
            totalIncome += inc * dtRemaining;
        }

        return Math.Round(totalIncome, 2);
    }

    private double SimulateShelter(List<Animal> pets, double hours, bool isOnline, double baseIncome = 0.01)
    {
        double totalIncome = 0;
        foreach (Animal pet in pets)
            totalIncome += SimulateIncomeForPet(pet, hours, isOnline, baseIncome);
        return Math.Round(totalIncome, 2);
    }

    // =============================
    // SAVE / LOAD
    // =============================

    public PlayerBalanceData Save()
    {
        return new PlayerBalanceData { money = this.money, gloveRarity = this.gloveRarity };
    }

    public void Load(PlayerBalanceData data, double hours)
    {
        if (animals == null)
            animals = FindObjectsByType<Animal>(FindObjectsSortMode.None).ToList();

        double earnedMoney = SimulateShelter(animals, hours, false);
        this.money = data.money + earnedMoney;
        SetGloveRarity(data.gloveRarity);
    }
}

[System.Serializable]
public struct PlayerBalanceData
{
    public double money;
    public int gloveRarity;
}
