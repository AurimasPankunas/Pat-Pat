using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIAfkScreenFunc : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement _welcomeBackContainer;
    private Label _moneyAmount;
    private Label _timeAmount;
    private Button _closeButton;
    private Button _quitButton;
    private PlayerBalance playerBalance;
    private TimeKeeper timeKeeper;

    void Start()
    {
        playerBalance = GameManager.Instance.playerBalance;
        timeKeeper = GameManager.Instance.timeKeeper;
        // Gets the document on the gameObject that has the UI elements
        _document = GetComponent<UIDocument>();

        // Grabbing UI elements from the document
        _welcomeBackContainer = _document.rootVisualElement.Q<VisualElement>("WelcomeBackContainer");
        _moneyAmount = _document.rootVisualElement.Q<Label>("MoneyAmount");
        _timeAmount = _document.rootVisualElement.Q<Label>("TimeAmount");
        _closeButton = _document.rootVisualElement.Q<Button>("CloseButton");
        _quitButton = _document.rootVisualElement.Q<Button>("QuitButton");

        // Set button function
        _closeButton.RegisterCallback<ClickEvent>(OnCloseClick);
        _quitButton.RegisterCallback<ClickEvent>(OnQuitClick);

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
        _welcomeBackContainer.style.display = DisplayStyle.None;
        _closeButton.UnregisterCallback<ClickEvent>(OnCloseClick);
    }

    public void OnQuitClick(ClickEvent evt)
    {
        // Quitting works only in build mode
        Debug.Log("Quit");
        Application.Quit();
    }

    public void OnDisable()
    {
        _closeButton.UnregisterCallback<ClickEvent>(OnCloseClick);
        _quitButton.UnregisterCallback<ClickEvent>(OnQuitClick);
    }
}
