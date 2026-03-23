using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIShopFunc : MonoBehaviour
{
    private ShopManager shopManager;

    private UIDocument _document;
    private Label _moneyAmount;
    private Button[] _categories;
    public Button _selectedCategory;
    private VisualElement _ListContainer;

    private float lastAnimationTime = 0f;
    public float moneySpentCooldown = 0.15f;

    [SerializeField] private VisualTreeAsset ShopElement;
    [SerializeField] private VisualTreeAsset ShopElementDescription;
    [SerializeField] private VisualTreeAsset ShopElementLong;

    //private List<ShopItem> shopList;
    private List<UIShopElement> shopElements;
    void Awake()
    {
        // Gets the document on the gameObject that has the UI elements
        _document = GetComponent<UIDocument>();

        // Grabbing UI elements from the document
        _moneyAmount = _document.rootVisualElement.Q<Label>("MoneyAmount");
        //_moneySpentAnim = _document.rootVisualElement.Q<Label>("MoneySpent");
        _categories = new Button[3];
        _categories[0] = _document.rootVisualElement.Q<Button>("CategoryFood");
        _categories[1] = _document.rootVisualElement.Q<Button>("CategoryWater");
        _categories[2] = _document.rootVisualElement.Q<Button>("CategoryGloves");
        _ListContainer = _document.rootVisualElement.Q<VisualElement>("ListContainer");

        // Setting functions to category buttons
        _categories[0].RegisterCallback<ClickEvent>(OnCategoryClick);
        _categories[1].RegisterCallback<ClickEvent>(OnCategoryClick);
        _categories[2].RegisterCallback<ClickEvent>(OnCategoryClick);

        shopElements = new List<UIShopElement>();
        /*shopList = new List<ShopItem>();
        shopList.Add(new FoodItem { name = "food" });
        shopList.Add(new WaterItem { name = "water" });
        shopList.Add(new GloveItem { type = "glove",rarity=1 });
        shopElements = new List<UIShopElement>();
        GenerateList(shopList);*/
    }


    /// <summary>
    /// Set selected category.
    /// </summary>
    /// <param name="i">Food,
    /// Water,
    /// Gloves.
    /// Any other string will set the selected
    /// category to Food</param>
    public void SetSelectedCategory(String category)
    {
        if (_selectedCategory != null) {
            _selectedCategory.RemoveFromClassList("CategoryButtonSelected");
            _selectedCategory.pickingMode = PickingMode.Position;
        }
        switch (category)
        {
            case "Water":
                _selectedCategory = _categories[1];
                break;
            case "Gloves":
                _selectedCategory = _categories[2];
                break;
            default:
                _selectedCategory = _categories[0];
                break;
        }
        _selectedCategory.AddToClassList("CategoryButtonSelected");
        _selectedCategory.pickingMode = PickingMode.Ignore;

    }

    /// <summary>
    /// Sets the money amount in the UI
    /// </summary>
    /// <param name="value"></param>
    public void SetMoneyAmount(double value)
    {
        string price = value.ToString("N") + "$";
        if (_moneyAmount.text != price)
        { _moneyAmount.text = price; 
            foreach(UIShopElement item in shopElements)
            {
                item.SetEnabledIfBalanceIsEnough();
            }
        }
    }

    /// <summary>
    /// Adds an item to list. Adding a Glove currently gives it
    /// a big upgrade glove UI element
    /// </summary>
    /// <param name="shopItem"></param>
    public void AddToList(ShopItem shopItem)
    {
        if (shopItem != null)
        {
            UIShopElement element;
            switch (shopItem)
            {
                case GloveItem:
                    element = new UIShopElement(ShopElementLong, shopItem, shopManager);
                    break;
                default:
                    element = new UIShopElement(ShopElement, shopItem, shopManager);
                    break;
            }
            shopElements.Add(element);
            _ListContainer.Add(element._shopElement);
        }
    }

    /// <summary>
    /// Generates a list (grid) of shop items in the UI from a list
    /// </summary>
    /// <param name="shopItem"></param>
    public void GenerateList(List<ShopItem> shopItem)
    {
        if (shopItem.Count == 0) return;
        //shopList = shopItem;
        foreach (ShopItem item in shopItem)
        {
            UIShopElement element = new UIShopElement(ShopElement, item, shopManager);
            shopElements.Add(element);
            _ListContainer.Add(element._shopElement);
        }

    }
    /// <summary>
    /// Clears out the shop item list
    /// </summary>
    public void ClearList()
    {
        foreach (UIShopElement item in shopElements)
        {
            _ListContainer.RemoveAt(0);
            item.OnDisable();
        }
        shopElements.Clear();
    }

    /// <summary>
    /// If category button is clicked, sets it as selected: white background style
    /// and makes the button unclickable. If another button is clicked other than
    /// the currently selected one, sets it as selected
    /// </summary>
    /// <param name="evt"></param>
    private void OnCategoryClick(ClickEvent evt)
    {
        Button button = evt.target as Button;
        // Unselect the last selected button and set the new one as selected
        _selectedCategory.RemoveFromClassList("CategoryButtonSelected");
        _selectedCategory.pickingMode = PickingMode.Position;
        button.AddToClassList("CategoryButtonSelected");
        button.pickingMode = PickingMode.Ignore;
        _selectedCategory = button;
        shopManager.CategoryClicked(button.name);
    }

    public void SubtractMoneyAnimation(double price)
    {
        if (Time.time < lastAnimationTime + moneySpentCooldown)
        {
            return;
        }
        Label _moneySpentAnim = new Label("-" + price.ToString("N") + "$");
        _moneySpentAnim.AddToClassList("MoneySpent");
        _moneyAmount.Add(_moneySpentAnim);
        // So that MoneySpent is added before MoneySpentAnimation is
        _moneySpentAnim.schedule.Execute(() => {
            _moneySpentAnim.AddToClassList("MoneySpentAnimation");
        }).ExecuteLater(10);

        _moneySpentAnim.schedule.Execute(() => {
            _moneySpentAnim.RemoveFromHierarchy();
        }).ExecuteLater(700);
        lastAnimationTime = Time.time;
    }

    public void OnDisable()
    {
        foreach(Button category in _categories)
        {
            category.UnregisterCallback<ClickEvent>(OnCategoryClick);
        }
        if (shopElements.Count != 0){
            foreach (UIShopElement item in shopElements)
            {
                item.OnDisable();
                _ListContainer.RemoveAt(0);
            }
        }
    }

    public void SetShopManager(ShopManager shopManager)
    {
        this.shopManager = shopManager;
    }
}
