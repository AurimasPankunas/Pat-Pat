using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections.Generic;

public class AnimalSell : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject priceTagCanvas;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject showPriceBox;

    // Track ALL items currently in the zone
    private HashSet<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable> _itemsInZone = new();

    private void Start() => HidePrice();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || _itemsInZone.Contains(grab)) return;

        var miniAnimal = grab.GetComponentInChildren<MiniAnimal>();
        if (miniAnimal == null) return;

        if (!grab.isSelected)
        {
            SellAnimal(grab, miniAnimal);
            return;
        }

        _itemsInZone.Add(grab);
        // Show price for the most recently entered held item
        ShowPrice(miniAnimal.data.likes.ToString() + " Like(s)");
        grab.selectExited.AddListener(OnItemReleased);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || !_itemsInZone.Contains(grab)) return;

        grab.selectExited.RemoveListener(OnItemReleased);
        _itemsInZone.Remove(grab);

        // If there are still items in the zone, show the last one's price
        RefreshPrice();
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
        // Find which grab triggered this
        var grab = args.interactableObject as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;
        if (grab == null || !_itemsInZone.Contains(grab)) return;

        var miniAnimal = grab.GetComponentInChildren<MiniAnimal>();
        if (miniAnimal == null) return;

        SellAnimal(grab, miniAnimal);
    }

    private void RefreshPrice()
    {
        // Clean up any destroyed entries first
        _itemsInZone.RemoveWhere(g => g == null);

        if (_itemsInZone.Count == 0)
        {
            HidePrice();
            return;
        }

        // Show price of whichever item remains
        foreach (var remaining in _itemsInZone)
        {
            var miniAnimal = remaining.GetComponentInChildren<MiniAnimal>();
            if (miniAnimal != null)
            {
                ShowPrice(miniAnimal.data.likes.ToString() + " Like(s)");
                return;
            }
        }

        HidePrice();
    }

    private void SellAnimal(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab, MiniAnimal miniAnimal)
    {
        if (grab == null) return;

        foreach (var col in grab.GetComponentsInChildren<Collider>())
            col.enabled = false;

        GameManager.Instance.playerBalance.AddLikes(miniAnimal.data.likes);

        grab.selectExited.RemoveListener(OnItemReleased);
        _itemsInZone.Remove(grab);

        GameManager.Instance.animalManager.RemoveMiniAnimal(miniAnimal);

        GameManager.Instance.questsManager.CaptureProgress(MissionType.SellAnimal, 1);

        RefreshPrice();
    }

    public void ShowPrice(string label)
    {
        if (priceTagCanvas != null) priceTagCanvas.SetActive(true);
        if (priceText != null) priceText.text = label;
    }

    public void HidePrice()
    {
        if (priceTagCanvas != null) priceTagCanvas.SetActive(false);
    }

    // Keep these in sync so AnimalPriceZone calls still work
    public void ShowPriceBox(string label) => ShowPrice(label);
    public void HidePriceBox() => HidePrice();
}