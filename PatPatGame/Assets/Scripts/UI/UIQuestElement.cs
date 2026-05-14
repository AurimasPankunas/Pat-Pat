using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIQuestElement
{
    public TemplateContainer _questElement;
    private Label _questName;
    private Label _moneyAmount;
    private VisualElement _moneyImage;
    private Label _likesAmount;
    private VisualElement _likesImage;
    private Label _timeLabel;
    private ProgressBar _progressBar;
    private Label _targetGoal;
    private Label _progressValue;
    public Mission missionData;

    public UIQuestElement(
        VisualTreeAsset templateContainer,
        Mission missionData
    )
    {
        this.missionData = missionData;
        // Instantiate visual element
        this._questElement = templateContainer.Instantiate();
        _questElement.name = templateContainer.name;
        // Get main UI elements from UI document
        _questName = _questElement.Q<Label>("NameLabel");
        _timeLabel = _questElement.Q<Label>("TimeLabel");
        _moneyAmount = _questElement.Q<Label>("MoneyAmount");
        _moneyImage = _questElement.Q<VisualElement>("MoneyImg");
        _likesAmount = _questElement.Q<Label>("LikesAmount");
        _likesImage = _questElement.Q<VisualElement>("LikesImg");
        _progressBar = _questElement.Q<ProgressBar>("ProgressBar");
        _targetGoal = _questElement.Q<Label>("TargetGoal");
        _progressValue = _questElement.Q<Label>("ProgressValue");

        // Set text and visuals
        _questName.text = missionData.missionName;
        if(missionData.rewardMoney > 0){
            _moneyAmount.text = missionData.rewardMoney + "";
        }
        else{
            _moneyAmount.style.display = DisplayStyle.None;
            _moneyImage.style.display = DisplayStyle.None;
        }

        if (missionData.rewardLikes > 0){
            _likesAmount.text = missionData.rewardLikes + "";
        }
        else{
            _likesAmount.style.display = DisplayStyle.None;
            _likesImage.style.display = DisplayStyle.None;
        }
        _targetGoal.text = "/" + missionData.targetGoal;
    }

    public void SetProgress(int progressValue)
    {
        if(progressValue >= 0) { 
            _progressValue.text = progressValue.ToString();
            _progressBar.value = (float)progressValue / missionData.targetGoal;
        }
    }

    public void SetTime(TimeSpan time)
    {
        if(time.Days != 0){
            _timeLabel.text = time.Days + "d ";
        } else
            _timeLabel.text = time.ToString(@"hh\:mm");
    }

    public void HideTime(bool isHidden)
    {
        if(isHidden)
            _timeLabel.style.display = DisplayStyle.None;
        else
            _timeLabel.style.display = DisplayStyle.Flex;
    }
}
