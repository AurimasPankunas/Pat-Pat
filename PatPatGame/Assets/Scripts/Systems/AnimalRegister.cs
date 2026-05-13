using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
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
    private UIAnimalElement uIAnimalElement;

    void Start()
    {
        InvokeRepeating(nameof(TestDailyFunc), 6f, 12f);
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
    /// Removes animal from the register list. If spawnPoint is given and there are free spots, spawns a mini animal
    /// </summary>
    /// <param name="animal"></param>
    public void RemoveAnimal(
        AnimalData animal,
        Transform spawnPoint,
        TemplateContainer _animalElement
    )
    {
        int freeSpots = animalManager.spots.Count(s => s.isBought && !s.isOccupied);
        int miniAnimals = animalManager.miniAnimals.Count();
        Debug.Log(spawnPoint == null && _animalElement == null);
        if (freeSpots - miniAnimals > 0 || (spawnPoint == null && _animalElement == null))
        {
            registerAnimals.Remove(animal);
            OnRegisterAnimalChanged?.Invoke();
            if (spawnPoint != null)
            {
                _animalElement.RemoveFromHierarchy();
                animalManager.CreateMiniAnimal(animal, spawnPoint.position, spawnPoint.rotation);
            }
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
    private void TestDailyFunc()
    {
        if (registerAnimals.Count < maxRegisterAnimals)
        {
            CreateRandomAnimal();
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
