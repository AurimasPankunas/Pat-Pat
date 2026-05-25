using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class UISettingsController : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    private UIDocument _document;
    private Toggle _hintToggle;
    private RadioButton _teleportationRadioButton;
    private RadioButton _continuousRadioButton;
    private RadioButtonGroup _movementTypeGroup;
    private Slider _masterSlider;
    private Slider _musicSlider;
    private Slider _SFXSlider;
    private Slider _UISlider;
    void Awake()
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

        //_masterSlider.RegisterValueChangedCallback(MasterAudioChanged);
        //_musicSlider.RegisterValueChangedCallback(MusicAudioChanged);
        //_SFXSlider.RegisterValueChangedCallback(SFXAudioChanged);
        //_UISlider.RegisterValueChangedCallback(AmbientAudioChanged);
        SetComponents();
    }

    void SetComponents()
    {
        float audioValue;
        if (PlayerPrefs.HasKey("HintToggle")){

        }
        if (PlayerPrefs.HasKey("MovementType")){

        }
        if (PlayerPrefs.HasKey("MasterVolume"))
        {

        }
        else {
            audioMixer.GetFloat("MasterVolume", out audioValue);
            //_masterSlider.value = MixerToSlider(audioValue);
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {

        }
        else {
            audioMixer.GetFloat("MusicVolume", out audioValue);
            //_musicSlider.value = MixerToSlider(audioValue);
        }
        if (PlayerPrefs.HasKey("SFXVolume")){

        }
        else{
            audioMixer.GetFloat("SFXVolume", out audioValue);
            //_SFXSlider.value = MixerToSlider(audioValue);
        }
        if (PlayerPrefs.HasKey("UIVolume")){

        }
        else{
            audioMixer.GetFloat("UIVolume", out audioValue);
            //_UISlider.value = MixerToSlider(audioValue);
        }
    }
}
