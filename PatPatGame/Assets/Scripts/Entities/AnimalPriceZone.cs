using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimalPriceZone : MonoBehaviour
{
    [SerializeField] private AnimalSell parentBin;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _trackedGrab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null) return;

        var miniAnimal = grab.GetComponentInChildren<MiniAnimal>();
        if (miniAnimal == null) return;

        _trackedGrab = grab;

        grab.selectEntered.AddListener(OnItemGrabbed);
        grab.selectExited.AddListener(OnItemReleased);

        if (grab.isSelected)
            parentBin.ShowPriceBox(miniAnimal.data.likes.ToString()+" Like(s)");
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

        var miniAnimal = _trackedGrab.GetComponentInChildren<MiniAnimal>();
        if (miniAnimal == null) return;

        parentBin.ShowPriceBox(miniAnimal.data.likes.ToString()+" Like(s)");
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