using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class PriceZone : MonoBehaviour
{
    [SerializeField] private ItemBin parentBin;

    private HashSet<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable> _trackedGrabs = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || _trackedGrabs.Contains(grab)) return;

        var sellable = grab.GetComponentInChildren<ISellable>();
        if (sellable == null) return;

        _trackedGrabs.Add(grab);
        grab.selectEntered.AddListener(OnItemGrabbed);
        grab.selectExited.AddListener(OnItemReleased);

        if (grab.isSelected)
            parentBin.ShowPriceBox(sellable.PriceLabel);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || !_trackedGrabs.Contains(grab)) return;

        RemoveTracked(grab);

        if (!AnyHeldInZone())
            parentBin.HidePriceBox();
    }

    private void OnItemGrabbed(SelectEnterEventArgs args)
    {
        var grab = args.interactableObject as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;
        if (grab == null || !_trackedGrabs.Contains(grab)) return;

        var sellable = grab.GetComponentInChildren<ISellable>();
        if (sellable != null)
            parentBin.ShowPriceBox(sellable.PriceLabel);
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
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