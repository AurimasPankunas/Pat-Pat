using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections.Generic;

public class ItemBin : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject priceTagCanvas;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject showPriceBox;

    private HashSet<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable> _itemsInZone = new();

    private void Start() => HidePrice();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || _itemsInZone.Contains(grab)) return;

        var sellable = grab.GetComponentInChildren<ISellable>();
        if (sellable == null) return;

        if (!grab.isSelected)
        {
            SellItem(grab, sellable);
            return;
        }

        _itemsInZone.Add(grab);
        ShowPrice(sellable.PriceLabel);
        grab.selectExited.AddListener(OnItemReleased);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || !_itemsInZone.Contains(grab)) return;

        grab.selectExited.RemoveListener(OnItemReleased);
        _itemsInZone.Remove(grab);

        RefreshPrice();
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
        var grab = args.interactableObject as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;
        if (grab == null || !_itemsInZone.Contains(grab)) return;

        var sellable = grab.GetComponentInChildren<ISellable>();
        if (sellable == null) return;

        SellItem(grab, sellable);
    }

    private void RefreshPrice()
    {
        _itemsInZone.RemoveWhere(g => g == null);

        if (_itemsInZone.Count == 0)
        {
            HidePrice();
            return;
        }

        foreach (var remaining in _itemsInZone)
        {
            var sellable = remaining.GetComponentInChildren<ISellable>();
            if (sellable != null)
            {
                ShowPrice(sellable.PriceLabel);
                return;
            }
        }

        HidePrice();
    }

    private void SellItem(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab, ISellable sellable)
    {
        if (grab == null) return;

        foreach (var col in grab.GetComponentsInChildren<Collider>())
            col.enabled = false;

        GameManager.Instance.playerBalance.AddMoney(sellable.SellValue);

        grab.selectExited.RemoveListener(OnItemReleased);
        _itemsInZone.Remove(grab);

        Destroy(grab.gameObject);

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

    public void ShowPriceBox(string label) => ShowPrice(label);
    public void HidePriceBox() => HidePrice();
}