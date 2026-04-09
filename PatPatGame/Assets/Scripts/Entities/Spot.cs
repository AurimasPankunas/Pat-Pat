using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spot : MonoBehaviour
{
    private AnimalManager manager;
    [field: SerializeField] public string id { get; private set; }
    [field: SerializeField] public double price { get; private set; }
    [field: SerializeField] public AnimalSize size { get; private set; }
    [field: SerializeField] public Transform spawnPoint { get; private set; }
    [field: SerializeField] public Animal animal { get; private set; }  // serialized so that animals can be set up for a new save
    public bool isOccupied { get; private set; }

    public event Action<Animal> OnSpotAnimalChanged;

    public void Initialize(AnimalManager manager)
    {
        this.manager = manager;
        isOccupied = false;
    }

    /// <summary>
    /// Updates spot state with the assigned animal
    /// </summary>
    public void AssignAnimal(Animal animal)
    {
        this.animal = animal;
        isOccupied = true;
        OnSpotAnimalChanged?.Invoke(animal);
    }

    /// <summary>
    /// Updates spot state after removing an animal
    /// </summary>
    public void RemoveAnimal()
    {
        animal = null;
        isOccupied = false;
        OnSpotAnimalChanged?.Invoke(null);
    }

    // Untested
    // Assigning animals by hand would probably look something like this.
    // Also other methods would need to enable/disable the collider at
    // appropriate times (performance reasons mostly) and prevent removed
    // animal from being instantly reassigned if mini variant spawns inside the collider.
    private void OnTriggerEnter(Collider other)
    {
        MiniAnimal animal = other.GetComponent<MiniAnimal>();
        if (animal != null)
            manager.AddAnimalToSpot(animal.data, this);
        return;
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
