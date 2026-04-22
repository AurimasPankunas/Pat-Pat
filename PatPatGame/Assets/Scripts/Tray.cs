using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Tray : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Rigidbody item = other.gameObject.GetComponentInParent<Rigidbody>();
        if (item != null)
        {
            if(other.CompareTag("Food") || other.CompareTag("Drink")) {
                // Freeze the item's physics
                item.constraints = RigidbodyConstraints.FreezeAll;
            
                // Parent the item (parent and all children) to the tray so it moves with it
                item.gameObject.transform.SetParent(transform);

                var grabInteractable = item.gameObject.GetComponent<XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    grabInteractable.selectEntered.AddListener((SelectEnterEventArgs args) => RemoveFromTray(item));
                }
            }
        }
    }

    void RemoveFromTray(Rigidbody item)
    {
        if (item != null)
        {
            item.gameObject.transform.SetParent(null);
            item.constraints = RigidbodyConstraints.None;
        }
    }
}
