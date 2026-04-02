using System.Collections.Generic;
using UnityEngine;

public class AnimalRegister : MonoBehaviour
{
    [SerializeField] private AnimalManager animalManager;
    public List<AnimalData> registerAnimals;
    private Animal testAnimal;

    void Start()
    {
        InvokeRepeating(nameof(TestFunc), 0f, 5f);
    }

    public void RegisterAnimal(AnimalData animalData)
    {
        registerAnimals.Add(animalData);
    }

    public void RemoveAnimal(int index)
    {
        if (!(index < registerAnimals.Count))
        {
            Debug.LogWarning("Index " + index + " is outside of registered animals range");
            return;
        }

        registerAnimals.RemoveAt(index);
    }
    
    public void RemoveAnimal(AnimalData animal)
    {
        registerAnimals.Remove(animal);
    }

    public AnimalData createRandomAnimal()
    {
        int typeCount = animalManager.types.Count;
        int typeChoice = Random.Range(0, typeCount);
        AnimalType type = animalManager.types[typeChoice];
        string name = "Bober";
        int rarity = Random.Range(1, 5);

        AnimalData animal = AnimalData.Create(type.id, name, rarity);
        RegisterAnimal(animal);

        return animal;
    }

    private void TestFunc()
    {
        AnimalData an1 = createRandomAnimal();
        testAnimal = animalManager.AssignAnimalToSpot(an1, animalManager.spots[0]);
        RemoveAnimal(an1);
        Invoke("RemoveAnimalAfterDelay", 4f);
    }

    private void RemoveAnimalAfterDelay()
    {
        animalManager.RemoveAnimalFromSpot(testAnimal);
    }
}
