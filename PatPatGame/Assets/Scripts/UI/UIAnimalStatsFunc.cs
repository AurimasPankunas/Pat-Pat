using UnityEngine;
using UnityEngine.UIElements;

public class UIAnimalStatsFunc : MonoBehaviour
{
    private UIDocument _document;
    private Label _animalName;
    private VisualElement _containerStats;
    private VisualElement _containerEmpty;
    private Button _buttonSpotNotBought;
    private Label _buttonSpotNotBoughtPrice;
    private Button _buttonLevelUp;
    private Label _buttonLevelUpPrice;
    private ProgressBar _happyBar;
    private ProgressBar _waterBar;
    private ProgressBar _foodBar;
    private ProgressBar _bondBar;
    private Label _levelAmount;

    /// Might not need serialize fields when SpotManager 
    /// or something is implemented
    [SerializeField] private Animal animal;
    [SerializeField] private Spot spot;
    [SerializeField] private PlayerBalance playerBalance;
    [SerializeField] private ShopItemDatabase shopItemDatabase;
    private double levelUpPrice = 50;
    private double spotPrice = 50;
    private bool isBoughtSpot = false;

    private string[] rarityColors = {"#000000", "#A15505", "#727D8E", "#BF9304", "#A42DCB" };
    void Start()
    {
        // Gets the document on the gameObject that has the UI elements 
        _document = GetComponent<UIDocument>();

        // Grabbing UI elements from the document
        _containerStats = _document.rootVisualElement.Q<VisualElement>("StatsContainer");
        _animalName = _document.rootVisualElement.Q<Label>("NameLabel");
        _happyBar = _document.rootVisualElement.Q<ProgressBar>("HappyBar");
        _waterBar = _document.rootVisualElement.Q<ProgressBar>("WaterBar");
        _foodBar = _document.rootVisualElement.Q<ProgressBar>("FoodBar");
        _bondBar = _document.rootVisualElement.Q<ProgressBar>("BondBar");
        _levelAmount = _document.rootVisualElement.Q<Label>("LvlAmount");

        _containerEmpty = _document.rootVisualElement.Q<VisualElement>("SpotEmptyContainer");
        _buttonSpotNotBought = _document.rootVisualElement.Q<Button>("SpotBuyButton");
        _buttonSpotNotBoughtPrice = _document.rootVisualElement.Q<Label>("SpotPriceLabel");
        _buttonLevelUp = _document.rootVisualElement.Q<Button>("LevelUpButton");
        _buttonLevelUpPrice = _document.rootVisualElement.Q<Label>("LevelUpPriceLabel");

        // Setting methods to buttons
        _buttonSpotNotBought.RegisterCallback<ClickEvent>(OnClickBuySpot);
        _buttonLevelUp.RegisterCallback<ClickEvent>(OnClickLevelUp);

        spot.OnSpotAnimalChanged += HandleOnSpotAnimalChanged;
        HandleOnSpotAnimalChanged(spot.animal);
        playerBalance = GameManager.Instance.playerBalance;

        // Sets the initial visuals depending on if:
        // There is(n't) an animal or the spot has been bought

        if (animal != null){ 
            SetName(animal.data.animalName);
            SetLevel(animal.data.level);
            SetRarity(animal.data.rarity);
            levelUpPrice = shopItemDatabase.GetLevelUp(animal.data.rarity, animal.data.level).price;
            _buttonLevelUpPrice.text = levelUpPrice.ToString("N0");
        }
        else{
            _containerStats.style.display = DisplayStyle.None;
            _containerEmpty.style.display = DisplayStyle.Flex;
            if (!isBoughtSpot)
            {
                SetButtonEnabledIfBalanceIsEnough(_buttonSpotNotBought, spotPrice);
                ShowBuySpotUI();
            }
        }
    }

    void Update()
    {
        if(!isBoughtSpot)
            SetButtonEnabledIfBalanceIsEnough(_buttonSpotNotBought, spotPrice);
        if (animal != null){
            SetHappiness(animal.data.happiness);
            SetWater(animal.data.water);
            SetFood(animal.data.food);
            SetBond(animal.data.bond);
            if(animal.data.level < 30){
                SetButtonEnabledIfBalanceIsEnough(_buttonLevelUp, levelUpPrice);
            }
        }
    }

    /// <summary>
    /// Sets buy button as enabled or disabled depending on price and
    /// player balance (amount of money that the player has)
    /// </summary>
    public void SetButtonEnabledIfBalanceIsEnough(Button button, double price)
    {
        if (playerBalance == null){
            return;
        }
        VisualElement elementToDisable = button;
        if (playerBalance.money < price)
        {
            if (elementToDisable.enabledSelf)
            {
                elementToDisable.SetEnabled(false);
            }
        }
        else
        {
            if (!elementToDisable.enabledSelf)
            {
                elementToDisable.SetEnabled(true);
            }
        }
    }

    // public void SetAnimal(Animal animal)
    // {
    //     this.animal = animal;
    // }


    /// <summary>
    /// Set animal name
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        _animalName.text = name;
    }

    /// <summary>
    /// Set animal name color
    /// </summary>
    /// <param name="hex">A hex color code, for example #FFFFFF</param>
    public void SetNameColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex,out Color _color);
        _animalName.style.color = _color;
    }

    /// <summary>
    /// Set happiness value of the bar
    /// </summary>
    /// <param name="value">value from 0 to 1</param>
    public void SetHappiness(double value)
    {
        if (_happyBar.value == (float)value)
            return;
        _happyBar.value = (float)value;
    }

    /// <summary>
    /// Set water value of the bar
    /// </summary>
    /// <param name="value">value from 0 to 1</param>
    public void SetWater(double value)
    {
        if (_waterBar.value == (float)value)
            return;
        _waterBar.value = (float)value;
    }

    /// <summary>
    /// Set food value of the bar
    /// </summary>
    /// <param name="value">value from 0 to 1</param>
    public void SetFood(double value)
    {
        if (_foodBar.value == (float)value)
            return;
        _foodBar.value = (float)value;
    }

    /// <summary>
    /// Set bond value of the bar
    /// </summary>
    /// <param name="value">value from 0 to 4</param>
    public void SetBond(double value)
    {
        if (_bondBar.value == (float)value)
            return;
        _bondBar.value = (float)value;
    }

    /// <summary>
    /// Set level number
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(int level)
    {
        _levelAmount.text = level.ToString();
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

    public void ShowBuySpotUI()
    {
        _buttonSpotNotBought.style.display = DisplayStyle.Flex;
        _containerStats.style.display = DisplayStyle.None;
        _containerEmpty.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// If no animal is present shows empty spot
    /// </summary>
    public void ShowAnimalStatsUI()
    {
        _buttonSpotNotBought.style.display = DisplayStyle.None;
        if (animal == null){
            _containerStats.style.display = DisplayStyle.None;
            _containerEmpty.style.display = DisplayStyle.Flex;
        }
        else{
            _containerStats.style.display = DisplayStyle.Flex;
            _containerEmpty.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Sets the price of the spot
    /// </summary>
    /// <param name="price"></param>
    public void SetSpotPrice(double price)
    {
        spotPrice = price;
        _buttonSpotNotBoughtPrice.text = price.ToString("N0");
    }

    /// <summary>
    /// Set whether the spot has been bought or not
    /// </summary>
    /// <param name="isBought"></param>
    public void SetIsBoughtSpot(bool isBought)
    {
        if (isBought != isBoughtSpot){
            isBoughtSpot = isBought;

            // Revert UI if false
            if (!isBoughtSpot) {
                ShowBuySpotUI();
                return; 
            }

            ShowAnimalStatsUI();
        }
    }

    /// <summary>
    /// Set the animal to show
    /// </summary>
    /// <param name="animal"></param>
    public void SetAnimal(Animal animal)
    {
        this.animal = animal;
        if (animal != null)
        {
            SetName(animal.data.animalName);
            SetLevel(animal.data.level);
            SetRarity(animal.data.rarity);
            levelUpPrice = shopItemDatabase.GetLevelUp(animal.data.rarity, animal.data.level).price;
            _buttonLevelUpPrice.text = levelUpPrice.ToString("N0");
        }

        if (isBoughtSpot){
            ShowAnimalStatsUI();
        }
    }

    /// <summary>
    /// Set player balance so buy button(s) can enable/disable
    /// depending on player's balance
    /// </summary>
    /// <param name="playerBalance"></param>
    public void SetPlayerBalance(PlayerBalance playerBalance)
    {
        this.playerBalance = playerBalance;
    }

    /// <summary>
    /// Method to execute when the spot is being bought
    /// </summary>
    /// <param name="evt"></param>
    public void OnClickBuySpot(ClickEvent evt)
    {
        // Implement SpotManager.OnBuySpot() or something 
        // here instead of or along with the code below
        if (playerBalance == null){
            Debug.Log("PlayerBalance on UIAnimalStats is null, price will react to balance amount if it is set");
            SetIsBoughtSpot(true);
            return;
        }
        if (playerBalance.money >= spotPrice){
            playerBalance.SubtractMoney(spotPrice);
            SetIsBoughtSpot(true);
        }
    }

    public void OnClickLevelUp(ClickEvent evt)
    {
        if(playerBalance.money >= levelUpPrice)
        {
            playerBalance.SubtractMoney(levelUpPrice);
            animal.LevelUp();
            int animalLvl = animal.data.level;
            SetLevel(animalLvl);
            levelUpPrice = shopItemDatabase.GetLevelUp(animal.data.rarity,animalLvl).price;
            _buttonLevelUpPrice.text = levelUpPrice.ToString("N0");
        }
    }

    private void HandleOnSpotAnimalChanged(Animal animal)
    {
        SetAnimal(animal);
    }

    public void OnDisable()
    {
        _buttonSpotNotBought.UnregisterCallback<ClickEvent>(OnClickBuySpot);
    }
}
