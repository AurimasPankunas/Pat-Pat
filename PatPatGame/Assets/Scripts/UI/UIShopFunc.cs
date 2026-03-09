using UnityEngine;
using UnityEngine.UIElements;

public class UIShopFunc : MonoBehaviour
{
    private UIDocument _document;
    private Label _moneyAmount;
    void Awake()
    {
        // Gets the document on the gameObject that has the UI elements
        _document = GetComponent<UIDocument>();

        // Grabbing UI elements from the document
        _moneyAmount = _document.rootVisualElement.Q<Label>("MoneyAmount");
    }

    /// <summary>
    /// Sets the money amount in the UI
    /// </summary>
    /// <param name="value"></param>
    public void SetMoneyAmount(double value)
    {
        _moneyAmount.text = value.ToString() + "$";
    }
}
