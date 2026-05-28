using UnityEngine;

public class Teleport_Items_Back : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.transform.root.position = new Vector3(2f, 2f, 1f);
    }
}
