using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnimalManager : MonoBehaviour
{
    public List<AnimalType> types;
    public List<Spot> spots;
    public List<AnimalData> registerAnimals;
    public List<Animal> transportedAnimals;
    public List<Animal> spotAnimals;
    
    private Dictionary<string, AnimalType> typeLookup;
    private Dictionary<string, Animal> spawnedAnimals = new Dictionary<string, Animal>();
    private Dictionary<string, AnimalData> allAnimals = new Dictionary<string, AnimalData>();

    private void Awake()
    {
        typeLookup = types.ToDictionary(d => d.id, d => d);
    }


    public Animal GetAnimalByID(string instanceID)
    {
        spawnedAnimals.TryGetValue(instanceID, out var animal);
        return animal;
    }

    public void RegisterAnimal(AnimalData animalData)
    {
        registerAnimals.Add(animalData);
    }

    public Animal SpawnAnimal(AnimalData animalData, Transform spawnPoint, bool fullVersion)
    {
        GameObject prefab = fullVersion ? GetType(animalData.typeID).fullPrefab : GetType(animalData.typeID).miniPrefab;

        GameObject spawnedObject = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        Animal spawnedAnimal = spawnedObject.GetComponent<Animal>();
        spawnedAnimal.data = animalData;
        spawnedAnimals[animalData.instanceID] = spawnedAnimal;

        return spawnedAnimal;
    }

    public AnimalData DespawnAnimal(Animal animal)
    {
        AnimalData data = animal.data;
        spawnedAnimals.Remove(animal.data.instanceID);
        Destroy(animal.gameObject);
        return data;
    }

    public AnimalType GetType(string typeID)
    {
        typeLookup.TryGetValue(typeID, out var def);
        return def;
    }
}

[System.Serializable]
public struct AnimalManagerSaveData
{
    public List<AnimalData> animals; // all registered and spawned animals
    public List<SpotSaveData> spots; // which animal is in which spot
}

// Example for spots
[System.Serializable]
public struct SpotSaveData
{
    public string spotID;
    public string assignedAnimalID;
}
