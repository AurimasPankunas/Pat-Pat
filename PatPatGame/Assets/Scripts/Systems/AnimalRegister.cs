using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimalRegister : MonoBehaviour
{
    [SerializeField]
    private AnimalManager animalManager;

    [SerializeField]
    private ShopItemDatabase shopItemDatabase;
    public List<AnimalData> registerAnimals;
    public int maxRegisterAnimals = 5;
    public event Action OnRegisterAnimalChanged;
    private TimeKeeper timeKeeper;

    void Start()
    {
        timeKeeper = FindFirstObjectByType<TimeKeeper>();
        GetDailyAnimals();
        // InvokeRepeating(nameof(GetDailyAnimals), 6f, 12f);
    }

    /// <summary>
    /// Adds animal data instance to the register list
    /// </summary>
    public void AddAnimalToRegister(AnimalData animalData)
    {
        registerAnimals.Add(animalData);
        OnRegisterAnimalChanged?.Invoke();
    }

    /// <summary>
    /// Removes animal from the register list based on index
    /// </summary>
    public AnimalData RemoveAnimal(int index)
    {
        if (!(index < registerAnimals.Count))
        {
            Debug.LogWarning("Index " + index + " is outside of registered animals range");
            return null;
        }

        AnimalData removedAnimal = registerAnimals[index];
        registerAnimals.RemoveAt(index);
        OnRegisterAnimalChanged?.Invoke();
        return removedAnimal;
    }

    /// <summary>
    /// Removes animal from the register list. If spawnPoint is given, spawns a mini animal
    /// </summary>
    /// <param name="animal"></param>
    public void RemoveAnimal(AnimalData animal, Transform spawnPoint)
    {
        registerAnimals.Remove(animal);
        OnRegisterAnimalChanged?.Invoke();
        if (spawnPoint != null)
        {
            animalManager.CreateMiniAnimal(animal, spawnPoint.position, spawnPoint.rotation);
        }
    }

    /// <summary>
    /// Creates a new animal data instance with randomized stats and puts it in the register
    /// </summary>
    public AnimalData CreateRandomAnimal()
    {
        int typeCount = animalManager.types.Count;
        int typeChoice = Random.Range(0, typeCount);
        AnimalType type = animalManager.types[typeChoice];
        string name = "Bober";
        int rarity = shopItemDatabase.RollDailyAnimalRarity();
        int level = Random.Range(1, 2);
        double happiness = Random.Range(0f, 1f);
        double food = Random.Range(0f, 1f);
        double water = Random.Range(0f, 1f);

        AnimalData animal = new AnimalData(type.id, name, rarity, level, happiness, food, water);
        AddAnimalToRegister(animal);

        return animal;
    }

    /// <summary>
    /// Creates animal with chest rarity
    /// </summary>
    public AnimalData CreateChestAnimal()
    {
        int chestLvl = GameManager.Instance.playerBalance.chestLevel;
        int typeCount = animalManager.types.Count;
        int typeChoice = Random.Range(0, typeCount);
        AnimalType type = animalManager.types[typeChoice];
        string name = $"Chest Lvl." + $"{chestLvl} Bober";
        int rarity = shopItemDatabase.RollAnimalRarity(chestLvl);
        int level = Random.Range(1, 5);
        double happiness = Random.Range(0f, 1f);
        double food = Random.Range(0f, 1f);
        double water = Random.Range(0f, 1f);

        AnimalData animal = new AnimalData(type.id, name, rarity, level, happiness, food, water);
        AddAnimalToRegister(animal);

        return animal;
    }

    // Daily animal spawning test
    private void GetDailyAnimals()
    {
        if (registerAnimals.Count < maxRegisterAnimals)
        {
            var timePassed = DateTime.Now - timeKeeper.lastLogin;
            for (int i = 0; i < timePassed.Days; i++)
            {
                if (registerAnimals.Count == maxRegisterAnimals)
                {
                    return;
                }
                CreateRandomAnimal();
            }
        }
    }

    public AnimalRegisterSaveData Save()
    {
        AnimalRegisterSaveData data = new AnimalRegisterSaveData { animals = registerAnimals };
        return data;
    }

    public void Load(AnimalRegisterSaveData data)
    {
        registerAnimals.Clear();
        data.animals.ForEach(a => registerAnimals.Add(a));
    }
}

[System.Serializable]
public struct AnimalRegisterSaveData
{
    public List<AnimalData> animals;
}
