using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIQuestsFunc : MonoBehaviour
{
    private QuestsManager questsManager;
    private UIDocument _document;
    private VisualElement _ListContainer;

    [SerializeField] private VisualTreeAsset QuestElement;
    private List<UIQuestElement> questElements;

    void Awake()
    {
        // Gets the document on the gameObject that has the UI elements
        _document = GetComponent<UIDocument>();

        // Grabbing UI elements from the document
        _ListContainer = _document.rootVisualElement.Q<VisualElement>("ListContainer");

        questElements = new List<UIQuestElement>();
    }

    /// <summary>
    /// Generates (adds to) a list of quest items in the UI from a list
    /// </summary>
    /// <param name="missionItem"></param>
    public void GenerateList(List<Mission> missionItem)
    {
        if (missionItem.Count == 0) return;
        foreach (Mission item in missionItem)
        {
            UIQuestElement element = new UIQuestElement(QuestElement, item);
            element.HideTime(true);
            questElements.Add(element);
            _ListContainer.Add(element._questElement);
        }
    }

    /// <summary>
    /// Generates (adds to) a list of quest items in the UI from a list.
    /// Uses TimeSpan to show how long a quest is going to last for
    /// </summary>
    /// <param name="missionItem"></param>
    /// <param name="time">The amount of time the quest is going to last for</param>
    public void GenerateList(List<Mission> missionItem, TimeSpan time)
    {
        if (missionItem.Count == 0) return;
        foreach (Mission item in missionItem)
        {
            UIQuestElement element = new UIQuestElement(QuestElement, item);
            element.SetTime(time);
            questElements.Add(element);
            _ListContainer.Add(element._questElement);
        }
    }

    /// <summary>
    /// Clears out the quest item list
    /// </summary>
    public void ClearList()
    {
        foreach (UIQuestElement item in questElements)
        {
            item._questElement.RemoveFromHierarchy();
        }
        questElements.Clear();
    }

    public void AddQuest(Mission mission)
    {
        if (mission == null) return;
        UIQuestElement element = new UIQuestElement(QuestElement, mission);
        element.HideTime(true);
        questElements.Add(element);
        _ListContainer.Add(element._questElement);
    }

    /// <param name="mission"></param>
    /// <param name="time">How long a quest is going to last for</param>
    public void AddQuest(Mission mission, TimeSpan time)
    {
        if (mission == null) return;
        UIQuestElement element = new UIQuestElement(QuestElement, mission);
        element.SetTime(time);
        questElements.Add(element);
        _ListContainer.Add(element._questElement);
    }

    /// <summary>
    /// Remove quest at index
    /// </summary>
    /// <param name="index"></param>
    public void RemoveQuest(int index)
    {
        if(questElements.Count > index) { 
            questElements[index]._questElement.RemoveFromHierarchy();
            questElements.RemoveAt(index);
        }
    }

    /// <summary>
    /// Remove specified quest (mission)
    /// </summary>
    /// <param name="mission"></param>
    public void RemoveQuest(Mission mission)
    {
        if (mission == null) return;
        UIQuestElement quest = questElements.Find(m => m.missionData == mission);
        if(quest == null) { return; }
        quest._questElement.RemoveFromHierarchy();
        questElements.Remove(quest);
    }

    /// <summary>
    /// Get Quest element by specified mission
    /// </summary>
    /// <param name="mission"></param>
    /// <returns></returns>
    public UIQuestElement GetUIQuestElement(Mission mission)
    {
        if (mission == null) return null;
        return questElements.Find(m => m.missionData == mission);
    }

    /// <summary>
    /// Get Quest element at specified index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public UIQuestElement GetUIQuestElement(int index)
    {
        if (questElements.Count > index)
        {
            return questElements[index];
        }
        return null;
    }

    public void SetQuestsManager(QuestsManager questsManager)
    {
        this.questsManager = questsManager;
    }
}
