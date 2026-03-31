using UnityEngine;
using UnityEngine.UIElements;

public class UIAnimalStatsFunc : MonoBehaviour
{
    private UIDocument _document;
    private Label _animalName;
    private ProgressBar _happyBar;
    private ProgressBar _waterBar;
    private ProgressBar _foodBar;
    private ProgressBar _bondBar;
    private Label _levelAmount;
    [SerializeField] private Animal animal;

    private string[] rarityColors = {"#000000", "#A15505", "#727D8E", "#BF9304", "#A42DCB" };
    void Start()
    {
        // Gets the document on the gameObject that has the UI elements 
        _document = GetComponent<UIDocument>();
        
        // Grabbing UI elements from the document
        _animalName = _document.rootVisualElement.Q<Label>("NameLabel");
        _happyBar = _document.rootVisualElement.Q<ProgressBar>("HappyBar");
        _waterBar = _document.rootVisualElement.Q<ProgressBar>("WaterBar");
        _foodBar = _document.rootVisualElement.Q<ProgressBar>("FoodBar");
        _bondBar = _document.rootVisualElement.Q<ProgressBar>("BondBar");
        _levelAmount = _document.rootVisualElement.Q<Label>("LvlAmount");
    }

    void Update()
    {
        if (animal != null)
        {
            AnimalData data = animal.data;
            SetName(data.animalName);
            SetHappiness(data.happiness);
            SetWater(data.water);
            SetFood(data.food);
            SetBond(data.bond);
            SetLevel(data.level);
            SetRarity(data.rarity);
        }
        
    }


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
        _happyBar.value = (float)value;
    }

    /// <summary>
    /// Set water value of the bar
    /// </summary>
    /// <param name="value">value from 0 to 1</param>
    public void SetWater(double value)
    {
        _waterBar.value = (float)value;
    }

    /// <summary>
    /// Set food value of the bar
    /// </summary>
    /// <param name="value">value from 0 to 1</param>
    public void SetFood(double value)
    {
        _foodBar.value = (float)value;
    }

    /// <summary>
    /// Set bond value of the bar
    /// </summary>
    /// <param name="value">value from 0 to 4</param>
    public void SetBond(double value)
    {
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
}
