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

    //---------------------------------------------------------------------------
    // Helper struct and methods for save/load integration of specific classes
    //---------------------------------------------------------------------------

    // Populate this with save data structs
    [System.Serializable]
    public struct SaveData 
    {
        public SaveDummyData saveDummyData; // for testing
    }

    // Use this to call the load method(s) for every class instance
    private void HandleLoadData() 
    {
        saveDummy?.Load(saveData.saveDummyData); // for testing
    }

    // Use this to call the save method for every class instance
    public void HandleSaveData() 
    {
        saveData.saveDummyData = saveDummy?.Save() ?? new SaveDummyData(); // for testing
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
        DontDestroyOnLoad(gameObject);
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
