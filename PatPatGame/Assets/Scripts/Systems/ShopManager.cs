using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] public PlayerBalance playerBalance;
    [SerializeField] private UIShopFunc shopUI;
    [SerializeField] private ShopItemDatabase shopItemDatabase;
    private List<ShopItem> shopList;

    void Start()
    {
        shopUI.SetShopManager(this);
        // Sets the initial category to be selected and
        // generates the shop items for it
        shopUI.SetSelectedCategory("Food");
        shopList = new List<ShopItem>(shopItemDatabase.foodPrices);
        shopUI.GenerateList(shopList);
    }

    // Update is called once per frame
    void Update()
    {
        shopUI.SetMoneyAmount(playerBalance.money);
    }

    /// <summary>
    /// Functions to execute when an item is clicked
    /// </summary>
    /// <param name="shopItem">Shop item that was clicked</param>
    public void ShopItemClicked(ShopItem shopItem)
    {
        if (playerBalance.money >= shopItem.price)
        {
            shopUI.SubtractMoneyAnimation(shopItem.price);
            playerBalance.SubtractMoney(shopItem.price);
            switch (shopItem)
            {
                case FoodItem:
                    FoodItem fitem = (FoodItem)shopItem;
                    Debug.Log(fitem.name);
                    // Do thing for food item
                    break;
                case WaterItem:
                    WaterItem witem = (WaterItem)shopItem;
                    Debug.Log(witem.name);
                    // Do thing for water item
                    break;
                case GloveItem:
                    GloveItem gitem = (GloveItem)shopItem;
                    Debug.Log(gitem.type);
                    // Do thing for glove item

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
