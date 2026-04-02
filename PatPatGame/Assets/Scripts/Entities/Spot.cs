using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spot : MonoBehaviour
{
    [field: SerializeField] public string id { get; private set; }
    [field: SerializeField] public AnimalSize size { get; private set; }
    [field: SerializeField] public Transform spawnPoint { get; private set; }
    public Animal animal { get; private set; }
    [SerializeField] private UIAnimalStatsFunc display;
    private AnimalManager manager;
    private bool isOccupied = false;


    public void Initialize(AnimalManager manager)
    {
        this.manager = manager;
    }

    public void AssignAnimal(Animal animal)
    {
        if (isOccupied)
            return;
            
        this.animal = animal;
        isOccupied = true;
        display.SetAnimal(animal);
    }

    public void RemoveAnimal()
    {
        animal = null;
        isOccupied = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        MiniAnimal animal = other.GetComponent<MiniAnimal>();
        if (animal != null)
            manager.AssignAnimalToSpot(animal.data, this);
    }
}

