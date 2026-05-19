using System;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
    public DateTime firstLogin { get; private set; }
    public DateTime lastLogin { get; private set; }
    public double totalPlayTime { get; private set; }

    void Start()
    {
        if (firstLogin == DateTime.MinValue)
        {
            firstLogin = DateTime.Now;
            totalPlayTime = 0;
        }
    }

    void Update()
    {
        totalPlayTime += Time.deltaTime;
    }

    public string GetTimeString()
    {
        return TimeSpan.FromSeconds(Math.Floor(totalPlayTime)).ToString();
    }

    public double GetSecondsSinceLastSave()
    {
        return (DateTime.Now - lastLogin).TotalSeconds;
    }

    public double GetHoursSinceLastSave()
    {
        return (DateTime.Now - lastLogin).TotalHours;
    }

    public TimeKeeperData Save()
    {
        TimeKeeperData data = new TimeKeeperData();
        data.firstLoginDate = firstLogin.ToString();
        data.saveDate = DateTime.Now.ToString();
        data.playTime = GetTimeString();
        return data;
    }

    public void Load(TimeKeeperData data)
    {
        firstLogin = DateTime.Parse(data.firstLoginDate);
        lastLogin = DateTime.Parse(data.saveDate);
        totalPlayTime = TimeSpan.Parse(data.playTime).TotalSeconds;
        // Debug.Log(string.Format("TimeKeeper data loaded: First login at {0};  Last login at {1};  Total playtime {2}",
        //     data.firstLoginDate, data.saveDate, data.playTime));
    }
}

[System.Serializable]
public struct TimeKeeperData
{
    public string firstLoginDate;
    public string saveDate;
    public string playTime;
}
