using UnityEngine;

public class SaveDummy : MonoBehaviour
{
    // Random movement, unrelated to saving itself
    void Update()
    {
        transform.position += new Vector3(Mathf.Cos(Time.realtimeSinceStartup), 0, Mathf.Sin(Time.realtimeSinceStartup)) / 100;
        transform.Rotate(new Vector3(0, 180, 0) * Time.deltaTime, Space.Self);
    }

    public SaveDummyData Save()
    {
        SaveDummyData data;
        data.myPosition = transform.position;
        data.myRotation = transform.rotation;
        Debug.Log(string.Format("SaveDummy data saved: Position {0}; Rotation {1}", data.myPosition, data.myRotation));
        return data;
    }

    public void Load(SaveDummyData data)
    {
        transform.position = data.myPosition;
        transform.rotation = data.myRotation;
        Debug.Log(string.Format("SaveDummy data loaded: Position {0}; Rotation {1}", data.myPosition, data.myRotation));
    }
}

//---------------------------------------------------------------------------
// Save data struct - only include data that is neccesary for saving/loading
//
// IMPORTANT - [System.Serializable] is required for reading/writing data
// to a save file
//---------------------------------------------------------------------------

[System.Serializable]
public struct SaveDummyData
{
    public Vector3 myPosition;
    public Quaternion myRotation;
}
