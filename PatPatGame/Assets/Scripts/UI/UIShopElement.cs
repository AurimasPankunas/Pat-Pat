using UnityEngine;
using UnityEngine.UIElements;

public class UIShopElement
{
    public ShopItem shopItem;
    private ShopManager shopManager;
    private PlayerBalance playerBalance;

    public TemplateContainer _shopElement;
    private Label _itemName;
    private Image _itemImage;
    private Button _buyButton;
    private Label _price;

    private Label _description;
    private Label _rarity;

    // Pictures for no image textures in items
    private Texture2D waterImg = Resources.Load<Texture2D>("Textures/Water");
    private Texture2D foodImg = Resources.Load<Texture2D>("Textures/Food");
    private Texture2D gloveImg = Resources.Load<Texture2D>("Textures/Glove");

    public UIShopElement(VisualTreeAsset templateContainer, ShopItem shopItem, ShopManager shopManager)
    {
        this.playerBalance = GameManager.Instance.playerBalance;
        this.shopManager = shopManager;
        this.shopItem = shopItem;
        // Instantiate visual element
        this._shopElement = templateContainer.Instantiate();
        _shopElement.name = templateContainer.name;
        // Get main UI elements from UI document
        _itemName = _shopElement.Q<Label>("ElementLabel");
        _itemImage = _shopElement.Q<Image>("ElementImage");
        _buyButton = _shopElement.Q<Button>("ElementBuyButton");
        _price = _shopElement.Q<Label>("ElementPriceLabel");

        // Set price and register button to a function
        _price.text = shopItem.price.ToString();
        _buyButton.RegisterCallback<ClickEvent>(OnClick);

        // Set the rest of the UI elements depending on item type
        switch (shopItem)
        {
            case FoodItem:
                FoodItem fitem = (FoodItem) shopItem;
                _itemName.text = fitem.name;
                if (fitem.image){
                    _itemImage.image = fitem.image;
                }
                else { _itemImage.image = foodImg; }
                break;
            case WaterItem:
                WaterItem witem = (WaterItem)shopItem;
                _itemName.text = witem.name;
                if (witem.image){
                    _itemImage.image = witem.image;
                }
                else { _itemImage.image = waterImg; }
                break;
            case GloveItem:
                GloveItem gitem = (GloveItem)shopItem;
                _rarity = _shopElement.Q<Label>("GloveRarity");
                _itemName.text = gitem.type;
                if (gitem.image){
                    _itemImage.image = gitem.image;
                }
                else { _itemImage.image = gloveImg;}
                // Image color tint of glove image vvv
                // Remove if removing imageTint from GloveItem class
                if(gitem.imageTint != null){
                    Color tint = new Color(gitem.imageTint.r, gitem.imageTint.g, gitem.imageTint.b);
                    _itemImage.tintColor = tint;
                }

                if (_rarity != null){
                    _rarity.text = gitem.rarity.ToString();
                    // If player has max rarity glove, hide button
                    if(gitem.rarity == playerBalance.gloveRarity){
                        _buyButton.style.visibility = Visibility.Hidden;
                    }
                }
                break;
            default:
                break;
        }

        // In case ShopItemElementDescription template would be used eventually
        //_description = _shopElement.Q<Label>("ElementDescription");
        SetEnabledIfBalanceIsEnough();
    }

    /// <summary>
    /// Sets shop element as enabled or disabled depending on
    /// player balance (amount of money that the player has)
    /// </summary>
    public void SetEnabledIfBalanceIsEnough()
    {
        if (playerBalance == null) return;
        VisualElement elementToDisable;
        if (_shopElement.name == "ShopItemElementLong") {
            elementToDisable = _buyButton;
        }
        else elementToDisable = _shopElement;
        if (playerBalance.money < shopItem.price)
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

    void OnClick(ClickEvent evt)
    {
        shopManager.ShopItemClicked(shopItem);
    }
    
    public void OnDisable()
    {
        _buyButton.UnregisterCallback<ClickEvent>(OnClick);
    }
}
