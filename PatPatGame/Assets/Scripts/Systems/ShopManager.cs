using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private PlayerBalance playerBalance;
    private UIShopFunc shopUI;
    [SerializeField] private ConveyorBelt conveyorBelt;
    [SerializeField] private ShopItemDatabase shopItemDatabase;
    private List<ShopItem> shopList;

    public float buyCooldownTime = 0.5f;
    private float buyCooldown = 0.5f;

    void Start()
    {
        shopUI = GetComponent<UIShopFunc>();
        shopUI.SetShopManager(this);
        if(!(playerBalance = GameManager.Instance.playerBalance)){
            Debug.Log("GameManager or PlayerBalance in GameManager is missing");
        }
        if (shopItemDatabase == null) {
            Debug.Log("ShopItemDatabase is missing");
        }
        if (conveyorBelt == null){
            Debug.Log("ConveyorBelt is missing");
        }
        // Gets in the way of testing but good to have in build?
        //playerBalance.OnMoneyChanged += shopUI.SetMoneyAmount;
        shopUI.SetMoneyAmount(playerBalance.money);
        // Sets the initial category to be selected and
        // generates the shop items for it
        shopUI.SetSelectedCategory("Food");
        shopList = new List<ShopItem>(shopItemDatabase.foodPrices);
        shopUI.GenerateList(shopList);
    }

    // Update is called once per frame
    void Update()
    {
        if (buyCooldown > 0){
            buyCooldown -= Time.deltaTime;
        }
        if (playerBalance)
        {
            shopUI.SetMoneyAmount(playerBalance.money);
        }
    }

    /// <summary>
    /// Functions to execute when an item is clicked
    /// </summary>
    /// <param name="shopItem">Shop item that was clicked</param>
    public void ShopItemClicked(ShopItem shopItem)
    {
        if (playerBalance == null) return;
        if (playerBalance.money >= shopItem.price && buyCooldown <= 0)
        {
            buyCooldown = buyCooldownTime;
            shopUI.SubtractMoneyAnimation(shopItem.price);
            playerBalance.SubtractMoney(shopItem.price);
            switch (shopItem)
            {
                case FoodItem:
                    FoodItem fitem = (FoodItem)shopItem;
                    //Debug.Log(fitem.name);
                    // Do thing for food item
                    if (fitem.obj != null){
                        conveyorBelt.SpawnItemOnBelt(Instantiate(fitem.obj));
                    }
                    else Debug.Log($"GameObject not assigned to FoodItem {fitem.name}");
                    break;
                case WaterItem:
                    WaterItem witem = (WaterItem)shopItem;
                    //Debug.Log(witem.name);
                    // Do thing for water item
                    if (witem.obj != null){
                        conveyorBelt.SpawnItemOnBelt(Instantiate(witem.obj));
                    }
                    else Debug.Log($"GameObject not assigned to WaterItem {witem.name}");
                        break;
                case GloveItem:
                    GloveItem gitem = (GloveItem)shopItem;
                    // Do thing for glove item
                    playerBalance.IncrementGloveRarity();
                    // Update Glove upgrade attributes in UI
                    shopUI.ClearList();
                    SetGloveUpgradeElement();
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the list of items to be generated when a category is clicked
    /// </summary>
    /// <param name="categoryButtonName">The name of the VisualElement 
    /// in the UI Document</param>
    public void CategoryClicked(string categoryButtonName)
    {
        switch (categoryButtonName)
        {
            case "CategoryFood":
                shopList = new List<ShopItem>(shopItemDatabase.foodPrices);
                shopUI.ClearList();
                shopUI.GenerateList(shopList);
                break;
            case "CategoryWater":
                shopList = new List<ShopItem>(shopItemDatabase.waterPrices);
                shopUI.ClearList();
                shopUI.GenerateList(shopList);
                break;
            case "CategoryGloves":
                shopUI.ClearList();
                SetGloveUpgradeElement();
                    //shopList = new List<ShopItem>(shopItemDatabase.glovePrices);
                    //shopUI.GenerateList(shopList);
                break;
        }
    }

    /// <summary>
    /// Show next glove upgrade. If glove rarity is max (5), show current glove
    /// without Upgrade button.
    /// </summary>
    public void SetGloveUpgradeElement()
    {
        if (playerBalance == null) return;
        if (playerBalance.gloveRarity < 5)
        {
            shopUI.AddToList(shopItemDatabase.GetGlove(playerBalance.gloveRarity + 1));
        }
        else
        {
            shopUI.AddToList(shopItemDatabase.GetGlove(playerBalance.gloveRarity));
        }
    }
}
