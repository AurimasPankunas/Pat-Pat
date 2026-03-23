using System.Collections.Generic;
using UnityEngine;

// ============================================================
// ShopItemDatabase.cs — Unity ScriptableObject
// Naudojimas: Create → Shop → Item Database
// Kiekvienas item turi gameObjectId kurį prijungi prie
// atitinkamo GameObject/Prefab Unity Inspector lange
// ============================================================

// --- Bazinė item klasė ---
[System.Serializable]
public class ShopItem
{
    public int    gameObjectId;
    public int    price;
}

// --- Pirštinės ---
[System.Serializable]
public class GloveItem : ShopItem
{
    public string type;
    public int    rarity;
    public Texture2D image;
    public Color imageTint;
}

// --- Maistas ---
[System.Serializable]
public class FoodItem : ShopItem
{
    public string name;
    public int    rarity;
    public float  foodAmount;
    public Texture2D image;
}

// --- Vanduo ---
[System.Serializable]
public class WaterItem : ShopItem
{
    public string name;
    public int    rarity;
    public float  waterAmount;
    public Texture2D image;
}

// --- Lygio kėlimas ---
[System.Serializable]
public class LevelUpItem : ShopItem
{
    public int rarity;
    public int level;
}

// --- Gyvūnų vietos ---
[System.Serializable]
public class SpotItem : ShopItem
{
    public int spot;
}

// ============================================================
// Pagrindinis ScriptableObject
// ============================================================
[CreateAssetMenu(fileName = "ShopItemDatabase", menuName = "Shop/Item Database")]
public class ShopItemDatabase : ScriptableObject
{
    [Header("Pirštinės")]
    public List<GloveItem> glovePrices = new List<GloveItem>
    {
        new GloveItem { gameObjectId = 1001, type = "Common",    rarity = 1, price =       1000 },
        new GloveItem { gameObjectId = 1002, type = "Uncommon",  rarity = 2, price =      10000 },
        new GloveItem { gameObjectId = 1003, type = "Rare",      rarity = 3, price =      50000 },
        new GloveItem { gameObjectId = 1004, type = "Epic",      rarity = 4, price =     250000 },
        new GloveItem { gameObjectId = 1005, type = "Legendary", rarity = 5, price =    1250000 },
    };

    [Header("Maistas")]
    public List<FoodItem> foodPrices = new List<FoodItem>
    {
        new FoodItem { gameObjectId = 2001, name = "Kibble",   rarity = 1, foodAmount = 0.10f, price =  25 },
        new FoodItem { gameObjectId = 2002, name = "Snack",    rarity = 2, foodAmount = 0.10f, price =  36 },
        new FoodItem { gameObjectId = 2003, name = "Meal",     rarity = 3, foodAmount = 0.10f, price =  58 },
        new FoodItem { gameObjectId = 2004, name = "Feast",    rarity = 4, foodAmount = 0.10f, price =  92 },
        new FoodItem { gameObjectId = 2005, name = "Delicacy", rarity = 5, foodAmount = 0.10f, price = 150 },
    };

    [Header("Vanduo")]
    public List<WaterItem> waterPrices = new List<WaterItem>
    {
        new WaterItem { gameObjectId = 3001, name = "Puddle", rarity = 1, waterAmount = 0.10f, price =  25 },
        new WaterItem { gameObjectId = 3002, name = "Cup",    rarity = 2, waterAmount = 0.10f, price =  36 },
        new WaterItem { gameObjectId = 3003, name = "Bottle", rarity = 3, waterAmount = 0.10f, price =  58 },
        new WaterItem { gameObjectId = 3004, name = "Jug",    rarity = 4, waterAmount = 0.10f, price =  92 },
        new WaterItem { gameObjectId = 3005, name = "Spring", rarity = 5, waterAmount = 0.10f, price = 150 },
    };

        [Header("Lygio kėlimas")]
    public List<LevelUpItem> levelUpPrices = new List<LevelUpItem>
    {
        new LevelUpItem { gameObjectId = 4201, rarity = 1, level =  1, price =     680 },
        new LevelUpItem { gameObjectId = 4202, rarity = 1, level =  2, price =    1570 },
        new LevelUpItem { gameObjectId = 4203, rarity = 1, level =  3, price =    2550 },
        new LevelUpItem { gameObjectId = 4204, rarity = 1, level =  4, price =    3600 },
        new LevelUpItem { gameObjectId = 4205, rarity = 1, level =  5, price =    4700 },
        new LevelUpItem { gameObjectId = 4206, rarity = 1, level =  6, price =    5860 },
        new LevelUpItem { gameObjectId = 4207, rarity = 1, level =  7, price =    7040 },
        new LevelUpItem { gameObjectId = 4208, rarity = 1, level =  8, price =    8270 },
        new LevelUpItem { gameObjectId = 4209, rarity = 1, level =  9, price =    9520 },
        new LevelUpItem { gameObjectId = 4210, rarity = 1, level = 10, price =   10810 },
        new LevelUpItem { gameObjectId = 4211, rarity = 1, level = 11, price =   12120 },
        new LevelUpItem { gameObjectId = 4212, rarity = 1, level = 12, price =   13450 },
        new LevelUpItem { gameObjectId = 4213, rarity = 1, level = 13, price =   14810 },
        new LevelUpItem { gameObjectId = 4214, rarity = 1, level = 14, price =   16180 },
        new LevelUpItem { gameObjectId = 4215, rarity = 1, level = 15, price =   17580 },
        new LevelUpItem { gameObjectId = 4216, rarity = 1, level = 16, price =   19000 },
        new LevelUpItem { gameObjectId = 4217, rarity = 1, level = 17, price =   20430 },
        new LevelUpItem { gameObjectId = 4218, rarity = 1, level = 18, price =   21880 },
        new LevelUpItem { gameObjectId = 4219, rarity = 1, level = 19, price =   23350 },
        new LevelUpItem { gameObjectId = 4220, rarity = 1, level = 20, price =   24830 },
        new LevelUpItem { gameObjectId = 4221, rarity = 1, level = 21, price =   26330 },
        new LevelUpItem { gameObjectId = 4222, rarity = 1, level = 22, price =   27840 },
        new LevelUpItem { gameObjectId = 4223, rarity = 1, level = 23, price =   29360 },
        new LevelUpItem { gameObjectId = 4224, rarity = 1, level = 24, price =   30900 },
        new LevelUpItem { gameObjectId = 4225, rarity = 1, level = 25, price =   32460 },
        new LevelUpItem { gameObjectId = 4226, rarity = 1, level = 26, price =   34020 },
        new LevelUpItem { gameObjectId = 4227, rarity = 1, level = 27, price =   35600 },
        new LevelUpItem { gameObjectId = 4228, rarity = 1, level = 28, price =   37180 },
        new LevelUpItem { gameObjectId = 4229, rarity = 1, level = 29, price =   38780 },
        new LevelUpItem { gameObjectId = 4301, rarity = 2, level =  1, price =    1090 },
        new LevelUpItem { gameObjectId = 4302, rarity = 2, level =  2, price =    2510 },
        new LevelUpItem { gameObjectId = 4303, rarity = 2, level =  3, price =    4080 },
        new LevelUpItem { gameObjectId = 4304, rarity = 2, level =  4, price =    5760 },
        new LevelUpItem { gameObjectId = 4305, rarity = 2, level =  5, price =    7530 },
        new LevelUpItem { gameObjectId = 4306, rarity = 2, level =  6, price =    9370 },
        new LevelUpItem { gameObjectId = 4307, rarity = 2, level =  7, price =   11270 },
        new LevelUpItem { gameObjectId = 4308, rarity = 2, level =  8, price =   13230 },
        new LevelUpItem { gameObjectId = 4309, rarity = 2, level =  9, price =   15240 },
        new LevelUpItem { gameObjectId = 4310, rarity = 2, level = 10, price =   17290 },
        new LevelUpItem { gameObjectId = 4311, rarity = 2, level = 11, price =   19390 },
        new LevelUpItem { gameObjectId = 4312, rarity = 2, level = 12, price =   21520 },
        new LevelUpItem { gameObjectId = 4313, rarity = 2, level = 13, price =   23690 },
        new LevelUpItem { gameObjectId = 4314, rarity = 2, level = 14, price =   25900 },
        new LevelUpItem { gameObjectId = 4315, rarity = 2, level = 15, price =   28130 },
        new LevelUpItem { gameObjectId = 4316, rarity = 2, level = 16, price =   30400 },
        new LevelUpItem { gameObjectId = 4317, rarity = 2, level = 17, price =   32690 },
        new LevelUpItem { gameObjectId = 4318, rarity = 2, level = 18, price =   35010 },
        new LevelUpItem { gameObjectId = 4319, rarity = 2, level = 19, price =   37360 },
        new LevelUpItem { gameObjectId = 4320, rarity = 2, level = 20, price =   39730 },
        new LevelUpItem { gameObjectId = 4321, rarity = 2, level = 21, price =   42120 },
        new LevelUpItem { gameObjectId = 4322, rarity = 2, level = 22, price =   44540 },
        new LevelUpItem { gameObjectId = 4323, rarity = 2, level = 23, price =   46980 },
        new LevelUpItem { gameObjectId = 4324, rarity = 2, level = 24, price =   49450 },
        new LevelUpItem { gameObjectId = 4325, rarity = 2, level = 25, price =   51930 },
        new LevelUpItem { gameObjectId = 4326, rarity = 2, level = 26, price =   54430 },
        new LevelUpItem { gameObjectId = 4327, rarity = 2, level = 27, price =   56950 },
        new LevelUpItem { gameObjectId = 4328, rarity = 2, level = 28, price =   59490 },
        new LevelUpItem { gameObjectId = 4329, rarity = 2, level = 29, price =   62050 },
        new LevelUpItem { gameObjectId = 4401, rarity = 3, level =  1, price =    1750 },
        new LevelUpItem { gameObjectId = 4402, rarity = 3, level =  2, price =    4010 },
        new LevelUpItem { gameObjectId = 4403, rarity = 3, level =  3, price =    6520 },
        new LevelUpItem { gameObjectId = 4404, rarity = 3, level =  4, price =    9210 },
        new LevelUpItem { gameObjectId = 4405, rarity = 3, level =  5, price =   12040 },
        new LevelUpItem { gameObjectId = 4406, rarity = 3, level =  6, price =   14990 },
        new LevelUpItem { gameObjectId = 4407, rarity = 3, level =  7, price =   18030 },
        new LevelUpItem { gameObjectId = 4408, rarity = 3, level =  8, price =   21170 },
        new LevelUpItem { gameObjectId = 4409, rarity = 3, level =  9, price =   24380 },
        new LevelUpItem { gameObjectId = 4410, rarity = 3, level = 10, price =   27670 },
        new LevelUpItem { gameObjectId = 4411, rarity = 3, level = 11, price =   31020 },
        new LevelUpItem { gameObjectId = 4412, rarity = 3, level = 12, price =   34440 },
        new LevelUpItem { gameObjectId = 4413, rarity = 3, level = 13, price =   37910 },
        new LevelUpItem { gameObjectId = 4414, rarity = 3, level = 14, price =   41430 },
        new LevelUpItem { gameObjectId = 4415, rarity = 3, level = 15, price =   45010 },
        new LevelUpItem { gameObjectId = 4416, rarity = 3, level = 16, price =   48630 },
        new LevelUpItem { gameObjectId = 4417, rarity = 3, level = 17, price =   52300 },
        new LevelUpItem { gameObjectId = 4418, rarity = 3, level = 18, price =   56020 },
        new LevelUpItem { gameObjectId = 4419, rarity = 3, level = 19, price =   59770 },
        new LevelUpItem { gameObjectId = 4420, rarity = 3, level = 20, price =   63570 },
        new LevelUpItem { gameObjectId = 4421, rarity = 3, level = 21, price =   67400 },
        new LevelUpItem { gameObjectId = 4422, rarity = 3, level = 22, price =   71270 },
        new LevelUpItem { gameObjectId = 4423, rarity = 3, level = 23, price =   75170 },
        new LevelUpItem { gameObjectId = 4424, rarity = 3, level = 24, price =   79110 },
        new LevelUpItem { gameObjectId = 4425, rarity = 3, level = 25, price =   83080 },
        new LevelUpItem { gameObjectId = 4426, rarity = 3, level = 26, price =   87090 },
        new LevelUpItem { gameObjectId = 4427, rarity = 3, level = 27, price =   91120 },
        new LevelUpItem { gameObjectId = 4428, rarity = 3, level = 28, price =   95190 },
        new LevelUpItem { gameObjectId = 4429, rarity = 3, level = 29, price =   99280 },
        new LevelUpItem { gameObjectId = 4501, rarity = 4, level =  1, price =    2790 },
        new LevelUpItem { gameObjectId = 4502, rarity = 4, level =  2, price =    6420 },
        new LevelUpItem { gameObjectId = 4503, rarity = 4, level =  3, price =   10440 },
        new LevelUpItem { gameObjectId = 4504, rarity = 4, level =  4, price =   14740 },
        new LevelUpItem { gameObjectId = 4505, rarity = 4, level =  5, price =   19270 },
        new LevelUpItem { gameObjectId = 4506, rarity = 4, level =  6, price =   23980 },
        new LevelUpItem { gameObjectId = 4507, rarity = 4, level =  7, price =   28860 },
        new LevelUpItem { gameObjectId = 4508, rarity = 4, level =  8, price =   33870 },
        new LevelUpItem { gameObjectId = 4509, rarity = 4, level =  9, price =   39010 },
        new LevelUpItem { gameObjectId = 4510, rarity = 4, level = 10, price =   44270 },
        new LevelUpItem { gameObjectId = 4511, rarity = 4, level = 11, price =   49630 },
        new LevelUpItem { gameObjectId = 4512, rarity = 4, level = 12, price =   55100 },
        new LevelUpItem { gameObjectId = 4513, rarity = 4, level = 13, price =   60650 },
        new LevelUpItem { gameObjectId = 4514, rarity = 4, level = 14, price =   66290 },
        new LevelUpItem { gameObjectId = 4515, rarity = 4, level = 15, price =   72020 },
        new LevelUpItem { gameObjectId = 4516, rarity = 4, level = 16, price =   77810 },
        new LevelUpItem { gameObjectId = 4517, rarity = 4, level = 17, price =   83690 },
        new LevelUpItem { gameObjectId = 4518, rarity = 4, level = 18, price =   89630 },
        new LevelUpItem { gameObjectId = 4519, rarity = 4, level = 19, price =   95640 },
        new LevelUpItem { gameObjectId = 4520, rarity = 4, level = 20, price =  101710 },
        new LevelUpItem { gameObjectId = 4521, rarity = 4, level = 21, price =  107840 },
        new LevelUpItem { gameObjectId = 4522, rarity = 4, level = 22, price =  114030 },
        new LevelUpItem { gameObjectId = 4523, rarity = 4, level = 23, price =  120280 },
        new LevelUpItem { gameObjectId = 4524, rarity = 4, level = 24, price =  126580 },
        new LevelUpItem { gameObjectId = 4525, rarity = 4, level = 25, price =  132940 },
        new LevelUpItem { gameObjectId = 4526, rarity = 4, level = 26, price =  139340 },
        new LevelUpItem { gameObjectId = 4527, rarity = 4, level = 27, price =  145800 },
        new LevelUpItem { gameObjectId = 4528, rarity = 4, level = 28, price =  152300 },
        new LevelUpItem { gameObjectId = 4529, rarity = 4, level = 29, price =  158850 },
        new LevelUpItem { gameObjectId = 4601, rarity = 5, level =  1, price =    4470 },
        new LevelUpItem { gameObjectId = 4602, rarity = 5, level =  2, price =   10270 },
        new LevelUpItem { gameObjectId = 4603, rarity = 5, level =  3, price =   16700 },
        new LevelUpItem { gameObjectId = 4604, rarity = 5, level =  4, price =   23590 },
        new LevelUpItem { gameObjectId = 4605, rarity = 5, level =  5, price =   30830 },
        new LevelUpItem { gameObjectId = 4606, rarity = 5, level =  6, price =   38370 },
        new LevelUpItem { gameObjectId = 4607, rarity = 5, level =  7, price =   46170 },
        new LevelUpItem { gameObjectId = 4608, rarity = 5, level =  8, price =   54190 },
        new LevelUpItem { gameObjectId = 4609, rarity = 5, level =  9, price =   62420 },
        new LevelUpItem { gameObjectId = 4610, rarity = 5, level = 10, price =   70830 },
        new LevelUpItem { gameObjectId = 4611, rarity = 5, level = 11, price =   79420 },
        new LevelUpItem { gameObjectId = 4612, rarity = 5, level = 12, price =   88160 },
        new LevelUpItem { gameObjectId = 4613, rarity = 5, level = 13, price =   97040 },
        new LevelUpItem { gameObjectId = 4614, rarity = 5, level = 14, price =  106070 },
        new LevelUpItem { gameObjectId = 4615, rarity = 5, level = 15, price =  115220 },
        new LevelUpItem { gameObjectId = 4616, rarity = 5, level = 16, price =  124500 },
        new LevelUpItem { gameObjectId = 4617, rarity = 5, level = 17, price =  133900 },
        new LevelUpItem { gameObjectId = 4618, rarity = 5, level = 18, price =  143400 },
        new LevelUpItem { gameObjectId = 4619, rarity = 5, level = 19, price =  153020 },
        new LevelUpItem { gameObjectId = 4620, rarity = 5, level = 20, price =  162730 },
        new LevelUpItem { gameObjectId = 4621, rarity = 5, level = 21, price =  172540 },
        new LevelUpItem { gameObjectId = 4622, rarity = 5, level = 22, price =  182450 },
        new LevelUpItem { gameObjectId = 4623, rarity = 5, level = 23, price =  192450 },
        new LevelUpItem { gameObjectId = 4624, rarity = 5, level = 24, price =  202530 },
        new LevelUpItem { gameObjectId = 4625, rarity = 5, level = 25, price =  212700 },
        new LevelUpItem { gameObjectId = 4626, rarity = 5, level = 26, price =  222950 },
        new LevelUpItem { gameObjectId = 4627, rarity = 5, level = 27, price =  233280 },
        new LevelUpItem { gameObjectId = 4628, rarity = 5, level = 28, price =  243680 },
        new LevelUpItem { gameObjectId = 4629, rarity = 5, level = 29, price =  254160 },
    };

                [Header("Gyvūnų vietos")]
    public List<SpotItem> spotPrices = new List<SpotItem>
    {
        new SpotItem { gameObjectId = 5001, spot =  1, price =      0 },
        new SpotItem { gameObjectId = 5002, spot =  2, price =   5000 },
        new SpotItem { gameObjectId = 5003, spot =  3, price =   5000 },
        new SpotItem { gameObjectId = 5004, spot =  4, price =   5000 },
        new SpotItem { gameObjectId = 5005, spot =  5, price =  28000 },
        new SpotItem { gameObjectId = 5006, spot =  6, price =  28000 },
        new SpotItem { gameObjectId = 5007, spot =  7, price =  28000 },
        new SpotItem { gameObjectId = 5008, spot =  8, price =  28000 },
        new SpotItem { gameObjectId = 5009, spot =  9, price = 160000 },
        new SpotItem { gameObjectId = 5010, spot = 10, price = 160000 },
    };

    // Pagalbiniai metodai
    public GloveItem  GetGlove(int rarity)   => glovePrices.Find(x => x.rarity == rarity);
    public FoodItem   GetFood(int rarity)    => foodPrices.Find(x => x.rarity == rarity);
    public WaterItem  GetWater(int rarity)   => waterPrices.Find(x => x.rarity == rarity);
    public SpotItem   GetSpot(int spot)      => spotPrices.Find(x => x.spot == spot);
    public LevelUpItem GetLevelUp(int rarity, int level) =>
        levelUpPrices.Find(x => x.rarity == rarity && x.level == level);
}
