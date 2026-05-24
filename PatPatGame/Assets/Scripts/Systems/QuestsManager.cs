using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class QuestsManager : MonoBehaviour
{
    private ShopItemDatabase itemDatabase;
    public List<Mission> dailyMissions { get; private set; }
    public List<Mission> weeklyMissions { get; private set; }
    private UIQuestsFunc questsFunc;
    private Dictionary<MissionType, List<Mission>> missionTypeLookup;
    private Dictionary<string, Mission> missionIdLookup;
    private PlayerBalance playerBalance;
    private TimeKeeper timeKeeper;

    public void Initialize()
    {
        itemDatabase = GameManager.Instance.shopItemDatabase;
        playerBalance = GameManager.Instance.playerBalance;
        timeKeeper = GameManager.Instance.timeKeeper;
        dailyMissions = itemDatabase.dailyMissions;
        weeklyMissions = itemDatabase.weeklyMissions;

        BuildMissionLookup();
        
        questsFunc = FindFirstObjectByType<UIQuestsFunc>();
        questsFunc.SetQuestsManager(this);
    }

    void Start()
    {
        Initialize();
        // Setting progress examples
        // Pet animal 10 times mission
        // questsFunc.GetUIQuestElement(dailyMissions[2]).SetProgress(7);
        // First quest in list
        // questsFunc.GetUIQuestElement(0).SetProgress(1);
        // questsFunc.GetUIQuestElement(1).SetProgress(3);
        // Other progress bar values are default visuals


        // Testing data
        /*Mission mission = new Mission
        {
            missionName = "Woah new quest waka waka " +
            "waka waka waka waka waka waka waka",
            rewardMoney = 555,
            rewardLikes = 2,
            targetGoal = 10
        };*/
        //questsFunc.AddQuest(mission);
        //questsFunc.AddQuest(dailyMissions[0]);
        //questsFunc.RemoveQuest(0);
        //questsFunc.RemoveQuest(1);
        //questsFunc.RemoveQuest(dailyMissions[0]);
        //questsFunc.RemoveQuest(mission);
    }


    /// <summary>
    /// Add progress amount to all missions of a given type
    /// </summary>
    public void CaptureProgress(MissionType type, int amount)
    {
        if (!missionTypeLookup.TryGetValue(type, out var missions))
            return;

        foreach (var mission in missions)
        {
            mission.progress += amount;

            if (mission.progress >= mission.targetGoal)
            {
                mission.progress = mission.targetGoal;
                CompleteMission(mission);
            }
            if (questsFunc.IsInitialized())
                questsFunc.GetUIQuestElement(mission).SetProgress(mission.progress);
        }
    }

    private void CompleteMission(Mission mission)
    {
        if (mission.isCompleted)
            return;
        
        mission.isCompleted = true;

        playerBalance.AddMoney(mission.rewardMoney);
        playerBalance.AddLikes(mission.rewardLikes);

        if (dailyMissions.Contains(mission) && mission.type != MissionType.CompleteDaily)
        {
            CaptureProgress(MissionType.CompleteDaily, 1);
        }
    }

    /// <summary>
    /// Apply mission resets based on last login, with optional overrides
    /// </summary>
    public void ApplyResets(bool forceResetDaily = false, bool forceResetWeekly = false)
    {
        if (timeKeeper.IsNewWeek() || forceResetWeekly)
        {
            foreach (var m in weeklyMissions)
            {
                m.progress = 0;
                m.isCompleted = false;

                if (forceResetWeekly)
                    questsFunc.GetUIQuestElement(m).SetProgress(m.progress);
            }
        }

        if (timeKeeper.IsNewDay() || forceResetDaily)
        {
            foreach (var m in dailyMissions)
            {
                m.progress = 0;
                m.isCompleted = false;

                if (forceResetDaily)
                    questsFunc.GetUIQuestElement(m).SetProgress(m.progress);
            }

            CaptureProgress(MissionType.LogIn, 1);
        }
    }

    private void BuildMissionLookup()
    {
        missionTypeLookup = new Dictionary<MissionType, List<Mission>>();
        missionIdLookup = new Dictionary<string, Mission>();

        AddMissionsToLookup(dailyMissions);
        AddMissionsToLookup(weeklyMissions);
    }

    private void AddMissionsToLookup(List<Mission> missions)
    {
        foreach (var mission in missions)
        {
            if (!missionTypeLookup.TryGetValue(mission.type, out var list))
            {
                list = new List<Mission>();
                missionTypeLookup[mission.type] = list;
            }

            list.Add(mission);

            if (!missionIdLookup.ContainsKey(mission.id))
                missionIdLookup[mission.id] = mission;
            else
                Debug.LogWarning($"Duplicate mission ID: {mission.id}");
        }
    }

    public QuestManagerSaveData Save()
    {
        List<QuestSaveData> quests = new List<QuestSaveData>();
        quests.AddRange(dailyMissions.Select(d => new QuestSaveData(d.id, d.progress, d.isCompleted)));
        quests.AddRange(weeklyMissions.Select(w => new QuestSaveData(w.id, w.progress, w.isCompleted)));
        return new QuestManagerSaveData { quests = quests };
    }

    public void Load(QuestManagerSaveData data)
    {
        if (data.quests == null)
            return;

        foreach (var questData in data.quests)
        {
            if (!missionIdLookup.TryGetValue(questData.id, out var quest))
                continue;

            if (questData.isCompleted == true)
            {
                quest.progress = quest.targetGoal;
                quest.isCompleted = true;
            }
            else
            {
                quest.progress = questData.progress;
            }
        }

        ApplyResets();
    }
}

[System.Serializable]
public struct QuestManagerSaveData
{
    public List<QuestSaveData> quests;
}

[System.Serializable]
public struct QuestSaveData
{
    public string id;
    public int progress;
    public bool isCompleted;

    public QuestSaveData(string id, int progress, bool isCompleted)
    {
        this.id = id;
        this.progress = progress;
        this.isCompleted = isCompleted;
    }
}
