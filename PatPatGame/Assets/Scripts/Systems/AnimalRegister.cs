using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimalRegister : MonoBehaviour
{
    [SerializeField] private AnimalManager animalManager;
    [SerializeField] private ShopItemDatabase shopItemDatabase;
    public List<AnimalData> registerAnimals;
    public int maxRegisterAnimals = 5;
    private Animal testAnimal;
    public event Action OnRegisterAnimalChanged;

    void Start()
    {
        InvokeRepeating(nameof(TestFunc), 0f, 5f);  // for testing
        InvokeRepeating(nameof(TestDailyFunc), 0f, 12f);
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
    /// Removes animal from the register list
    /// </summary>
    /// <param name="animal"></param>
    public void RemoveAnimal(AnimalData animal)
    {
        registerAnimals.Remove(animal);
        OnRegisterAnimalChanged?.Invoke();
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
        string name = $"Chest Lvl." +
            $"{chestLvl} Bober";
        int rarity = shopItemDatabase.RollAnimalRarity(chestLvl);
        int level = Random.Range(1, 5);
        double happiness = Random.Range(0f, 1f);
        double food = Random.Range(0f, 1f);
        double water = Random.Range(0f, 1f);

    AnimalData animal = new AnimalData(type.id, name, rarity,level,happiness,food,water);
        AddAnimalToRegister(animal);

        return animal;
    }

    // For testing, remove after proper implementation
    private void TestFunc()
    {
        AnimalData an1 = CreateRandomAnimal();
        Spot spot = animalManager.GetSpot("S1");
        if (spot == null)
            return;
        animalManager.AddAnimalToSpot(an1, spot);
        testAnimal = spot.animal;
        RemoveAnimal(an1);
        if (testAnimal != null)
            Invoke("RemoveAnimalAfterDelay", 4f);
    }
    // Daily animal spawning test
    private void TestDailyFunc()
    {
        if(registerAnimals.Count < maxRegisterAnimals)
        {
            CreateRandomAnimal();
        }
    }

    private void RemoveAnimalAfterDelay()
    {
        animalManager.RemoveAnimalFromSpot(testAnimal);
    }
}
