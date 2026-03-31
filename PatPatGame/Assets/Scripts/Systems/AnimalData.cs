using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct AnimalData
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

    public static AnimalData Create(string typeID, string name, int rarity, int level = 1, double happiness = 1, double food = 1, double water = 1, double bond = 0)
    {
        return new AnimalData
        {
            typeID = typeID,
            instanceID = Guid.NewGuid().ToString(),
            animalName = name,
            rarity = rarity,
            level = level,
            happiness = happiness,
            food = food,
            water = water,
            bond = bond
        };
    }
}



