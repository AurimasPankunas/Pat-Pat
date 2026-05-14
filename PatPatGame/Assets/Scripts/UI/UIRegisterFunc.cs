using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIRegisterFunc : MonoBehaviour
{
    private RegisterManager registerManager;

    private UIDocument _document;
    private Label _likesAmount;
    private Label _animalsAmount;
    private Label _levelUpPrice;
    private Button _buyChest;
    private Label _buyChestPrice;

    private Label _levelUpLabel;
    private Image _chestTrimImage;

    private Label _commonChance;
    private Label _uncommonChance;
    private Label _rareChance;
    private Label _epicChance;
    private Label _legendaryChance;

    private VisualElement _ListContainer;
    private VisualTreeAsset AnimalElement;

    private int BuyPrice;
    private bool isMaxAnimals = false;
    private ShopItemDatabase shopItemDatabase;

    private List<UIAnimalElement> registerUIAnimals;
    private string[] rarityColors = { "#460505", "#A15505", "#727D8E", "#BF9304", "#A42DCB" };

    void Awake()
    {
        // Gets the document on the gameObject that has the UI elements
        _document = GetComponent<UIDocument>();

        // Grabbing UI elements from the document
        _likesAmount = _document.rootVisualElement.Q<Label>("LikesAmount");
        _animalsAmount = _document.rootVisualElement.Q<Label>("AnimalsAmount");

        _levelUpPrice = _document.rootVisualElement.Q<Label>("LevelUpLabel");
        _buyChest = _document.rootVisualElement.Q<Button>("BuyButton");
        _buyChestPrice = _document.rootVisualElement.Q<Label>("BuyPriceLabel");

        _chestTrimImage = _document.rootVisualElement.Q<Image>("ChestTrimImage");
        _levelUpLabel = _document.rootVisualElement.Q<Label>("ChestsToOpen");

        _commonChance = _document.rootVisualElement.Q<Label>("CommonChance");
        _uncommonChance = _document.rootVisualElement.Q<Label>("UncommonChance");
        _rareChance = _document.rootVisualElement.Q<Label>("RareChance");
        _epicChance = _document.rootVisualElement.Q<Label>("EpicChance");
        _legendaryChance = _document.rootVisualElement.Q<Label>("LegendaryChance");

        _ListContainer = _document.rootVisualElement.Q<VisualElement>("ListContainer");
        AnimalElement = Resources.Load<VisualTreeAsset>("RegisterAnimalElement");
        registerUIAnimals = new List<UIAnimalElement>();

        // Setting functions to buttons
        _buyChest.RegisterCallback<ClickEvent>(OnBuyClick);
    }

    /// <summary>
    /// Sets chest values
    /// </summary>
    /// <param name="chestLvl"></param>
    public void SetChest(int chestLvl)
    {
        int levelUpPrice =
            shopItemDatabase.chestOpensToLevelUp[chestLvl - 1]
            - GameManager.Instance.playerBalance.levelChestsOpened;
        BuyPrice = shopItemDatabase.chestOpenCost;
        if (levelUpPrice > 0)
        {
            _levelUpPrice.text = $"{levelUpPrice}";
        }
        else
        {
            _levelUpPrice.style.visibility = Visibility.Hidden;
            _levelUpLabel.style.visibility = Visibility.Hidden;
        }
        _buyChestPrice.text = $"{BuyPrice}";
        ColorUtility.TryParseHtmlString(rarityColors[chestLvl - 1], out Color _color);
        _chestTrimImage.tintColor = _color;

        float[] probabilities = shopItemDatabase.chestRarityProbabilities[chestLvl - 1];
        _commonChance.text = probabilities[0].ToString("P");
        _uncommonChance.text = probabilities[1].ToString("P");
        _rareChance.text = probabilities[2].ToString("P");
        _epicChance.text = probabilities[3].ToString("P");
        _legendaryChance.text = probabilities[4].ToString("P");
    }

    public void SetOpenedChests(int chestLvl)
    {
        int levelUpPrice =
            shopItemDatabase.chestOpensToLevelUp[chestLvl - 1]
            - GameManager.Instance.playerBalance.levelChestsOpened;
        _levelUpPrice.text = $"{levelUpPrice}";
    }

    /// <summary>
    /// Generates a list animals in the UI from a list
    /// </summary>
    /// <param name="animals"></param>
    public void GenerateList(List<AnimalData> animals)
    {
        if (animals == null)
            return;
        ClearList();
        foreach (AnimalData animal in animals)
        {
            UIAnimalElement element = new UIAnimalElement(AnimalElement, animal, registerManager);
            registerUIAnimals.Add(element);
            _ListContainer.Add(element._animalElement);
        }
    }

    /// <summary>
    /// Clears out the animal list
    /// </summary>
    public void ClearList()
    {
        foreach (UIAnimalElement animal in registerUIAnimals)
        {
            animal.RemoveAnimalFromHierarchy();
            animal.OnDisable();
        }
        registerUIAnimals.Clear();
    }

    public void OnBuyClick(ClickEvent evt)
    {
        registerManager.OnChestBuyClicked();
    }

    public void SetLikes(int likes)
    {
        string price = likes.ToString();
        if (_likesAmount.text != price)
        {
            _likesAmount.text = price;
            UpdateBuyButton();
        }
    }

    public void SetAnimalsAmount(int amount, int max)
    {
        _animalsAmount.text = $"{amount}/{max}";
        if (amount >= max)
        {
            isMaxAnimals = true;
        }
        else
            isMaxAnimals = false;
        UpdateBuyButton();
    }

    public void UpdateBuyButton()
    {
        if (GameManager.Instance.playerBalance.likes >= BuyPrice && !isMaxAnimals)
            _buyChest.SetEnabled(true);
        else
            _buyChest.SetEnabled(false);
    }

    public void SubtractLikesAnimation(int price)
    {
        Label _likesSpentAnim = new Label("-" + price.ToString());
        _likesSpentAnim.AddToClassList("MoneySpent");
        _likesAmount.Add(_likesSpentAnim);
        // So that MoneySpent is added before MoneySpentAnimation is
        _likesSpentAnim
            .schedule.Execute(() =>
            {
                _likesSpentAnim.AddToClassList("MoneySpentAnimation");
            })
            .ExecuteLater(10);

        _likesSpentAnim
            .schedule.Execute(() =>
            {
                _likesSpentAnim.RemoveFromHierarchy();
            })
            .ExecuteLater(700);
    }

    public void SetRegisterManager(RegisterManager registerManager)
    {
        this.registerManager = registerManager;
    }

    public void SetShopDatabase(ShopItemDatabase shopItemDatabase)
    {
        this.shopItemDatabase = shopItemDatabase;
    }
}
