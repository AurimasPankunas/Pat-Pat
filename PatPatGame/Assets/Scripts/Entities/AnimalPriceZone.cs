using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class AnimalPriceZone : MonoBehaviour
{
    [SerializeField] private AnimalSell parentBin;

    // Track ALL grabs in this zone, not just one
    private HashSet<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable> _trackedGrabs = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || _trackedGrabs.Contains(grab)) return;

        var miniAnimal = grab.GetComponentInChildren<MiniAnimal>();
        if (miniAnimal == null) return;

        _trackedGrabs.Add(grab);
        grab.selectEntered.AddListener(OnItemGrabbed);
        grab.selectExited.AddListener(OnItemReleased);

        if (grab.isSelected)
            parentBin.ShowPriceBox(miniAnimal.data.likes.ToString() + " Like(s)");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || !_trackedGrabs.Contains(grab)) return;

        // Only clean up this specific item
        RemoveTracked(grab);

        // If no held items remain in zone, hide price
        if (!AnyHeldInZone())
            parentBin.HidePriceBox();
    }

    private void OnItemGrabbed(SelectEnterEventArgs args)
    {
        var grab = args.interactableObject as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;
        if (grab == null || !_trackedGrabs.Contains(grab)) return;

        var miniAnimal = grab.GetComponentInChildren<MiniAnimal>();
        if (miniAnimal != null)
            parentBin.ShowPriceBox(miniAnimal.data.likes.ToString() + " Like(s)");
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
        // Only hide if no other held items remain in the zone
        if (!AnyHeldInZone())
            parentBin.HidePriceBox();
    }

    private bool AnyHeldInZone()
    {
        foreach (var g in _trackedGrabs)
            if (g != null && g.isSelected) return true;
        return false;
    }

    private void RemoveTracked(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab)
    {
        grab.selectEntered.RemoveListener(OnItemGrabbed);
        grab.selectExited.RemoveListener(OnItemReleased);
        _trackedGrabs.Remove(grab);
    }
}