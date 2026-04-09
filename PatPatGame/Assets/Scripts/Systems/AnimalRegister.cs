using System.Collections.Generic;
using UnityEngine;

public class AnimalRegister : MonoBehaviour
{
    [SerializeField] private AnimalManager animalManager;
    public List<AnimalData> registerAnimals;
    private Animal testAnimal;

    void Start()
    {
        InvokeRepeating(nameof(TestFunc), 0f, 5f);  // for testing
    }

    /// <summary>
    /// Adds animal data instance to the register list
    /// </summary>
    public void AddAnimalToRegister(AnimalData animalData)
    {
        registerAnimals.Add(animalData);
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
        return removedAnimal;
    }
    
    /// <summary>
    /// Removes animal from the register list
    /// </summary>
    /// <param name="animal"></param>
    public void RemoveAnimal(AnimalData animal)
    {
        registerAnimals.Remove(animal);
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
        int rarity = Random.Range(1, 5);

        AnimalData animal = new AnimalData(type.id, name, rarity);
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

    private void RemoveAnimalAfterDelay()
    {
        animalManager.RemoveAnimalFromSpot(testAnimal);
    }
}
