using UnityEngine;

public class Food : MonoBehaviour, ISellable
{
    public int foodItemID;

    [Header("Selling")]
    [Range(0f, 1f)]
    [SerializeField] private float sellPercent = 0.5f;

    public double SellValue
    {
        get
        {
            var item = GameManager.Instance.shopItemDatabase.GetFood(foodItemID);
            if(item != null)
            {
                return System.Math.Round(item.price * sellPercent, 2);
            }
            else
            {
                return 0;
            }
        }
    }

    public string PriceLabel => $"${SellValue:F2}";
}
