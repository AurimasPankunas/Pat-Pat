using UnityEngine;
using UnityEngine.UIElements;


public class UIAnimalElement
{
    public AnimalData animalData;
    private RegisterManager registerManager;

    public TemplateContainer _animalElement;
    private Label _animalName;
    private Image _animalImage;
    private Label _animalLevel;
    private Label _animalSize;
    private Button _takeInButton;
    private Button _turnAwayButton;
    private ProgressBar _happinessBar;

    private string[] rarityColors = { "#000000", "#A15505", "#727D8E", "#BF9304", "#A42DCB" };
    public UIAnimalElement(VisualTreeAsset templateContainer, AnimalData animalData, RegisterManager registerManager)
    {
        this.registerManager = registerManager;
        this.animalData = animalData;
        // Instantiate visual element
        this._animalElement = templateContainer.Instantiate();
        _animalElement.name = templateContainer.name;
        // Get main UI elements from UI document
        _animalName = _animalElement.Q<Label>("NameLabel");
        _animalImage = _animalElement.Q<Image>("AnimalImage");
        _animalLevel = _animalElement.Q<Label>("AnimalLevel");
        _animalSize = _animalElement.Q<Label>("AnimalSize");
        _takeInButton = _animalElement.Q<Button>("TakeInButton");
        _turnAwayButton = _animalElement.Q<Button>("TurnAwayButton");
        _happinessBar = _animalElement.Q<ProgressBar>("SmallHappyBar");

        // Set button functions
        _takeInButton.RegisterCallback<ClickEvent>(OnClickTakeIn);
        _turnAwayButton.RegisterCallback<ClickEvent>(OnClickTurnAway);

        // Set the rest of the UI
        if(animalData != null){
            _animalName.text = animalData.animalName;
            _animalImage.image = Resources.Load<Texture2D>($"Textures/Animals/{animalData.typeID}Icon");
            _happinessBar.value = (float)animalData.happiness;
            _animalLevel.text = animalData.level.ToString();
            SetRarity(animalData.rarity);
            switch (registerManager.GetType(animalData.typeID).size)
            {
                case AnimalSize.Small:
                    _animalSize.text = "S";
                    break;
                case AnimalSize.Large:
                    _animalSize.text = "L";
                    break;
                default:
                    _animalSize.text = "M";
                    break;
            }
        }
        SetEnabledIfCanTakeIn(true);
    }

    public void SetEnabledIfCanTakeIn(bool isTakeable)
    {
        _takeInButton.SetEnabled(isTakeable);
    }

    /// <summary>
    /// Changes name color according to rarity
    /// </summary>
    /// <param name="rarity">rarity from 1 to 5. If too high or low will 
    /// be set to rarity 1 color (black)</param>
    public void SetRarity(int rarity)
    {
        if (1 <= rarity && rarity <= 5)
        {
            SetNameColor(rarityColors[rarity - 1]);
        }
        else
            SetNameColor(rarityColors[0]);
    }

    /// <summary>
    /// Set animal name color
    /// </summary>
    /// <param name="hex">A hex color code, for example #FFFFFF</param>
    public void SetNameColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color _color);
        _animalName.style.color = _color;
    }

    void OnClickTakeIn(ClickEvent evt)
    {
        registerManager.AnimalTakeInClicked(animalData);
        _animalElement.RemoveFromHierarchy();
        OnDisable();
    }
    void OnClickTurnAway(ClickEvent evt)
    {
        registerManager.AnimalTurnAwayClicked(animalData);
        _animalElement.RemoveFromHierarchy();
        OnDisable();
    }

    public void OnDisable()
    {
        _takeInButton.UnregisterCallback<ClickEvent>(OnClickTakeIn);
        _turnAwayButton.UnregisterCallback<ClickEvent>(OnClickTurnAway);
    }
}

