using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;
using Unity.XR.CoreUtils;

public class Tray : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Rigidbody item = other.gameObject.GetComponentInParent<Rigidbody>();
        if (item != null)
        {
            if((other.CompareTag("Food") || other.CompareTag("Drink")) && !item.isKinematic) {
                // Freeze the item's physics
                //Debug.Log("TriggerEnter");
                PlaceObject(item.gameObject);
            }
        }
    }

    /// <summary>
    /// Freeze an object in the tray as its parent and setting its rigidbody as Kinematic
    /// </summary>
    void PlaceObject(GameObject obj)
    {
        //Debug.Log("Placing "+obj.name);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        obj.transform.SetParent(transform, true);
        // Set layer to ContainedObject, in order for tray's Rigidbody
        // to ignore collisions with objects inside it
        obj.SetLayerRecursively(6);
        
        
        // Setting movement type for interactable and
        // adding listener so object can get removed from tray
        XRGrabInteractable grabInteractable = obj.GetComponent<XRGrabInteractable>();
        grabInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;
        grabInteractable.selectExited.AddListener(RemoveObject);
    }

    /// <summary>
    /// Unfreeze an object from the tray
    /// </summary>
    private void RemoveObject(BaseInteractionEventArgs args)
    {
        GameObject obj = args.interactableObject.transform.gameObject;
        RemoveObject(obj);
    }

    private void RemoveObject(GameObject obj)
    {
        //Debug.Log("Removing "+obj.name);
        obj.transform.SetParent(null);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        // Set layer back to Default
        obj.SetLayerRecursively(0);
        rb.constraints = RigidbodyConstraints.None;

        XRGrabInteractable grabInteractable = obj.GetComponent<XRGrabInteractable>();
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        //reset its eventListeners
        grabInteractable.selectExited.RemoveListener(RemoveObject);
    }
}
