using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[System.Serializable]
public class AnimalData
{
    public string typeID;
    public string instanceID;
    public string animalName;
    public int rarity;
    public int level;
    public double happiness;
    public double food;
    public double water;
    public double bond;

    private static double RarityMultiplier(int rarity) => Math.Pow(1.6, rarity - 1);
    private static double NeedsEffect(double food, double water)
    {
        double needs = food * 0.5 + water * 0.5;
        return 0.1 + needs * 0.9;
    }
    public int likes => (int)(1 +
    1 *
    (RarityMultiplier(this.rarity) * 1.50) *
    (1 + Math.Pow(this.level, 0.6)) *
    (1 + Math.Pow(this.happiness, 3)) *
    (1 + Math.Pow(this.bond, 0.4)) *
    Math.Pow(NeedsEffect(this.food, this.water), 3));

    public AnimalData(string typeID, string name, int rarity, int level = 1, double happiness = 1, double food = 1, double water = 1, double bond = 0)
    {
            this.typeID = typeID;
            this.instanceID = Guid.NewGuid().ToString();
            this.animalName = name;
            this.rarity = rarity;
            this.level = level;
            this.happiness = happiness;
            this.food = food;
            this.water = water;
            this.bond = bond;
    }
}

public enum AnimalStateType
{
    Full, Mini, Register
}



