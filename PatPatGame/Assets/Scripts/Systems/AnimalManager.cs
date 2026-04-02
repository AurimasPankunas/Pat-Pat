using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manager for placing/removing and keeping track of physical animals (mini animals & spot animals)
public class AnimalManager : MonoBehaviour
{
    public List<AnimalType> types;
    public List<Spot> spots;
    public List<MiniAnimal> miniAnimals;
    public List<Animal> spotAnimals;
    
    private Dictionary<string, AnimalType> typeLookup;
    private Dictionary<string, Spot> spotLookup;
    private Dictionary<Animal, Spot> spotAnimalLookup;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        spots = FindObjectsByType<Spot>(FindObjectsSortMode.None).ToList();
        spotLookup = spots.ToDictionary(s => s.id, s => s);
        typeLookup = types.ToDictionary(t => t.id, t => t);
        spotAnimalLookup = new Dictionary<Animal, Spot>();

        foreach (Spot spot in spots)
            spot.Initialize(this);
    }


    public Animal AssignAnimalToSpot(AnimalData animalData, Spot spot)
    {
        if (GetType(animalData.typeID).size != spot.size)
            return null;
        GameObject prefab = GetType(animalData.typeID).fullPrefab;
        GameObject spawnedObject = Instantiate(prefab, spot.spawnPoint.position, spot.spawnPoint.rotation);
        Animal spawnedAnimal = spawnedObject.GetComponent<Animal>();
        spawnedAnimal.data = animalData;
        spotAnimals.Add(spawnedAnimal);
        spotAnimalLookup.Add(spawnedAnimal, spot);
        spot.AssignAnimal(spawnedAnimal);
        return spawnedAnimal;
    }

    public AnimalData RemoveAnimalFromSpot(Animal animal)
    {
        AnimalData data = animal.data;
        spotAnimals.Remove(animal);
        spotAnimalLookup.TryGetValue(animal, out var spot);
        spot.RemoveAnimal();
        spotAnimalLookup.Remove(animal);
        Destroy(animal.gameObject);
        return data;
    }

    public AnimalData DespawnAnimal(Animal animal)
    {
        AnimalData data = animal.data;
        Destroy(animal.gameObject);
        return data;
    }

    public AnimalType GetType(string typeID)
    {
        typeLookup.TryGetValue(typeID, out var def);
        return def;
    }

    public AnimalManagerSaveData Save()
    {
        AnimalManagerSaveData data = new AnimalManagerSaveData();
        // data.miniAnimals = miniAnimals.Select(a => a.Save()).ToList();
        // data.spotAnimals = spotAnimals.Select(a => a.data).ToList();
        // data.spots = spots.Select(s => s.Save()).ToList();
        return data;
    }

    public void Load(AnimalManagerSaveData data)
    {
        
    }
}

[System.Serializable]
public struct AnimalManagerSaveData
{
    public List<MiniAnimalData> miniAnimals;
    public List<SpotAnimalData> spotAnimals;
    // public List<AnimalData> animals; // all animals
    // public List<SpotData> spots; // which animal is in which spot
    // public List<MiniAnimalData> miniAnimals; // mini animal locations
    // public List<RegisterAnimalData> registerAnimals; // animals in the register
}

public struct SpotAnimalData
{
    public AnimalData animalData;
    public string spotID;

    public SpotAnimalData(AnimalData animalData, string spotID)
    {
        this.animalData = animalData;
        this.spotID = spotID;
    }
}

public struct MiniAnimalData
{
    public AnimalData animalData;
    public Vector3 position;
    public Quaternion rotation;

    public MiniAnimalData(AnimalData data, Vector3 position, Quaternion rotation) : this()
    {
        this.animalData = data;
        this.position = position;
        this.rotation = rotation;
    }
}