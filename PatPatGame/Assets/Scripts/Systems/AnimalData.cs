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



