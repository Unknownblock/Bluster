using UnityEngine;

public class ControlSettings : MonoBehaviour
{
    public enum ControlSettingsState
    {
        ControlMainPage,
        Transmission,
        SteeringSettings,
        AccelerationSettings
    }

    [Header("Main Variables")]
    public ControlSettingsState controlSettingsState;
    
    public GameObject mainPage;
    public Settings settings;
    
    [Header("Acceleration Settings Variables")]
    public GameObject accelerationSettings;
    
    public CustomButton accelerationSettingsButton;
    public CustomButton pedalAccelerationOption;
    public CustomButton gyroscopeAccelerationOption;
    
    [Header("Steering Settings Variables")] 
    public GameObject steeringSettings;
    
    public CustomButton steeringSettingsButton;
    public CustomButton arrowSteeringOption;
    public CustomButton steeringWheelOption;
    public CustomButton gyroscopeSteeringOption;

    private void Update()
    {
        SettingsManagement();
        UIManagement();
    }

    private void SettingsManagement()
    {
        if (controlSettingsState == ControlSettingsState.SteeringSettings)
        {
            SteeringSettings();
        }
        
        else if (controlSettingsState == ControlSettingsState.ControlMainPage)
        {
            ControlsSettings();
        }
        
        else if (controlSettingsState == ControlSettingsState.AccelerationSettings)
        {
            AccelerationSettings();
        }
    }
    
    private void ControlsSettings()
    {
        if (accelerationSettingsButton.isPressed)
        {
            controlSettingsState = ControlSettingsState.AccelerationSettings;
        }
        
        else if (steeringSettingsButton.isPressed)
        {
            controlSettingsState = ControlSettingsState.SteeringSettings;
        }
    }

    private void SteeringSettings()
    {
        if (steeringWheelOption.isPressed)
        {
            InputManager.Instance.steeringType = InputManager.SteeringType.SteeringWheel;
        }
        
        else if (arrowSteeringOption.isPressed)
        {
            InputManager.Instance.steeringType = InputManager.SteeringType.ArrowSteering;
        }
        
        else if (gyroscopeSteeringOption.isPressed)
        {
            InputManager.Instance.steeringType = InputManager.SteeringType.GyroscopeSteering;
        }
    }

    private void AccelerationSettings()
    {
        if (gyroscopeAccelerationOption.isPressed)
        {
            InputManager.Instance.accelerationType = InputManager.AccelerationType.GyroscopeAcceleration;
        }
        
        else if (pedalAccelerationOption.isPressed)
        {
            InputManager.Instance.accelerationType = InputManager.AccelerationType.PedalAcceleration;
        }
    }
    
    private void UIManagement()
    {
        if (controlSettingsState == ControlSettingsState.ControlMainPage)
        {
            mainPage.SetActive(true);
            steeringSettings.SetActive(false);
            accelerationSettings.SetActive(false);
        }
        
        else if (controlSettingsState == ControlSettingsState.SteeringSettings)
        {
            mainPage.SetActive(false);
            steeringSettings.SetActive(true);
            accelerationSettings.SetActive(false);
        }
        
        else if (controlSettingsState == ControlSettingsState.AccelerationSettings)
        {
            mainPage.SetActive(false);
            steeringSettings.SetActive(false);
            accelerationSettings.SetActive(true);
        }
    }

    private void OnDisable()
    {
        controlSettingsState = ControlSettingsState.ControlMainPage;
    }
    
    public void Return()
    {
        if (controlSettingsState == ControlSettingsState.AccelerationSettings)
        {
            controlSettingsState = ControlSettingsState.ControlMainPage;
        }

        else if (settings.controlSettings.controlSettingsState == ControlSettingsState.SteeringSettings)
        {
            controlSettingsState = ControlSettingsState.ControlMainPage;
        }
                    
        else if (settings.controlSettings.controlSettingsState == ControlSettingsState.ControlMainPage)
        {
            settings.settingsState = Settings.SettingsState.MainPage;
        }
    }
}
