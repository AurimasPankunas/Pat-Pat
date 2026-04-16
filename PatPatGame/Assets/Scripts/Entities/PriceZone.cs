using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PriceZone : MonoBehaviour
{
    [SerializeField] private ItemBin parentBin;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _trackedGrab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null) return;

        var sellable = grab.GetComponentInChildren<ISellable>();
        if (sellable == null) return;

        _trackedGrab = grab;

        grab.selectEntered.AddListener(OnItemGrabbed);
        grab.selectExited.AddListener(OnItemReleased);


        if (grab.isSelected)
            parentBin.ShowPriceBox(sellable.PriceLabel);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || grab != _trackedGrab) return;

        Cleanup();
    }

    private void OnItemGrabbed(SelectEnterEventArgs args)
    {
        if (_trackedGrab == null) return;

        var sellable = _trackedGrab.GetComponentInChildren<ISellable>();
        if (sellable == null) return;

        parentBin.ShowPriceBox(sellable.PriceLabel);
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
        parentBin.HidePriceBox();
    }

    private void Cleanup()
    {
        parentBin.HidePriceBox();

        if (_trackedGrab != null)
        {
            _trackedGrab.selectEntered.RemoveListener(OnItemGrabbed);
            _trackedGrab.selectExited.RemoveListener(OnItemReleased);
            _trackedGrab = null;
        }
    }
}