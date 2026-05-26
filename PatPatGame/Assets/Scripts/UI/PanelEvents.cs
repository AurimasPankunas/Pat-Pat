using UnityEngine;

public class PanelEvents : MonoBehaviour
{
    private SettingsManager settingsManager;
    private Canvas canvas;
    void Start()
    {
        settingsManager = GameManager.Instance.settingsManager;
        canvas = GetComponent<Canvas>();
        settingsManager.OnHintToggleChanged += HintsHide;
    }

    private void HintsHide(bool value)
    {
        canvas.gameObject.SetActive(value);
    }
}
