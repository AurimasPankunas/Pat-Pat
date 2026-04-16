using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;


public class ItemBin : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject priceTagCanvas;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject showPriceBox;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _hoveredItem;

    private void Start()
    {
        HidePrice();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null) return;

        var sellable = grab.GetComponentInChildren<ISellable>();
        if (sellable == null) return;

        if (!grab.isSelected)
        {
            SellItem(grab, sellable);
            return;
        }

        _hoveredItem = grab;
        ShowPrice(sellable.PriceLabel);
        grab.selectExited.AddListener(OnItemReleased);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || grab != _hoveredItem) return;

        HidePrice();
        grab.selectExited.RemoveListener(OnItemReleased);
        _hoveredItem = null;
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
        if (_hoveredItem == null) return;

        var sellable = _hoveredItem.GetComponentInChildren<ISellable>();
        if (sellable == null) return;

        SellItem(_hoveredItem, sellable);
    }

    private void SellItem(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab, ISellable sellable)
    {
        if (grab == null) return;

        foreach (var col in grab.GetComponentsInChildren<Collider>())
            col.enabled = false;

        GameManager.Instance.playerBalance.AddMoney(sellable.SellValue);

        grab.selectExited.RemoveListener(OnItemReleased);
        HidePrice();
        _hoveredItem = null;

        Destroy(grab.gameObject);
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

    public void ShowPriceBox(string label)
    {
        if (priceTagCanvas != null) priceTagCanvas.SetActive(true);
        if (priceText != null) priceText.text = label;
    }

    public void HidePriceBox()
    {
        if (priceTagCanvas != null) priceTagCanvas.SetActive(false);
    }
}