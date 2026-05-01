using System;
using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Collider))]
public class Spot : MonoBehaviour
{
    private AnimalManager manager;
    private Collider col;
    [field: SerializeField] public string id { get; private set; }
    [field: SerializeField] public double price { get; private set; }
    [field: SerializeField] public AnimalSize size { get; private set; }
    [field: SerializeField] public Transform spawnPoint { get; private set; }
    [field: SerializeField] public Animal animal { get; private set; }  // serialized so that animals can be set up for a new save
    public bool isOccupied { get; private set; }
    private bool isPlacingEnabled = false;
    [field: SerializeField] public bool isBought { get; private set; }
    [SerializeField] private InputActionProperty removeAnimalInputAction;

    public event Action<Animal> OnSpotAnimalChanged;
    public event Action<bool> OnSpotBoughtUpdated;  // for loading
    public event Action<bool> OnSpotBought;  // 

    public void Initialize(AnimalManager manager)
    {
        this.manager = manager;
        isOccupied = false;
        col = GetComponent<Collider>();
        StartCoroutine(EnablePlacingAfterTime(1));
    }

    /// <summary>
    /// Updates spot state with the assigned animal
    /// </summary>
    public void AssignAnimal(Animal animal)
    {
        this.animal = animal;
        isOccupied = true;
        isPlacingEnabled = false;
        // col.enabled = false;
        OnSpotAnimalChanged?.Invoke(animal);

    }

    /// <summary>
    /// Updates spot state after removing an animal
    /// </summary>
    public void RemoveAnimal()
    {
        animal = null;
        isOccupied = false;
        StartCoroutine(EnablePlacingAfterTime(5));
        OnSpotAnimalChanged?.Invoke(null);
    }

    /// <summary>
    /// Updates spot bought state (for loading save data)
    /// </summary>
    public void UpdateSpotBought(bool value)
    {
        isBought = value;
        OnSpotBought?.Invoke(value);
    }

    /// <summary>
    /// Sets spot bought state to true
    /// </summary>
    public void BuySpot()
    {
        isBought = true;
        OnSpotBoughtUpdated?.Invoke(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        MiniAnimal animal = other.GetComponent<MiniAnimal>();
        if (isPlacingEnabled && animal != null)
            manager.MoveMiniAnimalToSpot(animal, this);
    } 

    public void TryRemoveAnimal()
    {
        if (isOccupied)
            manager.RemoveAnimalFromSpot(animal, spawnPoint.position + new Vector3(0, 1, 0), spawnPoint.rotation);
    }

    private IEnumerator EnablePlacingAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (isOccupied == false)
            isPlacingEnabled = true;
    } 
}
