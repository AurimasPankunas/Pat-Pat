using UnityEngine;
using UnityEngine.InputSystem;

public class HandGripColliderController : MonoBehaviour
{
    [SerializeField] private SphereCollider triggerCollider; // The IsTrigger collider
    [SerializeField] private InputActionProperty gripAction;

    void Update()
    {
        float gripValue = gripAction.action.ReadValue<float>();
        
        if (gripValue > 0.5f) // Grip button pressed
        {
            if (triggerCollider != null && triggerCollider.enabled)
            {
                triggerCollider.enabled = false;
            }
        }
        else // Grip button released
        {
            if (triggerCollider != null && !triggerCollider.enabled)
            {
                triggerCollider.enabled = true;
            }
        }
    }
}