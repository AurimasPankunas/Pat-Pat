using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class QuestsManager : MonoBehaviour
{
    private ShopItemDatabase itemDatabase;
    private List<Mission> dailyMissions;
    private List<Mission> weeklyMissions;
    private UIQuestsFunc questsFunc;

    void Start()
    {
        itemDatabase = GameManager.Instance.shopItemDatabase;
        dailyMissions = itemDatabase.dailyMissions;
        weeklyMissions = itemDatabase.weeklyMissions;
        questsFunc = GetComponent<UIQuestsFunc>();
        questsFunc.SetQuestsManager(this);

        // Functions are available without TimeSpan as well
        // TimeSpan currently does not have a countdown
        questsFunc.GenerateList(dailyMissions, new System.TimeSpan(24, 0, 0));
        questsFunc.GenerateList(weeklyMissions, new System.TimeSpan(7,0, 0, 0));

        // Setting progress examples
        // Pet animal 10 times mission
        questsFunc.GetUIQuestElement(dailyMissions[2]).SetProgress(7);
        // First quest in list
        questsFunc.GetUIQuestElement(0).SetProgress(1);
        questsFunc.GetUIQuestElement(1).SetProgress(3);
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
}
