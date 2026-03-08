using NUnit.Framework;
using System;
using UnityEngine.InputSystem;

[System.Serializable]
public class Animal
{
    private const double minimumHappiness = 0.01;

    public int rarity { get; private set; }
    public int level {  get; private set; }
    public double happiness{ get; private set; }
    public double food { get; private set; }
    public double water { get; private set; }
    public double bond { get; private set; }
    public int Likes { get; private set; }

    public Animal(int rarity = 1, double food = 1, double water = 1, double bond = 0, int level = 1, double happiness = 1, int likes = 1)
    {
        this.level = level;
        this.happiness = happiness;
        this.food = food;
        this.water = water;
        this.bond = bond;
        this.Likes = likes;
    }

    private static double rarityMultiplier(int rarity) => Math.Pow(1.3, rarity - 1);
    private static double levelMultiplier(int level) => Math.Pow(level, 1.03);
    private static double needsEffect(double food, double water) => 0.5 + (food * 0.5 + water * 0.5) / 2.0;


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

        double newHappiness = happiness - happinessUpdateMultiplier * dt * Math.Pow(happiness, 1.15);

        if(newHappiness < minimumHappiness)
        {
            this.happiness = minimumHappiness;
        }
        else
        {
            this.happiness = newHappiness;
        }
    }

    public void UpdateBond(double dt = 1)
    {
        double bondIncreaseVariable = 0.00001 * dt * Math.Pow(happiness, 1.5);

        if (bond + bondIncreaseVariable > 4)
        {
            bond = 4;
        }
        else
        {
            bond += bondIncreaseVariable;
        }
    }

    public void UpdateLikes()
    {
        this.Likes = (int)(1 +
            1 *
            (rarityMultiplier(this.rarity) * 1.50) *
            (1 + Math.Pow(this.level, 0.6)) *
            (1 + Math.Pow(this.happiness, 3)) *
            (1 + Math.Pow(this.bond, 0.4)) *
            Math.Pow(needsEffect(this.food, this.water), 3));
    }

    public void PettingHappinessIncrease(int gloveRarity)
    {
        double happinessBonusPercent = 0.20 + 0.10 * (gloveRarity - 1);
        if (happiness + happinessBonusPercent < 1)
        {
            happiness += happinessBonusPercent;
        }
        else
        {
            happiness = 1;
        }
    }

    public double PettingIncome(int gloveRarity)
    {
        PettingHappinessIncrease(gloveRarity);

        double @base = 20;

        double income =
            @base *
            rarityMultiplier(this.rarity) *
            levelMultiplier(this.level) *
            (1 + this.happiness) *
            (1 + this.bond * 0.10) *
            (1 + Math.Pow(gloveRarity, 1.25)) *
            needsEffect(this.food, this.water);

        return Math.Round(income, 2);
    }

    public void LevelUp()
    {
        this.level += 1;
    }


    public void updateFood(double foodAmount)
    {
        if(this.food + foodAmount < 0)
        {
            this.food = 0;
        }
        else
        {
            this.food += foodAmount;
        }
    }

    public void updateWater(double waterAmount)
    {
        if (this.water + waterAmount < 0)
        {
            this.water = 0;
        }
        else
        {
            this.water += waterAmount;
        }
    }

    public double IncomeCalculation(bool isOnline, double @base = 0.1, double afkProgress = 0)
    {
        double rarityVariable = rarityMultiplier(this.rarity);
        double levelVariable = levelMultiplier(this.level);
        double needs = needsEffect(this.food, this.water);

        double onlineMultiplier;
        if (isOnline)
        {
            onlineMultiplier = 1;
        }
        else
        {
            onlineMultiplier = Math.Exp(-3.2 * afkProgress);
        }

        return @base * rarityVariable * levelVariable * this.happiness *
               (1 + Math.Pow(this.bond, 2)) *
               onlineMultiplier * needs;
    }
}
