using Unity.VisualScripting;
using UnityEngine;

public class Spot : MonoBehaviour
{
    public SpotData data;
    public AnimalManager animalManager;
    public Animal animal { get; private set; }
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private UIAnimalStatsFunc display;
    private bool isOccupied = false;

    public void AssignAnimal(Animal animal)
    {
        this.animal = animalManager.SpawnAnimal(animal.data, spawnPoint, true);
        isOccupied = true;
    }

    public void RemoveAnimal()
    {
        animal = null;
        isOccupied = false;
    }
}

public struct SpotData
{
    public string spotID;
    public string assignedAnimalID;
}
