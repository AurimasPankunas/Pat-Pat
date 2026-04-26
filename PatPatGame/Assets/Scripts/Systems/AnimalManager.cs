using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manager for placing, removing and keeping track of physical animals (mini animals & spot animals)
public class AnimalManager : MonoBehaviour
{
    [field: SerializeField] public List<AnimalType> types { get; private set; }  // all animal types (Scriptable Objects)
    public List<Spot> spots { get; private set; }
    public List<MiniAnimal> miniAnimals { get; private set; }
    public List<Animal> spotAnimals { get; private set; }
    
    private Dictionary<string, AnimalType> typeLookup;
    private Dictionary<string, Spot> spotLookup;
    private Dictionary<Animal, Spot> spotAnimalLookup;


    /// <summary>
    /// Gathers and initializes spots and sets up dictionaries
    /// </summary>
    public void Initialize(bool isNewSave)
    {
        spots = FindObjectsByType<Spot>(FindObjectsSortMode.None).ToList();
        miniAnimals = new List<MiniAnimal>();
        spotAnimals = new List<Animal>();
        spotLookup = spots.ToDictionary(s => s.id, s => s);
        typeLookup = types.ToDictionary(t => t.id, t => t);
        spotAnimalLookup = new Dictionary<Animal, Spot>();

        foreach (Spot spot in spots)
        {
            spot.Initialize(this);
            Animal spotAnimal = spot.animal;
            
            if (spotAnimal == null)
            continue;
           
            if (isNewSave)
            {
                spotAnimal.Initialize();
                spotAnimals.Add(spotAnimal);
                spotAnimalLookup.Add(spotAnimal, spot);
                spot.AssignAnimal(spotAnimal);
            }
            else
            {
                spot.RemoveAnimal();
                Destroy(spotAnimal.gameObject);
            }
            
        }
    }

    /// <summary>
    /// Spawns a mini animal at a given spawn point
    /// </summary>
    public MiniAnimal CreateMiniAnimal(AnimalData animalData, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject prefab = GetType(animalData.typeID).miniPrefab;
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, spawnRotation);
        MiniAnimal spawnedAnimal = spawnedObject.GetComponent<MiniAnimal>();
        spawnedAnimal.data = animalData;
        miniAnimals.Add(spawnedAnimal);
        return spawnedAnimal;
    }

    /// <summary>
    /// Destroys the mini animal game GameObject
    /// </summary>
    private AnimalData RemoveMiniAnimal(MiniAnimal miniAnimal)
    {
        miniAnimals.Remove(miniAnimal);
        AnimalData data = miniAnimal.data;
        Destroy(miniAnimal.gameObject);
        return data;
    }

    /// <summary>
    /// Converts mini animal to spot animal
    /// </summary>
    public Animal MoveMiniAnimalToSpot(MiniAnimal miniAnimal, Spot spot)
    {
        Animal animal = AddAnimalToSpot(miniAnimal.data, spot);

        // check if animal assignment to spot was successful
        if (animal != null)
            RemoveMiniAnimal(miniAnimal);

        return animal;
    }

    /// <summary>
    /// Spawns an animal into the scene and assigns it to a spot
    /// </summary>
    /// <returns>spawned animal</returns>
    private Animal AddAnimalToSpot(AnimalData animalData, Spot spot)
    {
        if (GetType(animalData.typeID).size != spot.size)
            return null;

        if (spot.isOccupied || !spot.isBought)
            return null;

        // Creating a new animal GameObject
        GameObject prefab = GetType(animalData.typeID).fullPrefab;
        GameObject spawnedObject = Instantiate(prefab, spot.spawnPoint.position, spot.spawnPoint.rotation);
        Animal spawnedAnimal = spawnedObject.GetComponent<Animal>();
        spawnedAnimal.data = animalData;

        // Updating state
        spotAnimals.Add(spawnedAnimal);
        spotAnimalLookup.Add(spawnedAnimal, spot);
        spot.AssignAnimal(spawnedAnimal);

        return spawnedAnimal;
    }

    /// <summary>
    /// Clears the spot occupied by the specified animal and destroys the animal GameObject
    /// </summary>
    /// <returns>removed animal instance data</returns>
    public AnimalData RemoveAnimalFromSpot(Animal animal)
    {
        AnimalData data = animal.data;
        spotAnimals.Remove(animal);
        spotAnimalLookup.TryGetValue(animal, out var spot);
        spot.RemoveAnimal();
        spotAnimalLookup.Remove(animal);

        // Move the animal far away and let its scripts finish before destroying
        animal.gameObject.transform.Translate(new Vector3(0,-1000,0), Space.World);
        Destroy(animal.gameObject, 4);
        Debug.Log("Animal removal code reached return");
        return data;
    }

    /// <summary>
    /// Clears the spot occupied by the specified animal, destroys the animal GameObject and spawns a mini animal
    /// </summary>
    public AnimalData RemoveAnimalFromSpot(Animal animal, Vector3 position, Quaternion rotation)
    {
        AnimalData data = RemoveAnimalFromSpot(animal);
            CreateMiniAnimal(data, position, rotation);
        return data;
    }

    /// <summary>
    /// Clears the spot occupied an the animal and destroys the animal GameObject
    /// </summary>
    /// <returns>removed animal instance data</returns>
    public AnimalData RemoveAnimalFromSpot(Spot spot)
    {
        Animal animal = spot.animal;
        if (animal == null)
        {
            Debug.LogWarning("Attempting to remove an animal from an empty spot: " + spot.id);
            return null;
        }
        AnimalData data = animal.data;
        spotAnimals.Remove(animal);
        spot.RemoveAnimal();
        spotAnimalLookup.Remove(animal);
        Destroy(animal.gameObject);
        return data;
    }

    /// <summary>
    /// Returns animal type data with the given ID
    /// </summary>
    public AnimalType GetType(string typeID)
    {
        typeLookup.TryGetValue(typeID, out var def);
        return def;
    }

    /// <summary>
    /// Returns a spot with the given ID
    /// </summary>
    public Spot GetSpot(string spotID)
    {
        spotLookup.TryGetValue(spotID, out var spot);
        return spot;
    }

    public AnimalManagerSaveData Save()
    {
        miniAnimals = FindObjectsByType<MiniAnimal>(FindObjectsSortMode.None).ToList();

        AnimalManagerSaveData data = new AnimalManagerSaveData
        {
            miniAnimals = miniAnimals.Select(m => new MiniAnimalSaveData(m.data, m.transform.position, m.transform.rotation)).ToList(),
            spotAnimals = this.spotAnimals.Select(s => new SpotAnimalSaveData(s.data, spotAnimalLookup[s].id)).ToList(),
            spots = this.spots.Select(s => new SpotData(s.id, s.isBought)).ToList()
        };
        return data;
    }

    public void Load(AnimalManagerSaveData data)
    {
        data.spots.ForEach(s => spotLookup[s.spotID].UpdateSpotBought(s.isBought));
        data.spotAnimals.ForEach(s => AddAnimalToSpot(s.animalData, GetSpot(s.spotID)));
        data.miniAnimals.ForEach(m => CreateMiniAnimal(m.animalData, m.position, m.rotation));
    }
}

[System.Serializable]
public struct AnimalManagerSaveData
{
    public List<MiniAnimalSaveData> miniAnimals;
    public List<SpotAnimalSaveData> spotAnimals;
    public List<SpotData> spots;
}

[System.Serializable]
public struct SpotAnimalSaveData
{
    public string spotID;
    public AnimalData animalData;

    public SpotAnimalSaveData(AnimalData animalData, string spotID)
    {
        this.animalData = animalData;
        this.spotID = spotID;
    }
}

[System.Serializable]
public struct MiniAnimalSaveData
{
    public AnimalData animalData;
    public Vector3 position;
    public Quaternion rotation;

    public MiniAnimalSaveData(AnimalData animalData, Vector3 position, Quaternion rotation) : this()
    {
        this.animalData = animalData;
        this.position = position;
        this.rotation = rotation;
    }
}

[System.Serializable]
public struct SpotData
{
    public string spotID;
    public bool isBought;
    public SpotData(string spotID, bool isBought)
    {
        this.spotID = spotID;
        this.isBought = isBought;
    }
}
