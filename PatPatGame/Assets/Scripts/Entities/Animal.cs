using System;
using UnityEngine;

[System.Serializable]
public class Animal: MonoBehaviour
{
    private const double MinimumHappiness = 0.01;

    [field: SerializeField] public int Rarity { get; private set; }
    public int Level { get; private set; }
    public double Happiness { get; private set; }
    public double Food { get; private set; }
    public double Water { get; private set; }
    public double Bond { get; private set; }
    public int Likes => (int)(1 +
        1 *
        (RarityMultiplier(Rarity) * 1.50) *
        (1 + Math.Pow(Level, 0.6)) *
        (1 + Math.Pow(Happiness, 3)) *
        (1 + Math.Pow(Bond, 0.4)) *
        Math.Pow(NeedsEffect(Food, Water), 3));

    public void Initialize(int rarity = 1, double food = 1, double water = 1, double bond = 0, int level = 1, double happiness = 1)
    {
        if (rarity < 1) throw new ArgumentException("Rarity must be >= 1", nameof(rarity));
        if (level < 1) throw new ArgumentException("Level must be >= 1", nameof(level));

        Level = level;
        Happiness = Math.Clamp(happiness, MinimumHappiness, 1.0);
        Food = Math.Clamp(food, 0.0, 1.0);
        Water = Math.Clamp(water, 0.0, 1.0);
        Bond = Math.Clamp(bond, 0.0, 4.0);
        Rarity = rarity;
    }

    private static double RarityMultiplier(int rarity) => Math.Pow(1.3, rarity - 1);
    private static double LevelMultiplier(int level) => Math.Pow(level, 1.03);
    private static double NeedsEffect(double food, double water) => 0.5 + (food * 0.5 + water * 0.5) / 2.0;


    public void UpdateHappiness(bool isOnline, double afkProgeress = 0, double dt = 1)
    {
        double happinessUpdateMultiplier;

        if (isOnline)
        {
            happinessUpdateMultiplier = 0.0000020;
        }
        else
        {
            happinessUpdateMultiplier = 0.0000045 + (0.0000035 * afkProgeress);
        }

        double newHappiness = Happiness - happinessUpdateMultiplier * dt * Math.Pow(Happiness, 1.15);

        if(newHappiness < MinimumHappiness)
        {
            this.Happiness = MinimumHappiness;
        }
        else
        {
            this.Happiness = newHappiness;
        }
    }

    public void UpdateBond(double dt = 1)
    {
        double bondIncreaseVariable = 0.00001 * dt * Math.Pow(Happiness, 1.5);

        if (Bond + bondIncreaseVariable > 4)
        {
            Bond = 4;
        }
        else
        {
            Bond += bondIncreaseVariable;
        }
    }

    public double PettingIncome(int gloveRarity)
    {
        PettingHappinessIncrease(gloveRarity);

        double @base = 20;

        double income =
            @base *
            RarityMultiplier(this.Rarity) *
            LevelMultiplier(this.Level) *
            (1 + this.Happiness) *
            (1 + this.Bond * 0.10) *
            (1 + Math.Pow(gloveRarity, 1.25)) *
            NeedsEffect(this.Food, this.Water);

        return Math.Round(income, 2);
    }

    public void LevelUp()
    {
        this.Level += 1;
    }


    public void UpdateFood(double foodAmount)
    {
        Food = Math.Clamp(Food + foodAmount, 0.0, 1.0);
    }

    public void UpdateWater(double waterAmount)
    {
        Water = Math.Clamp(Water + waterAmount, 0.0, 1.0);
    }

    public double IncomeCalculation(bool isOnline, double @base = 0.1, double afkProgress = 0)
    {
        double rarityVariable = RarityMultiplier(this.Rarity);
        double levelVariable = LevelMultiplier(this.Level);
        double needs = NeedsEffect(this.Food, this.Water);

        double onlineMultiplier;
        if (isOnline)
        {
            onlineMultiplier = 1;
        }
        else
        {
            onlineMultiplier = Math.Exp(-3.2 * afkProgress);
        }

        return @base * rarityVariable * levelVariable * this.Happiness *
               (1 + Math.Pow(this.Bond, 2)) *
               onlineMultiplier * needs;
    }

    public void PettingHappinessIncrease(int gloveRarity)
    {
        double happinessBonusPercent = 0.20 + 0.10 * (gloveRarity - 1);
        if (Happiness + happinessBonusPercent < 1)
        {
            Happiness += happinessBonusPercent;
        }
        else
        {
            Happiness = 1;
        }
    }
}
