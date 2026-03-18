using Unity.VisualScripting;
using UnityEngine;

public class ConveyorBeltTest : MonoBehaviour
{
    private ConveyorBelt conveyorBelt;
    [SerializeField] private GameObject testobject;

    void Start()
    {
        conveyorBelt = FindFirstObjectByType<ConveyorBelt>();
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Test");
        GameObject item = Instantiate(testobject);
        conveyorBelt.SpawnItemOnBelt(item);
    }
}
