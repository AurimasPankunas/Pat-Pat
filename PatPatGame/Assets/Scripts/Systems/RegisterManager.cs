using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RegisterManager : MonoBehaviour
{
    private AnimalRegister animalRegister;
    private AnimalManager animalManager;
    private PlayerBalance playerBalance;
    private UIRegisterFunc registerUI;

    [SerializeField]
    private ShopItemDatabase shopItemDatabase;

    [SerializeField]
    private Transform miniAnimalSpawnPoint;

    [SerializeField]
    private AnimatedChest chest;

    public float buyCooldownTime = 0.3f;
    private float buyCooldown = 0.3f;

    void Start()
    {
        registerUI = GetComponent<UIRegisterFunc>();
        registerUI.SetRegisterManager(this);
        animalRegister = GameManager.Instance.animalRegister;
        animalManager = GameManager.Instance.animalManager;
        playerBalance = GameManager.Instance.playerBalance;
        if (shopItemDatabase == null)
        {
            Debug.Log("ShopItemDatabase is missing");
        }
        else
        {
            registerUI.SetShopDatabase(shopItemDatabase);
            if (playerBalance.chestLevel > 0)
                registerUI.SetChest(playerBalance.chestLevel);
            else
                registerUI.SetChest(1);
        }
        animalRegister.OnRegisterAnimalChanged += HandleRegisterAnimalChanged;
        // Gets in the way of testing but good to have in build?
        //playerBalance.OnLikesChanged += registerUI.SetLikes;
        registerUI.SetLikes(playerBalance.likes);
        registerUI.SetAnimalsAmount(
            animalRegister.registerAnimals.Count,
            animalRegister.maxRegisterAnimals
        );

        List<AnimalData> animalList = animalRegister.registerAnimals;
        if (animalList != null)
        {
            registerUI.GenerateList(animalList);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buyCooldown > 0)
        {
            buyCooldown -= Time.deltaTime;
        }

        if (playerBalance)
        {
            registerUI.SetLikes(playerBalance.likes);
        }
    }

    public AnimalType GetType(string typeID)
    {
        return animalManager.GetType(typeID);
    }

    private void HandleRegisterAnimalChanged()
    {
        registerUI.GenerateList(animalRegister.registerAnimals);
        registerUI.SetAnimalsAmount(
            animalRegister.registerAnimals.Count,
            animalRegister.maxRegisterAnimals
        );
    }

    /// <summary>
    /// Function to execute when an animal is taken in
    /// </summary>
    /// <param name="animal">Animal that was clicked</param>
    public void AnimalTakeInClicked(AnimalData animal, TemplateContainer _animalElement)
    {
        Debug.Log("Take in");
        animalRegister.TakeInCheckAndRemove(animal, miniAnimalSpawnPoint, _animalElement);
    }

    /// <summary>
    /// Function to execute when an animal is turned away
    /// </summary>
    /// <param name="animal">Animal that was clicked</param>
    public void AnimalTurnAwayClicked(AnimalData animal)
    {
        Debug.Log("Turn away");
        animalRegister.RemoveAnimal(animal, null);
    }

    public void OnChestBuyClicked()
    {
        int openCost = shopItemDatabase.chestOpenCost;
        if (playerBalance.likes >= openCost && buyCooldown <= 0)
        {
            if (animalRegister.registerAnimals.Count >= animalRegister.maxRegisterAnimals)
                return;
            registerUI.SubtractLikesAnimation(openCost);
            playerBalance.SubtractLikes(openCost);
            if (playerBalance.chestLevel < 1)
                playerBalance.IncrementChestLevel();
            int chestLvl = playerBalance.chestLevel;
            AnimalData animalData = animalRegister.CreateChestAnimal();
            chest.OpenChest(animalData);
            playerBalance.AddChestsOpened(1);
            registerUI.SetOpenedChests(chestLvl);
            int toLevelUp = shopItemDatabase.chestOpensToLevelUp[chestLvl - 1];
            if (playerBalance.levelChestsOpened >= toLevelUp)
            {
                playerBalance.SubtractChestsOpened(toLevelUp);
                playerBalance.IncrementChestLevel();
                registerUI.SetChest(playerBalance.chestLevel);
            }
            buyCooldown = buyCooldownTime;
        }
    }

    private void OnDisable()
    {
        animalRegister.OnRegisterAnimalChanged -= HandleRegisterAnimalChanged;
    }
}
