using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class PlayerBalance : MonoBehaviour
{
    [field: SerializeField] public double money { get; private set; }
    [SerializeField] private float MAX_AFK_HOURS = 15 * 24;
    private List<Animal> animals;
    

    void Awake()
    {
        // For now the animal list is initialized in this class
        // It would be better to have a separate animal manager that keeps a list of all animals

        animals = FindObjectsByType<Animal>(FindObjectsSortMode.None).ToList();


        // Calculate income once every second
        InvokeRepeating(nameof(CalculateOneSecondIncomeForPets), 0, 1);
    }

    void Update()
    {
        // Alternatively, income can be calculated every frame

        // foreach(Animal animal in animals)
            // money += animal.IncomeCalculation(true) * Time.deltaTime;
    }


    public void AddMoney(double amount)
    {
        money += amount;
    }

    public double AddPettingMoney(Animal animal)
    {
        // PlayerBalance should probably have a reference so some sort of player class
        // that contains equiped glove rarity and get the value from there
        // That class doesn't exist yet, so rarity is hardcoded to 1
        int gloveRarity = 1;
        double amount = animal.PettingIncome(gloveRarity);
        money += amount;
        return amount;
    }

    private void CalculateOneSecondIncomeForPets()
    {
        double amount = 0;
        foreach(Animal animal in animals)
        {
            amount += animal.IncomeCalculation(true);
        }
        money += amount;
    }

    // Simulate income and stats change for a SINGLE animal in a given time period (hours)
    private double SimulateIncomeForPet(Animal pet, double hours, bool isOnline, double @base = 0.1)
    {
        if (!isOnline)
            hours = Math.Min(hours, MAX_AFK_HOURS);

        double totalIncome = 0;
        int fullHours = (int)hours;
        double remainingHours = hours - fullHours;
        double dtHour = 3600;

        double lastHourIncome = 0;

        for (int t = 0; t < fullHours; t++)
        {
            double afkProgress = (double)t / MAX_AFK_HOURS;

            pet.UpdateHappiness(isOnline, afkProgress, dtHour);

            pet.UpdateFood(-0.000008 * dtHour);
            pet.UpdateWater(-0.000008 * dtHour);

            pet.UpdateBond(dtHour);

            double inc = pet.IncomeCalculation(isOnline, @base, afkProgress);

            lastHourIncome = inc * dtHour;
            totalIncome += lastHourIncome;
        }

        if (remainingHours > 0 && fullHours > 0)
            totalIncome += lastHourIncome * remainingHours;

        return Math.Round(totalIncome, 2);
    }

    // Simulate income and stats change for ALL animals in a given time period (hours)
    private double SimulateShelter(List<Animal> pets, double hours, bool isOnline, double baseIncome = 0.01)
    {
        // var report = new List<PetReport>();
        double totalIncome = 0;

        foreach (Animal pet in pets)
        {
            double income = SimulateIncomeForPet(pet, hours, isOnline, baseIncome);

            // report.Add(new PetReport
            // {
            //     PetId = i + 1,
            //     Rarity = pet.Rarity,
            //     Level = pet.Level,
            //     Income = income,
            //     Bond = Math.Round(pet.Bond, 4),
            //     Happiness = Math.Round(pet.Happiness, 4),
            //     Food = Math.Round(pet.Food, 4),
            //     Water = Math.Round(pet.Water, 4)
            // });

            totalIncome += income;
        }

        return Math.Round(totalIncome, 2);
    }

    public PlayerBalanceData Save()
    {
        PlayerBalanceData data = new PlayerBalanceData();
        data.money = this.money;
        return data;
    }

    public void Load(PlayerBalanceData data, double hours)
    {
        if (animals == null)
        {
            animals = FindObjectsByType<Animal>(FindObjectsSortMode.None).ToList();
        }
        double savedMoney = data.money;
        double earnedMoney = SimulateShelter(animals, hours, false);
        this.money = savedMoney + earnedMoney;
    }
}

[System.Serializable]
public struct PlayerBalanceData
{
    public double money;
}
