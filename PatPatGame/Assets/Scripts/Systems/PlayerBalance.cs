using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerBalance : MonoBehaviour
{
    [field: SerializeField] public double money { get; private set; }
    public event Action<double> OnMoneyChanged;
    [field: SerializeField] public int likes { get; private set; }
    public event Action<int> OnLikesChanged;
    [field: SerializeField] public int chestLevel { get; private set; }
    [field: SerializeField] public int levelChestsOpened { get; private set; }
    [SerializeField] private float MAX_AFK_HOURS = 15 * 24;
    public int gloveRarity { get; private set; } = 0;
    public event Action<int> OnGloveRarityChanged;
    [SerializeField] private AnimalManager animalManager;

    public void Initialize()
    {
        // Calculate shelter income once every seconds while playing
        InvokeRepeating(nameof(CalculateOneSecondIncomeForPets), 0, 1);
    }

    // =============================
    // MONEY
    // =============================

    public void AddMoney(double amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }
    public void SubtractMoney(double amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke(money);
    }

    public double AddPettingMoney(Animal animal)
    {
        double amount = animal.PettingIncome(gloveRarity);
        money += amount;
        OnMoneyChanged?.Invoke(money);
        return amount;
    }

    // =============================    
    // LIKES
    // =============================

    public void AddLikes(int amount)
    {
        likes += amount;
        OnLikesChanged?.Invoke(likes);
    }
    public void SubtractLikes(int amount)
    {
        likes -= amount;
        OnLikesChanged?.Invoke(likes);
    }

    // =============================    
    // CHESTS
    // =============================
    public void AddChestsOpened(int amount)
    {
        levelChestsOpened += amount;
    }

    public void SubtractChestsOpened(int amount)
    {
        levelChestsOpened -= amount;
    }

    public void IncrementChestLevel()
    {
        if(chestLevel < 5)
            chestLevel++;
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
        List<Animal> animals = animalManager.spotAnimals;
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
        OnMoneyChanged?.Invoke(money);
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
        if (chestLevel <= 0) chestLevel = 1;
        return new PlayerBalanceData { money = this.money, likes = this.likes, chestLevel = this.chestLevel,
            levelChestsOpened = this.levelChestsOpened, gloveRarity = this.gloveRarity };
    }

    public void Load(PlayerBalanceData data, double hours)
    {
        List<Animal> animals = animalManager.spotAnimals;

        double earnedMoney = SimulateShelter(animals, hours, false);
        this.money = data.money + earnedMoney;
        this.likes = data.likes;
        this.chestLevel = data.chestLevel;
        this.levelChestsOpened = data.levelChestsOpened;
        SetGloveRarity(data.gloveRarity);
    }
}

[System.Serializable]
public struct PlayerBalanceData
{
    public double money;
    public int likes;
    public int chestLevel;
    public int levelChestsOpened;
    public int gloveRarity;
}
