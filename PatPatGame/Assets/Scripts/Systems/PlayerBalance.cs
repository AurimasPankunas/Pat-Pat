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
        
        animals = Resources.FindObjectsOfTypeAll<Animal>().ToList();
        foreach(Animal animal in animals)
        {
            animal.Initialize();
        }

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

    public void AddPettingMoney(Animal animal, int gloveRarity)
    {
        money += animal.PettingIncome(gloveRarity);
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
}
