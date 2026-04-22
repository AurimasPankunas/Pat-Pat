using UnityEngine;

public class Animal_Stat_Increase : MonoBehaviour
{
    private string Message;
    private Animal animal;
    private AnimalGraphicalFeedback graphicalFeedback;
    [SerializeField] private ShopItemDatabase shopItemDatabase;
    private FoodItem foodData;
    private WaterItem waterData;
    
    [SerializeField] private float floatingTextCooldown = 0.5f;
    private float lastFloatingTextTime = -1f;

    void Start()
    {
        animal = GetComponent<Animal>();
        graphicalFeedback = GetComponent<AnimalGraphicalFeedback>();
    }

    private void SpawnFloatingTextWithCooldown(string message)
    {
        if (Time.time >= lastFloatingTextTime + floatingTextCooldown)
        {
            StartCoroutine(graphicalFeedback.SpawnFloatingText(message, false));
            lastFloatingTextTime = Time.time;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collider entered: {other.gameObject.name}, Tag: {other.tag}");
        
        if(other.CompareTag("Food"))
        {
            Food food = other.GetComponentInParent<Food>();
            if(food != null)
            {
                foodData = shopItemDatabase.GetFood(food.foodItemID);
                if(foodData.foodAmount + animal.data.food >= 1.0) {
                    Message = "Not hungry!";
                } else if(foodData.rarity == animal.data.rarity)
                {
                    animal.UpdateFood(foodData.foodAmount);
                    animal.GetComponent<Animator>().SetTrigger("Eat");
                    Message = null;
                    Destroy(other.transform.root.gameObject);
                } else
                {
                    Message = "Wrong rarity food!";
                }
                SpawnFloatingTextWithCooldown(Message);
            }
        } else if(other.CompareTag("Drink"))
        {
            Drink drink = other.GetComponentInParent<Drink>();
            if(drink != null)
            {
                waterData = shopItemDatabase.GetWater(drink.drinkItemID);
                if(waterData.waterAmount + animal.data.water >= 1.0) {
                    Message = "Not thirsty!";
                } else if(waterData.rarity == animal.data.rarity)
                {
                    animal.UpdateWater(waterData.waterAmount);
                    animal.GetComponent<Animator>().SetTrigger("Eat");
                    Message = null;
                    Destroy(other.transform.root.gameObject);
                } else
                {
                    Message = "Wrong rarity drink!";
                }
                SpawnFloatingTextWithCooldown(Message);
            }
        } else
        {
            Debug.Log("Something that wasn't food or a hand has entered the animal collider");
        }
    }
}
