using UnityEngine;

public class ControlSettings : MonoBehaviour
{
    public enum ControlSettingsState
    {
        ControlMainPage,
        SteeringSettings,
        AccelerationSettings,
        TransmissionSettings
    }

    [Header("Main Variables")]
    public ControlSettingsState controlSettingsState;
    
    public GameObject mainPage;
    public Settings settings;
    
    [Header("Acceleration Settings Variables")]
    public SettingsOptionManager accelerationSettings;
    
    public CustomButton accelerationSettingsButton;
    
    [Header("Steering Settings Variables")] 
    public SettingsOptionManager steeringSettings;
    
    public CustomButton steeringSettingsButton;

    [Header("Transmission Settings Variables")] 
    public SettingsOptionManager transmissionSettings;
    
    public CustomButton transmissionSettingsButton;

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

        else if (controlSettingsState == ControlSettingsState.AccelerationSettings)
        {
            AccelerationSettings();
        }
        
        else if (controlSettingsState == ControlSettingsState.ControlMainPage)
        {
            ControlsSettings();
        }

        else if (controlSettingsState == ControlSettingsState.TransmissionSettings)
        {
            TransmissionSettings();
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

        else if (transmissionSettingsButton.isPressed)
        {
            controlSettingsState = ControlSettingsState.TransmissionSettings;
        }
    }

    private void SteeringSettings()
    {
        if (steeringSettings.chosenOptionNum == 0)
        {
            InputManager.Instance.steeringType = InputManager.SteeringType.SteeringWheel;
        }
        
        else if (steeringSettings.chosenOptionNum == 1)
        {
            InputManager.Instance.steeringType = InputManager.SteeringType.ArrowSteering;
        }
        
        else if (steeringSettings.chosenOptionNum == 2)
        {
            InputManager.Instance.steeringType = InputManager.SteeringType.GyroscopeSteering;
        }
    }

    private void TransmissionSettings()
    {
        if (transmissionSettings.chosenOptionNum == 0)
        {
            InputManager.Instance.transmissionType = InputManager.TransmissionType.AutomaticTransmission;
        }
        
        else if (transmissionSettings.chosenOptionNum == 1)
        {
            InputManager.Instance.transmissionType = InputManager.TransmissionType.ManualTransmission;
        }
        
        else if (transmissionSettings.chosenOptionNum == 2)
        {
            InputManager.Instance.transmissionType = InputManager.TransmissionType.SimpleTransmission;
        }
    }

    private void AccelerationSettings()
    {
        if (accelerationSettings.chosenOptionNum == 0)
        {
            InputManager.Instance.accelerationType = InputManager.AccelerationType.PedalAcceleration;
        }

        else if (accelerationSettings.chosenOptionNum == 1)
        {
            InputManager.Instance.accelerationType = InputManager.AccelerationType.GyroscopeAcceleration;
        }
    }
    
    private void UIManagement()
    {
        if (controlSettingsState == ControlSettingsState.ControlMainPage)
        {
            mainPage.SetActive(true);
            steeringSettings.gameObject.SetActive(false);
            accelerationSettings.gameObject.SetActive(false);
            transmissionSettings.gameObject.SetActive(false);
        }
        
        else if (controlSettingsState == ControlSettingsState.SteeringSettings)
        {
            mainPage.SetActive(false);
            steeringSettings.gameObject.SetActive(true);
            accelerationSettings.gameObject.SetActive(false);
            transmissionSettings.gameObject.SetActive(false);
        }
        
        else if (controlSettingsState == ControlSettingsState.AccelerationSettings)
        {
            mainPage.SetActive(false);
            steeringSettings.gameObject.SetActive(false);
            accelerationSettings.gameObject.SetActive(true);
            transmissionSettings.gameObject.SetActive(false);
        }

        else if (controlSettingsState == ControlSettingsState.TransmissionSettings)
        {
            mainPage.SetActive(false);
            steeringSettings.gameObject.SetActive(false);
            accelerationSettings.gameObject.SetActive(false);
            transmissionSettings.gameObject.SetActive(true);
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

        else if (settings.controlSettings.controlSettingsState == ControlSettingsState.TransmissionSettings)
        {
            controlSettingsState = ControlSettingsState.ControlMainPage;
        }
                    
        else if (settings.controlSettings.controlSettingsState == ControlSettingsState.ControlMainPage)
        {
            settings.settingsState = Settings.SettingsState.MainPage;
        }
    }
}
