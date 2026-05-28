using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class UISettingsFunc : MonoBehaviour
{
    private SettingsManager settingsManager;
    private UIDocument _document;
    private Toggle _hintToggle;
    private RadioButton _teleportationRadioButton;
    private RadioButton _continuousRadioButton;
    private RadioButtonGroup _movementTypeGroup;
    private Slider _masterSlider;
    private Slider _musicSlider;
    private Slider _SFXSlider;
    private Slider _UISlider;
    void Start()
    {
        // Gets the document on the gameObject that has the UI elements
        _document = GetComponent<UIDocument>();

        _hintToggle = _document.rootVisualElement.Q<Toggle>("HintToggle");
        _movementTypeGroup = _document.rootVisualElement.Q<RadioButtonGroup>("MovementTypeGroup");
        _teleportationRadioButton = _document.rootVisualElement.Q<RadioButton>("RadioTeleportation");
        _continuousRadioButton = _document.rootVisualElement.Q<RadioButton>("RadioContinuous");
        // Volume sliders
        _masterSlider = _document.rootVisualElement.Q<Slider>("SliderMaster");
        _musicSlider = _document.rootVisualElement.Q<Slider>("SliderMusic");
        _SFXSlider = _document.rootVisualElement.Q<Slider>("SliderSFX");
        _UISlider = _document.rootVisualElement.Q<Slider>("SliderUI");

        _hintToggle.RegisterValueChangedCallback(HintToggleChanged);
        _movementTypeGroup.RegisterValueChangedCallback(MovementTypeChanged);

        _masterSlider.RegisterValueChangedCallback(MasterVolumeChanged);
        _musicSlider.RegisterValueChangedCallback(MusicVolumeChanged);
        _SFXSlider.RegisterValueChangedCallback(SFXVolumeChanged);
        _UISlider.RegisterValueChangedCallback(UIVolumeChanged);
    }

    private void HintToggleChanged(ChangeEvent<bool> evt)
    {
        settingsManager.HintToggleChanged(evt.newValue);
    }

    private void MovementTypeChanged(ChangeEvent<int> evt)
    {
        settingsManager.MovementTypeChanged(evt.newValue);
    }


    private void MasterVolumeChanged(ChangeEvent<float> evt)
    {
        settingsManager.MasterVolumeChanged(evt.newValue);
    }

    private void MusicVolumeChanged(ChangeEvent<float> evt)
    {
        settingsManager.MusicVolumeChanged(evt.newValue);
    }

    private void SFXVolumeChanged(ChangeEvent<float> evt)
    {
        settingsManager.SFXVolumeChanged(evt.newValue);
    }

    private void UIVolumeChanged(ChangeEvent<float> evt)
    {
        settingsManager.UIVolumeChanged(evt.newValue);
    }

    public void SetHintToggle(bool value)
    {
        _hintToggle.value = value;
    }

    public void SetMovementType(int value)
    {
        _movementTypeGroup.value = value;
    }

    public void SetMasterSliderUI(float value)
    {
        _masterSlider.value = value;
    }
    public void SetMusicSliderUI(float value)
    {
        _musicSlider.value = value;
    }
    public void SetSFXSliderUI(float value)
    {
        _SFXSlider.value = value;
    }
    public void SetUISliderUI(float value)
    {
        _UISlider.value = value;
    }

    public void SetSettingsManager(SettingsManager settingsManager)
    {
        this.settingsManager = settingsManager;
    }

    private void OnDisable()
    {
        _masterSlider.UnregisterValueChangedCallback(MasterVolumeChanged);
        _musicSlider.UnregisterValueChangedCallback(MusicVolumeChanged);
        _SFXSlider.UnregisterValueChangedCallback(SFXVolumeChanged);
        _UISlider.UnregisterValueChangedCallback(UIVolumeChanged);
    }
}
