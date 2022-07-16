using UnityEngine;

public class Settings : MonoBehaviour
{
    public enum SettingsState
    {
        MainPage,
        ControlSettings,
    }

    [Header("Main Variables")]
    public SettingsState settingsState;

    public GameObject mainPage;

    [Header("Control Settings")]
    public CustomButton controlSettingsButton;
    public ControlSettings controlSettings;
    
    private void Update()
    {
        UIManagement();
        
        if (controlSettingsButton.isPressed)
        {
            settingsState = SettingsState.ControlSettings;
        }
    }

    private void UIManagement()
    {
        if (settingsState == SettingsState.MainPage)
        {
            mainPage.SetActive(true);
            controlSettings.gameObject.SetActive(false);
        }
        
        else if (settingsState == SettingsState.ControlSettings)
        {
            mainPage.SetActive(false);
            controlSettings.gameObject.SetActive(true);
        }
    }
}

