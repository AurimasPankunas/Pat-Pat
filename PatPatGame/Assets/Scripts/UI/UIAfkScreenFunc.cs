using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIAfkScreenFunc : MonoBehaviour
{
    private UIDocument _document;
    private Label _moneyAmount;
    private Label _timeAmount;
    private Button _closeButton;
    private PlayerBalance playerBalance;
    private TimeKeeper timeKeeper;

    void Start()
    {
        playerBalance = GameManager.Instance.playerBalance;
        timeKeeper = GameManager.Instance.timeKeeper;
        // Gets the document on the gameObject that has the UI elements
        _document = GetComponent<UIDocument>();

        // Grabbing UI elements from the document
        _moneyAmount = _document.rootVisualElement.Q<Label>("MoneyAmount");
        _timeAmount = _document.rootVisualElement.Q<Label>("TimeAmount");
        _closeButton = _document.rootVisualElement.Q<Button>("CloseButton");

        // Set button function
        _closeButton.RegisterCallback<ClickEvent>(OnCloseClick);

        // Set visuals
        TimeSpan time = DateTime.Now - timeKeeper.lastLogin;
        string lastTime = "";
        if (time.Days > 0)
        {
            lastTime += time.Days + "d ";
        }
        lastTime += time.ToString(@"hh\:mm\:ss");
        _timeAmount.text = lastTime;
        _moneyAmount.text = playerBalance.earnedMoneyOffline.ToString("N2");

    }

    public void OnCloseClick(ClickEvent evt)
    {
        OnDisable();
        Destroy(this.gameObject);
    }

    public void OnDisable()
    {
        _closeButton.UnregisterCallback<ClickEvent>(OnCloseClick);
    }
}
