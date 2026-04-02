using System;
using UnityEngine;

[System.Serializable]
public class Animal : MonoBehaviour
{
    private const double MinimumHappiness = 0.01;
    private const double DrainPerSecond   = 1.0 / (72.0 * 3600.0);

    [field: SerializeField] public string AnimalName { get; private set; }
    [field: SerializeField] public int    Rarity     { get; private set; }
    public int    Level     { get; private set; }
    public double Happiness { get; private set; }
    public double Food      { get; private set; }
    public double Water     { get; private set; }
    public double Bond      { get; private set; }
//LIKES INPLEMENTED
    public int Likes => (int)(1 +
        1 *
        (RarityMultiplier(Rarity) * 1.50) *
        (1 + Math.Pow(Level, 0.6)) *
        (1 + Math.Pow(Happiness, 3)) *
        (1 + Math.Pow(Bond, 0.4)) *
        Math.Pow(NeedsEffect(Food, Water), 3));
    private static double RarityMultiplier(int rarity) => Math.Pow(1.6, rarity - 1);
    private static double LevelMultiplier(int level) => Math.Pow(level, 1.2);
    private static double NeedsEffect(double food, double water)
    {
        double needs = food * 0.5 + water * 0.5;
        return 0.1 + needs * 0.9;
    }
    private static double BondEffect(double bond) => 1.0 + bond * 0.5;

    public void Initialize(int rarity = 1, double food = 1, double water = 1,
                           double bond = 0, int level = 1, double happiness = 1)
    {
        if (rarity < 1) throw new ArgumentException("Rarity must be >= 1", nameof(rarity));
        if (level  < 1) throw new ArgumentException("Level must be >= 1",  nameof(level));

        Rarity    = rarity;
        Level     = level;
        Happiness = Math.Clamp(happiness, MinimumHappiness, 1.0);
        Food      = Math.Clamp(food,      0.0, 1.0);
        Water     = Math.Clamp(water,     0.0, 1.0);
        Bond      = Math.Clamp(bond,      0.0, 4.0);
    }
    public void UpdateHappiness(bool isOnline, double afkProgress = 0, double dt = 1)
    {
        double k = isOnline
            ? 0.0000020
            : 0.0000032 + (0.0000035 * afkProgress);

        double newHappiness = Happiness - k * dt * Math.Pow(Happiness, 1.15);
        Happiness = newHappiness < MinimumHappiness ? MinimumHappiness : newHappiness;
    }
    public void UpdateBond(double dt = 1)
    {
        double increase = 0.00001 * dt * Math.Pow(Happiness, 1.5);
        Bond = Math.Min(4.0, Bond + increase);
    }
    public void UpdateNeeds(double dt = 1)
    {
        Food  = Math.Max(0.0, Food  - DrainPerSecond * dt);
        Water = Math.Max(0.0, Water - DrainPerSecond * dt);
    }
    public void UpdateFood(double foodAmount)
    {
        Food = Math.Clamp(Food + foodAmount, 0.0, 1.0);
    }

    public void UpdateWater(double waterAmount)
    {
        Water = Math.Clamp(Water + waterAmount, 0.0, 1.0);
    }

    public double IncomeCalculation(bool isOnline, double @base = 0.01, double afkProgress = 0)
    {
        double r     = RarityMultiplier(Rarity);
        double l     = LevelMultiplier(Level);
        double needs = NeedsEffect(Food, Water);
        double b     = BondEffect(Bond);

        double onlineMultiplier = isOnline
            ? 1.0
            : Math.Exp(-3.2 * afkProgress);

        return @base * r * l * Happiness * b * onlineMultiplier * needs;
    }

    public void PettingHappinessIncrease(int gloveRarity)
    {
        double bonus = 0.20 + 0.10 * (gloveRarity - 1);
        Happiness = Math.Min(Happiness + bonus, 1.0);
    }

    public double PettingIncome(int gloveRarity)
    {
        PettingHappinessIncrease(gloveRarity);

        double income =
            20.0 *
            RarityMultiplier(Rarity) *
            LevelMultiplier(Level) *
            (1 + Happiness) *
            BondEffect(Bond) *
            (1 + Math.Pow(gloveRarity, 1.25)) *
            NeedsEffect(Food, Water);

        return Math.Round(income, 2);
    }

    public void LevelUp()
    {
        Level += 1;
    }

    public void SetAnimalName(string name)
    {
        AnimalName = name;
    }

    public AnimalData Save()
    {
        return new AnimalData
        {
            animalName = AnimalName,
            rarity     = Rarity,
            level      = Level,
            happiness  = Happiness,
            food       = Food,
            water      = Water,
            bond       = Bond
        };
    }

    public void Load(AnimalData data)
    {
        SetAnimalName(data.animalName);
        Initialize(data.rarity, data.food, data.water, data.bond, data.level, data.happiness);
    }
}
