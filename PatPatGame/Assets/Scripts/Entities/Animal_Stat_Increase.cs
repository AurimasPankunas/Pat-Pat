using UnityEngine;

public class Animal_Stat_Increase : MonoBehaviour
{
    private string rarityMessage;
    private Animal animal;
    [SerializeField] private ShopItemDatabase shopItemDatabase;
    private FoodItem foodData;
    private WaterItem waterData;

    void Start()
    {
        animal = GetComponent<Animal>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collider entered: {other.gameObject.name}, Tag: {other.tag}");
        
        if(other.CompareTag("Hand"))
        {
            return;
        }
        if(other.CompareTag("Food"))
        {
            Food food = other.GetComponent<Food>();
            if(food != null)
            {
                if(food.foodItemID == animal.data.rarity)
                {
                    foodData = shopItemDatabase.GetFood(food.foodItemID);
                    animal.UpdateFood(foodData.foodAmount);
                    Destroy(other.gameObject);
                }
                rarityMessage = "Wrong rarity food!";
            }
        } else if(other.CompareTag("Drink"))
        {
            Drink drink = other.GetComponent<Drink>();
            if(drink != null)
            {
                if(drink.drinkItemID == animal.data.rarity)
                {
                    waterData = shopItemDatabase.GetWater(drink.drinkItemID);
                    animal.UpdateWater(waterData.waterAmount);
                    Destroy(other.gameObject);
                }
                rarityMessage = "Wrong rarity drink!";
            }
        } else
        {
            Debug.Log("Something that wasn't food or a hand has entered the animal collider");
        }
    }
}
