using System;
using UnityEngine;
using System.Globalization;

public class TimeKeeper : MonoBehaviour
{
    public DateTime firstLogin { get; private set; }
    public DateTime lastLogin { get; private set; }
    public double totalPlayTime { get; private set; }
    public int lastWeeklyResetWeek { get; private set; }
    public int lastWeeklyResetYear { get; private set; }

    void Start()
    {
        if (firstLogin == DateTime.MinValue)
        {
            firstLogin = DateTime.Now;
            totalPlayTime = 0;
            MarkWeeklyReset();
        }
    }

    void Update()
    {
        totalPlayTime += Time.deltaTime;
    }

    public string GetTimeString()
    {
        TimeSpan time = TimeSpan.FromSeconds(Math.Floor(totalPlayTime));
        return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
    }

    public double GetSecondsSinceLastSave()
    {
        return (DateTime.Now - lastLogin).TotalSeconds;
    }

    public double GetHoursSinceLastSave()
    {
        return (DateTime.Now - lastLogin).TotalHours;
    }

    public bool IsNewDay()
    {
        return lastLogin.Date != DateTime.Now.Date;
    }

    public bool IsNewWeek()
    {
        DateTime now = DateTime.Now;

        int currentWeek = ISOWeek.GetWeekOfYear(now);
        int currentYear = now.Year;

        return currentWeek != lastWeeklyResetWeek
               || currentYear != lastWeeklyResetYear;
    }

    public void MarkWeeklyReset()
    {
        DateTime now = DateTime.Now;

        lastWeeklyResetWeek = ISOWeek.GetWeekOfYear(now);
        lastWeeklyResetYear = now.Year;
    }

    public TimeKeeperData Save()
    {
        if (lastWeeklyResetWeek <= 0 || lastWeeklyResetYear <= 0)
        {
            MarkWeeklyReset();
        }

        TimeKeeperData data = new TimeKeeperData();
        data.firstLoginDate = firstLogin.ToString("O");
        data.saveDate = DateTime.Now.ToString("O");
        data.playTime = GetTimeString();
        // data.playTime = totalPlayTime;
        data.lastWeeklyResetWeek = lastWeeklyResetWeek;
        data.lastWeeklyResetYear = lastWeeklyResetYear;
        return data;
    }

    public void Load(TimeKeeperData data)
    {
        firstLogin = DateTime.Parse(data.firstLoginDate);
        lastLogin = DateTime.Parse(data.saveDate);
        totalPlayTime = TimeSpan.Parse(data.playTime).TotalSeconds;
        // totalPlayTime = data.playTime;
        lastWeeklyResetWeek = data.lastWeeklyResetWeek;
        lastWeeklyResetYear = data.lastWeeklyResetYear;

        // Backward compatibility / first-time initialization
        if (lastWeeklyResetWeek <= 0 || lastWeeklyResetYear <= 0)
        {
            MarkWeeklyReset();
        }
    }
}

[System.Serializable]
public struct TimeKeeperData
{
    public string firstLoginDate;
    public string saveDate;
    public string playTime;
    // public double playTime;
    public int lastWeeklyResetWeek;
    public int lastWeeklyResetYear;
}
