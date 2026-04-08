using System;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private int autosaveTimeSeconds;
    private static SaveData saveData;
    // [SerializeField] private TimeKeeper timeKeeper;
    // [SerializeField] private PlayerBalance playerBalance;
    // private AnimalManager animalManager;

    //---------------------------------------------------------------------------
    // Helper struct and methods for save/load integration of specific classes
    //---------------------------------------------------------------------------

    // Populate this with save data structs
    [System.Serializable]
    public struct SaveData 
    {
        public TimeKeeperData timeKeeperData;
        public PlayerBalanceData playerBalanceData;
        public AnimalManagerSaveData animalManagerData;
    }

    // Use this to call the load method(s) for every class instance
    private void HandleLoadData()
    {
        GameManager.Instance.timeKeeper.Load(saveData.timeKeeperData);
        GameManager.Instance.animalManager.Load(saveData.animalManagerData);
        GameManager.Instance.playerBalance.Load(saveData.playerBalanceData, GameManager.Instance.timeKeeper.GetHoursSinceLastSave());
    }

    // Use this to call the save method for every class instance
    public void HandleSaveData() 
    {
        saveData.timeKeeperData = GameManager.Instance.timeKeeper.Save();
        saveData.animalManagerData = GameManager.Instance.animalManager.Save();
        saveData.playerBalanceData = GameManager.Instance.playerBalance.Save();
    }


    //---------------------------------------------------------------------------
    // Save Manager internal logic
    //---------------------------------------------------------------------------

    public static string SaveFileName() 
    {
        return Application.persistentDataPath + "/save.json";
    }

    public static bool SaveFileExists()
    {
        return File.Exists(SaveFileName());
    }
    public void Save()
    {
        HandleSaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SaveFileName(), json);
    }

    public void Load()
    {
        if (File.Exists(SaveFileName()))
        {
            string saveContent = File.ReadAllText(SaveFileName());
            saveData = JsonUtility.FromJson<SaveData>(saveContent);
            HandleLoadData();
        }
        else
        {
            Debug.LogWarning(SaveFileName() + "not found");
        }
    }

    public void DeleteSave() 
    {
        if (File.Exists(SaveFileName()))
            File.Delete(SaveFileName());
    }

    void Awake() 
    {
        // if (File.Exists(SaveFileName()))
        // {
        //     Load();
        // }
        InvokeRepeating(nameof(Save), autosaveTimeSeconds, autosaveTimeSeconds);
    }

    void OnApplicationQuit()
    {
        Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            Save();
    }
}
