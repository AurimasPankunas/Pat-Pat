using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

/// <summary>
/// Drop onto your animal bin model. Requires a trigger Collider.
/// Sells MiniAnimals by adding their likes to the player's likes balance.
/// </summary>
public class AnimalSell : MonoBehaviour
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

        var miniAnimal = grab.GetComponentInChildren<MiniAnimal>();
        if (miniAnimal == null) return;

        if (!grab.isSelected)
        {
            SellAnimal(grab, miniAnimal);
            return;
        }

        _hoveredItem = grab;
        ShowPrice(miniAnimal.data.likes.ToString()+" Like(s)");
        grab.selectExited.AddListener(OnItemReleased);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsChildOf(transform)) return;

        var grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null || grab != _hoveredItem) return;

        grab.selectExited.RemoveListener(OnItemReleased);
        _hoveredItem = null;
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
        if (_hoveredItem == null) return;

        var miniAnimal = _hoveredItem.GetComponentInChildren<MiniAnimal>();
        if (miniAnimal == null) return;

        SellAnimal(_hoveredItem, miniAnimal);
    }

    private void SellAnimal(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab, MiniAnimal miniAnimal)
    {
        if (grab == null) return;

        foreach (var col in grab.GetComponentsInChildren<Collider>())
            col.enabled = false;

        GameManager.Instance.playerBalance.AddLikes(miniAnimal.data.likes);

        grab.selectExited.RemoveListener(OnItemReleased);
        _hoveredItem = null;

        GameManager.Instance.animalManager.miniAnimals.Remove(miniAnimal);

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