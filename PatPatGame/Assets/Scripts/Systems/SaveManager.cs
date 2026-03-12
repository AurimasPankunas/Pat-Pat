using System;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private int autosaveTimeSeconds;
    private SaveData saveData;
    [SerializeField] private SaveDummy saveDummy; // for testing
    [SerializeField] private TimeKeeper timeKeeper;
    [SerializeField] private PlayerBalance playerBalance;
    [SerializeField] private Animal[] animals;



    //---------------------------------------------------------------------------
    // Helper struct and methods for save/load integration of specific classes
    //---------------------------------------------------------------------------

    // Populate this with save data structs
    [System.Serializable]
    public struct SaveData 
    {
        public SaveDummyData saveDummyData; // for testing
        public TimeKeeperData timeKeeperData;
        public PlayerBalanceData playerBalanceData;
        public AnimalData[] animals;
    }

    // Use this to call the load method(s) for every class instance
    private void HandleLoadData()
    {
        saveDummy?.Load(saveData.saveDummyData); // for testing
        timeKeeper.Load(saveData.timeKeeperData);
        if (saveData.animals != null)
        {
            for (int i = 0; i < animals.Length && i < saveData.animals.Length; i++)
            {
                animals[i].Load(saveData.animals[i]);
            }
        }
        playerBalance.Load(saveData.playerBalanceData, timeKeeper.GetHoursSinceLastSave());
    }

    // Use this to call the save method for every class instance
    public void HandleSaveData() 
    {
        saveData.saveDummyData = saveDummy?.Save() ?? new SaveDummyData(); // for testing
        saveData.timeKeeperData = timeKeeper.Save();
        saveData.playerBalanceData = playerBalance.Save();

        saveData.animals = new AnimalData[animals.Length];

        for(int i = 0; i < animals.Length; i++) 
        {
            saveData.animals[i] = animals[i].Save();
        }
    }


    //---------------------------------------------------------------------------
    // Save Manager internal logic
    //---------------------------------------------------------------------------

    public static string SaveFileName() 
    {
        return Application.persistentDataPath + "/save.json";
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
        animals = FindObjectsByType<Animal>(FindObjectsSortMode.None);

        if (File.Exists(SaveFileName()))
            Load();
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
