using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public int gameObjectId;
    public int price;
}

[System.Serializable]
public class GloveItem : ShopItem
{
    public string type;
    public int    rarity;
    public Texture2D image;
    public Color imageTint;
}

[System.Serializable]
public class FoodItem : ShopItem
{
    public string name;
    public int    rarity;
    public float  foodAmount;
    public Texture2D image;
    public GameObject obj;
}

[System.Serializable]
public class WaterItem : ShopItem
{
    public string name;
    public int    rarity;
    public float  waterAmount;
    public Texture2D image;
    public GameObject obj;
}

[System.Serializable]
public class LevelUpItem : ShopItem
{
    public int rarity;
    public int level;
}

[System.Serializable]
public class SpotItem : ShopItem
{
    public int spot;
}

[System.Serializable]
public class Mission
{
    public string id;
    public string missionName;
    public MissionType type;
    public int rewardMoney;
    public int rewardLikes;
    //[HideInInspector] public int currentProgress; 
    public int targetGoal; 
    public int progress = 0;
    public bool isCompleted = false;
}

public enum MissionType
{
    LogIn,
    BuyFoodWater,
    PatAnimal,
    FeedAnimal,
    LevelAnimal,
    OpenChest,
    SellAnimal,
    CompleteDaily
}

[CreateAssetMenu(fileName = "ShopItemDatabase", menuName = "Shop/Item Database")]
public class ShopItemDatabase : ScriptableObject
{
    [Header("Daily Missions")]
    public List<Mission> dailyMissions = new List<Mission>()
    {
        new Mission { id = "d_login", missionName = "Log in", rewardMoney = 1000, rewardLikes = 0, targetGoal = 1, type = MissionType.LogIn },
        new Mission { id = "d_buy_food", missionName = "Buy food or water", rewardMoney = 1000, rewardLikes = 0, targetGoal = 1, type = MissionType.BuyFoodWater },
        new Mission { id = "d_pat", missionName = "Pet the animals 10 times", rewardMoney = 0, rewardLikes = 1, targetGoal = 10, type = MissionType.PatAnimal },
        new Mission { id = "d_give_food", missionName = "Give an animal food or water", rewardMoney = 1000, rewardLikes = 0, targetGoal = 1, type = MissionType.FeedAnimal },
        new Mission { id = "d_level", missionName = "Level up an animal", rewardMoney = 1000, rewardLikes = 0, targetGoal = 1, type = MissionType.LevelAnimal },
        new Mission { id = "d_complete", missionName = "Complete all daily quests", rewardMoney = 0, rewardLikes = 1, targetGoal = 5, type = MissionType.CompleteDaily }
    };

    [Header("Weekly Missions")]
    public List<Mission> weeklyMissions = new List<Mission>()
    {
        new Mission { id = "w_login", missionName = "Log in daily 5 times", rewardMoney = 3000, rewardLikes = 2, targetGoal = 5, type = MissionType.LogIn },
        new Mission { id = "w_chest", missionName = "Open the chest one time", rewardMoney = 3000, rewardLikes = 1, targetGoal = 1, type = MissionType.OpenChest },
        new Mission { id = "w_sell_animal", missionName = "Give away an animal", rewardMoney = 3000, rewardLikes = 2, targetGoal = 1, type = MissionType.SellAnimal },
        new Mission { id = "w_pat", missionName = "Pet the animals 100 times total", rewardMoney = 3000, rewardLikes = 1, targetGoal = 100, type = MissionType.PatAnimal }
    };


    // Kaina atidaryti chesta Likes valiuta
    public int chestOpenCost = 10;

    // Kiek kartu atidaryti kad chestas uzsilveltina i kita lygi. Indeksas = chestLevel-1. -1 = max lygis.
    public int[] chestOpensToLevelUp = new int[]
    {
          10,  // Level 1 -> Level 2
          20,  // Level 2 -> Level 3
          30,  // Level 3 -> Level 4
          40,  // Level 4 -> Level 5
          -1,  // Level 5: maksimalus lygis
    };

    // Tikimybes gauti kiekviena rarity pagal chesto lygi.
    // Eilute = chestLevel-1, stulpelis = rarity-1: [0]Common [1]Uncommon [2]Rare [3]Epic [4]Legendary
    // Kiekvienos eilutes tikimybiu suma = 1.0
    public float[][] chestRarityProbabilities = new float[][]
    {
        // Lv1: Common  Uncommon  Rare    Epic    Legendary
        new float[] { 0.700f,  0.200f,  0.080f, 0.020f, 0.000f },
        // Lv2: Common  Uncommon  Rare    Epic    Legendary
        new float[] { 0.550f,  0.250f,  0.140f, 0.045f, 0.015f },
        // Lv3: Common  Uncommon  Rare    Epic    Legendary
        new float[] { 0.350f,  0.300f,  0.220f, 0.100f, 0.030f },
        // Lv4: Common  Uncommon  Rare    Epic    Legendary
        new float[] { 0.150f,  0.250f,  0.350f, 0.180f, 0.070f },
        // Lv5: Common  Uncommon  Rare    Epic    Legendary
        new float[] { 0.050f,  0.150f,  0.350f, 0.300f, 0.150f },
    };

    // Grąžina atsitiktinį rarity pagal chesto lygį: 1=Common ... 5=Legendary
    public int RollAnimalRarity(int chestLevel)
    {
        float[] probs = chestRarityProbabilities[chestLevel - 1];
        float roll = Random.value;
        float cumulative = 0f;
        for (int i = 0; i < probs.Length; i++)
        {
            cumulative += probs[i];
            if (roll < cumulative) return i + 1;
        }
        return probs.Length;
    }

    // ============================================================
    // KASDIENINIO GYVŪNO TIKIMYBĖS
    // Kas dieną žaidėjas gauna vieną naują gyvūną.
    // Jis visada bus žemo rarity — tik Common/Uncommon/Rare.
    // Epic ir Legendary galima gauti tik iš chestų.
    // Indeksai: 0=Common(1), 1=Uncommon(2), 2=Rare(3), 3=Epic(4), 4=Legendary(5)
    // Suma = 1.0
    // ============================================================
    [Header("Kasdieninio gyvūno tikimybės")]
    public float[] dailyAnimalProbabilities = new float[]
    {
        0.700f,  // Common     (rarity 1) — 70.0%
        0.250f,  // Uncommon   (rarity 2) — 25.0%
        0.050f,  // Rare       (rarity 3) —  5.0%
        0.000f,  // Epic       (rarity 4) —  0.0%  (tik iš chestų)
        0.000f,  // Legendary  (rarity 5) —  0.0%  (tik iš chestų)
    };

    // Grąžina kasdieninio gyvūno rarity: 1=Common, 2=Uncommon, 3=Rare
    public int RollDailyAnimalRarity()
    {
        float roll = Random.value;
        float cumulative = 0f;
        for (int i = 0; i < dailyAnimalProbabilities.Length; i++)
        {
            cumulative += dailyAnimalProbabilities[i];
            if (roll < cumulative) return i + 1;
        }
        return 1;
    }

    // Pirštinių kainos pagal rarity
    public List<GloveItem> glovePrices = new List<GloveItem>
    {
        new GloveItem { gameObjectId = 1001, type = "Common",    rarity = 1, price =       1000 },
        new GloveItem { gameObjectId = 1002, type = "Uncommon",  rarity = 2, price =      10000 },
        new GloveItem { gameObjectId = 1003, type = "Rare",      rarity = 3, price =      50000 },
        new GloveItem { gameObjectId = 1004, type = "Epic",      rarity = 4, price =     250000 },
        new GloveItem { gameObjectId = 1005, type = "Legendary", rarity = 5, price =    1250000 },
    };

    // Maisto kainos pagal rarity
    public List<FoodItem> foodPrices = new List<FoodItem>
    {
        new FoodItem { gameObjectId = 2001, name = "Kibble",   rarity = 1, foodAmount = 0.10f, price =  60 },
        new FoodItem { gameObjectId = 2002, name = "Snack",    rarity = 2, foodAmount = 0.10f, price =  150 },
        new FoodItem { gameObjectId = 2003, name = "Meal",     rarity = 3, foodAmount = 0.10f, price =  300 },
        new FoodItem { gameObjectId = 2004, name = "Feast",    rarity = 4, foodAmount = 0.10f, price =  560 },
        new FoodItem { gameObjectId = 2005, name = "Delicacy", rarity = 5, foodAmount = 0.10f, price = 1030 },
    };

    // Vandens kainos pagal rarity
    public List<WaterItem> waterPrices = new List<WaterItem>
    {
        new WaterItem { gameObjectId = 3001, name = "Puddle", rarity = 1, waterAmount = 0.10f, price =  60 },
        new WaterItem { gameObjectId = 3002, name = "Cup",    rarity = 2, waterAmount = 0.10f, price =  150 },
        new WaterItem { gameObjectId = 3003, name = "Bottle", rarity = 3, waterAmount = 0.10f, price =  300 },
        new WaterItem { gameObjectId = 3004, name = "Jug",    rarity = 4, waterAmount = 0.10f, price =  560 },
        new WaterItem { gameObjectId = 3005, name = "Spring", rarity = 5, waterAmount = 0.10f, price = 1030 },
    };

    // Gyvuno lygio kėlimo kainos pagal rarity ir lygi
    public List<LevelUpItem> levelUpPrices = new List<LevelUpItem>
{
    // --- RARITY 1 (ID: 4201-4230) ---
    new LevelUpItem { gameObjectId = 4201, rarity = 1, level = 1, price = 350 },
    new LevelUpItem { gameObjectId = 4202, rarity = 1, level = 2, price = 580 },
    new LevelUpItem { gameObjectId = 4203, rarity = 1, level = 3, price = 790 },
    new LevelUpItem { gameObjectId = 4204, rarity = 1, level = 4, price = 1000 },
    new LevelUpItem { gameObjectId = 4205, rarity = 1, level = 5, price = 1220 },
    new LevelUpItem { gameObjectId = 4206, rarity = 1, level = 6, price = 1440 },
    new LevelUpItem { gameObjectId = 4207, rarity = 1, level = 7, price = 1670 },
    new LevelUpItem { gameObjectId = 4208, rarity = 1, level = 8, price = 1910 },
    new LevelUpItem { gameObjectId = 4209, rarity = 1, level = 9, price = 2150 },
    new LevelUpItem { gameObjectId = 4210, rarity = 1, level = 10, price = 2400 },
    new LevelUpItem { gameObjectId = 4211, rarity = 1, level = 11, price = 3540 },
    new LevelUpItem { gameObjectId = 4212, rarity = 1, level = 12, price = 3890 },
    new LevelUpItem { gameObjectId = 4213, rarity = 1, level = 13, price = 4250 },
    new LevelUpItem { gameObjectId = 4214, rarity = 1, level = 14, price = 4620 },
    new LevelUpItem { gameObjectId = 4215, rarity = 1, level = 15, price = 4990 },
    new LevelUpItem { gameObjectId = 4216, rarity = 1, level = 16, price = 5350 },
    new LevelUpItem { gameObjectId = 4217, rarity = 1, level = 17, price = 5700 },
    new LevelUpItem { gameObjectId = 4218, rarity = 1, level = 18, price = 6030 },
    new LevelUpItem { gameObjectId = 4219, rarity = 1, level = 19, price = 6330 },
    new LevelUpItem { gameObjectId = 4220, rarity = 1, level = 20, price = 6610 },
    new LevelUpItem { gameObjectId = 4221, rarity = 1, level = 21, price = 8510 },
    new LevelUpItem { gameObjectId = 4222, rarity = 1, level = 22, price = 8750 },
    new LevelUpItem { gameObjectId = 4223, rarity = 1, level = 23, price = 8990 },
    new LevelUpItem { gameObjectId = 4224, rarity = 1, level = 24, price = 9220 },
    new LevelUpItem { gameObjectId = 4225, rarity = 1, level = 25, price = 9450 },
    new LevelUpItem { gameObjectId = 4226, rarity = 1, level = 26, price = 9670 },
    new LevelUpItem { gameObjectId = 4227, rarity = 1, level = 27, price = 9900 },
    new LevelUpItem { gameObjectId = 4228, rarity = 1, level = 28, price = 10110 },
    new LevelUpItem { gameObjectId = 4229, rarity = 1, level = 29, price = 10330 },
    new LevelUpItem { gameObjectId = 4230, rarity = 1, level = 30, price = 10540 },

    // --- RARITY 2 (ID: 4301-4330) ---
    new LevelUpItem { gameObjectId = 4301, rarity = 2, level = 1, price = 570 },
    new LevelUpItem { gameObjectId = 4302, rarity = 2, level = 2, price = 930 },
    new LevelUpItem { gameObjectId = 4303, rarity = 2, level = 3, price = 1270 },
    new LevelUpItem { gameObjectId = 4304, rarity = 2, level = 4, price = 1610 },
    new LevelUpItem { gameObjectId = 4305, rarity = 2, level = 5, price = 1960 },
    new LevelUpItem { gameObjectId = 4306, rarity = 2, level = 6, price = 2310 },
    new LevelUpItem { gameObjectId = 4307, rarity = 2, level = 7, price = 2680 },
    new LevelUpItem { gameObjectId = 4308, rarity = 2, level = 8, price = 3050 },
    new LevelUpItem { gameObjectId = 4309, rarity = 2, level = 9, price = 3440 },
    new LevelUpItem { gameObjectId = 4310, rarity = 2, level = 10, price = 3840 },
    new LevelUpItem { gameObjectId = 4311, rarity = 2, level = 11, price = 5670 },
    new LevelUpItem { gameObjectId = 4312, rarity = 2, level = 12, price = 6230 },
    new LevelUpItem { gameObjectId = 4313, rarity = 2, level = 13, price = 6810 },
    new LevelUpItem { gameObjectId = 4314, rarity = 2, level = 14, price = 7390 },
    new LevelUpItem { gameObjectId = 4315, rarity = 2, level = 15, price = 7980 },
    new LevelUpItem { gameObjectId = 4316, rarity = 2, level = 16, price = 8560 },
    new LevelUpItem { gameObjectId = 4317, rarity = 2, level = 17, price = 9120 },
    new LevelUpItem { gameObjectId = 4318, rarity = 2, level = 18, price = 9650 },
    new LevelUpItem { gameObjectId = 4319, rarity = 2, level = 19, price = 10130 },
    new LevelUpItem { gameObjectId = 4320, rarity = 2, level = 20, price = 10580 },
    new LevelUpItem { gameObjectId = 4321, rarity = 2, level = 21, price = 13620 },
    new LevelUpItem { gameObjectId = 4322, rarity = 2, level = 22, price = 14010 },
    new LevelUpItem { gameObjectId = 4323, rarity = 2, level = 23, price = 14380 },
    new LevelUpItem { gameObjectId = 4324, rarity = 2, level = 24, price = 14760 },
    new LevelUpItem { gameObjectId = 4325, rarity = 2, level = 25, price = 15120 },
    new LevelUpItem { gameObjectId = 4326, rarity = 2, level = 26, price = 15480 },
    new LevelUpItem { gameObjectId = 4327, rarity = 2, level = 27, price = 15840 },
    new LevelUpItem { gameObjectId = 4328, rarity = 2, level = 28, price = 16190 },
    new LevelUpItem { gameObjectId = 4329, rarity = 2, level = 29, price = 16530 },
    new LevelUpItem { gameObjectId = 4330, rarity = 2, level = 30, price = 16870 },

    // --- RARITY 3 (ID: 4401-4430) ---
    new LevelUpItem { gameObjectId = 4401, rarity = 3, level = 1, price = 910 },
    new LevelUpItem { gameObjectId = 4402, rarity = 3, level = 2, price = 1490 },
    new LevelUpItem { gameObjectId = 4403, rarity = 3, level = 3, price = 2030 },
    new LevelUpItem { gameObjectId = 4404, rarity = 3, level = 4, price = 2580 },
    new LevelUpItem { gameObjectId = 4405, rarity = 3, level = 5, price = 3130 },
    new LevelUpItem { gameObjectId = 4406, rarity = 3, level = 6, price = 3700 },
    new LevelUpItem { gameObjectId = 4407, rarity = 3, level = 7, price = 4290 },
    new LevelUpItem { gameObjectId = 4408, rarity = 3, level = 8, price = 4890 },
    new LevelUpItem { gameObjectId = 4409, rarity = 3, level = 9, price = 5510 },
    new LevelUpItem { gameObjectId = 4410, rarity = 3, level = 10, price = 6150 },
    new LevelUpItem { gameObjectId = 4411, rarity = 3, level = 11, price = 9080 },
    new LevelUpItem { gameObjectId = 4412, rarity = 3, level = 12, price = 9980 },
    new LevelUpItem { gameObjectId = 4413, rarity = 3, level = 13, price = 10890 },
    new LevelUpItem { gameObjectId = 4414, rarity = 3, level = 14, price = 11830 },
    new LevelUpItem { gameObjectId = 4415, rarity = 3, level = 15, price = 12770 },
    new LevelUpItem { gameObjectId = 4416, rarity = 3, level = 16, price = 13700 },
    new LevelUpItem { gameObjectId = 4417, rarity = 3, level = 17, price = 14600 },
    new LevelUpItem { gameObjectId = 4418, rarity = 3, level = 18, price = 15440 },
    new LevelUpItem { gameObjectId = 4419, rarity = 3, level = 19, price = 16210 },
    new LevelUpItem { gameObjectId = 4420, rarity = 3, level = 20, price = 16930 },
    new LevelUpItem { gameObjectId = 4421, rarity = 3, level = 21, price = 21790 },
    new LevelUpItem { gameObjectId = 4422, rarity = 3, level = 22, price = 22410 },
    new LevelUpItem { gameObjectId = 4423, rarity = 3, level = 23, price = 23020 },
    new LevelUpItem { gameObjectId = 4424, rarity = 3, level = 24, price = 23610 },
    new LevelUpItem { gameObjectId = 4425, rarity = 3, level = 25, price = 24200 },
    new LevelUpItem { gameObjectId = 4426, rarity = 3, level = 26, price = 24780 },
    new LevelUpItem { gameObjectId = 4427, rarity = 3, level = 27, price = 25340 },
    new LevelUpItem { gameObjectId = 4428, rarity = 3, level = 28, price = 25900 },
    new LevelUpItem { gameObjectId = 4429, rarity = 3, level = 29, price = 26450 },
    new LevelUpItem { gameObjectId = 4430, rarity = 3, level = 30, price = 27000 },

    // --- RARITY 4 (ID: 4501-4530) ---
    new LevelUpItem { gameObjectId = 4501, rarity = 4, level = 1, price = 1460 },
    new LevelUpItem { gameObjectId = 4502, rarity = 4, level = 2, price = 2380 },
    new LevelUpItem { gameObjectId = 4503, rarity = 4, level = 3, price = 3260 },
    new LevelUpItem { gameObjectId = 4504, rarity = 4, level = 4, price = 4130 },
    new LevelUpItem { gameObjectId = 4505, rarity = 4, level = 5, price = 5020 },
    new LevelUpItem { gameObjectId = 4506, rarity = 4, level = 6, price = 5920 },
    new LevelUpItem { gameObjectId = 4507, rarity = 4, level = 7, price = 6860 },
    new LevelUpItem { gameObjectId = 4508, rarity = 4, level = 8, price = 7820 },
    new LevelUpItem { gameObjectId = 4509, rarity = 4, level = 9, price = 8820 },
    new LevelUpItem { gameObjectId = 4510, rarity = 4, level = 10, price = 9840 },
    new LevelUpItem { gameObjectId = 4511, rarity = 4, level = 11, price = 14520 },
    new LevelUpItem { gameObjectId = 4512, rarity = 4, level = 12, price = 15960 },
    new LevelUpItem { gameObjectId = 4513, rarity = 4, level = 13, price = 17430 },
    new LevelUpItem { gameObjectId = 4514, rarity = 4, level = 14, price = 18930 },
    new LevelUpItem { gameObjectId = 4515, rarity = 4, level = 15, price = 20440 },
    new LevelUpItem { gameObjectId = 4516, rarity = 4, level = 16, price = 21930 },
    new LevelUpItem { gameObjectId = 4517, rarity = 4, level = 17, price = 23360 },
    new LevelUpItem { gameObjectId = 4518, rarity = 4, level = 18, price = 24710 },
    new LevelUpItem { gameObjectId = 4519, rarity = 4, level = 19, price = 25940 },
    new LevelUpItem { gameObjectId = 4520, rarity = 4, level = 20, price = 27090 },
    new LevelUpItem { gameObjectId = 4521, rarity = 4, level = 21, price = 34870 },
    new LevelUpItem { gameObjectId = 4522, rarity = 4, level = 22, price = 35860 },
    new LevelUpItem { gameObjectId = 4523, rarity = 4, level = 23, price = 36830 },
    new LevelUpItem { gameObjectId = 4524, rarity = 4, level = 24, price = 37780 },
    new LevelUpItem { gameObjectId = 4525, rarity = 4, level = 25, price = 38720 },
    new LevelUpItem { gameObjectId = 4526, rarity = 4, level = 26, price = 39640 },
    new LevelUpItem { gameObjectId = 4527, rarity = 4, level = 27, price = 40550 },
    new LevelUpItem { gameObjectId = 4528, rarity = 4, level = 28, price = 41450 },
    new LevelUpItem { gameObjectId = 4529, rarity = 4, level = 29, price = 42330 },
    new LevelUpItem { gameObjectId = 4530, rarity = 4, level = 30, price = 43200 },

    // --- RARITY 5 (ID: 4601-4630) ---
    new LevelUpItem { gameObjectId = 4601, rarity = 5, level = 1, price = 2330 },
    new LevelUpItem { gameObjectId = 4602, rarity = 5, level = 2, price = 3810 },
    new LevelUpItem { gameObjectId = 4603, rarity = 5, level = 3, price = 5210 },
    new LevelUpItem { gameObjectId = 4604, rarity = 5, level = 4, price = 6610 },
    new LevelUpItem { gameObjectId = 4605, rarity = 5, level = 5, price = 8030 },
    new LevelUpItem { gameObjectId = 4606, rarity = 5, level = 6, price = 9480 },
    new LevelUpItem { gameObjectId = 4607, rarity = 5, level = 7, price = 10980 },
    new LevelUpItem { gameObjectId = 4608, rarity = 5, level = 8, price = 12520 },
    new LevelUpItem { gameObjectId = 4609, rarity = 5, level = 9, price = 14110 },
    new LevelUpItem { gameObjectId = 4610, rarity = 5, level = 10, price = 15740 },
    new LevelUpItem { gameObjectId = 4611, rarity = 5, level = 11, price = 23240 },
    new LevelUpItem { gameObjectId = 4612, rarity = 5, level = 12, price = 25550 },
    new LevelUpItem { gameObjectId = 4613, rarity = 5, level = 13, price = 27900 },
    new LevelUpItem { gameObjectId = 4614, rarity = 5, level = 14, price = 30290 },
    new LevelUpItem { gameObjectId = 4615, rarity = 5, level = 15, price = 32700 },
    new LevelUpItem { gameObjectId = 4616, rarity = 5, level = 16, price = 35090 },
    new LevelUpItem { gameObjectId = 4617, rarity = 5, level = 17, price = 37380 },
    new LevelUpItem { gameObjectId = 4618, rarity = 5, level = 18, price = 39540 },
    new LevelUpItem { gameObjectId = 4619, rarity = 5, level = 19, price = 41510 },
    new LevelUpItem { gameObjectId = 4620, rarity = 5, level = 20, price = 43350 },
    new LevelUpItem { gameObjectId = 4621, rarity = 5, level = 21, price = 55800 },
    new LevelUpItem { gameObjectId = 4622, rarity = 5, level = 22, price = 57380 },
    new LevelUpItem { gameObjectId = 4623, rarity = 5, level = 23, price = 58930 },
    new LevelUpItem { gameObjectId = 4624, rarity = 5, level = 24, price = 60460 },
    new LevelUpItem { gameObjectId = 4625, rarity = 5, level = 25, price = 61960 },
    new LevelUpItem { gameObjectId = 4626, rarity = 5, level = 26, price = 63430 },
    new LevelUpItem { gameObjectId = 4627, rarity = 5, level = 27, price = 64880 },
    new LevelUpItem { gameObjectId = 4628, rarity = 5, level = 28, price = 66320 },
    new LevelUpItem { gameObjectId = 4629, rarity = 5, level = 29, price = 67730 },
    new LevelUpItem { gameObjectId = 4630, rarity = 5, level = 30, price = 69120 }
};
    // Gyvuno vietos kainos shope
    public List<SpotItem> spotPrices = new List<SpotItem>
    {
        new SpotItem { gameObjectId = 5001, spot =  1, price =      0 },
        new SpotItem { gameObjectId = 5002, spot =  2, price =   1000 },
        new SpotItem { gameObjectId = 5003, spot =  3, price =   5000 },
        new SpotItem { gameObjectId = 5004, spot =  4, price =   5000 },
        new SpotItem { gameObjectId = 5005, spot =  5, price =  28000 },
        new SpotItem { gameObjectId = 5006, spot =  6, price =  28000 },
        new SpotItem { gameObjectId = 5007, spot =  7, price =  28000 },
        new SpotItem { gameObjectId = 5008, spot =  8, price =  28000 },
        new SpotItem { gameObjectId = 5009, spot =  9, price = 160000 },
        new SpotItem { gameObjectId = 5010, spot = 10, price = 160000 },
    };

    public GloveItem   GetGlove(int rarity)              => glovePrices.Find(x => x.rarity == rarity);
    public FoodItem    GetFood(int gameObjectId)               => foodPrices.Find(x => x.gameObjectId == gameObjectId);
    public WaterItem   GetWater(int gameObjectId)              => waterPrices.Find(x => x.gameObjectId == gameObjectId);
    public SpotItem    GetSpot(int spot)                 => spotPrices.Find(x => x.spot == spot);
    public LevelUpItem GetLevelUp(int rarity, int level) => levelUpPrices.Find(x => x.rarity == rarity && x.level == level);
}
