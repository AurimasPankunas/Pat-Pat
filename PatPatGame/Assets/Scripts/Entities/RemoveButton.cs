using UnityEngine;

public class SpotRemoveButton : MonoBehaviour
{
    [SerializeField] private Spot spot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
            spot.TryRemoveAnimal();
    }
}