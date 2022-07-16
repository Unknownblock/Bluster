using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Information Variables")] 
    public bool openedSettings;
    public bool canPause;
    public bool isPaused;

    [Header("Assignable Variables")] 
    public GameObject inGameUI;
    public GameObject pauseMenu;
    public Settings settingsPanel;

    [Header("Button Variables")] 
    public CustomButton pauseButton;
    public CustomButton backButton;
    
    public CustomButton resumeButton;
    public CustomButton restartButton;
    public CustomButton settingsButton;
    public CustomButton quitButton;

    public static PauseMenu Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UIManagement();
        ButtonManagement();

        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || backButton.isPressed)
            {
                backButton.isPressed = false;
                
                if (settingsPanel.settingsState == Settings.SettingsState.MainPage)
                {
                    openedSettings = false;
                }

                else if (settingsPanel.settingsState == Settings.SettingsState.ControlSettings)
                {
                    settingsPanel.controlSettings.Return();
                }

                else if (!openedSettings)
                {
                    isPaused = false;
                }
            }
        }

        if (!isPaused && canPause)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || pauseButton.isPressed)
            {
                isPaused = true;
            }
        }

        if (!canPause)
        {
            isPaused = false;
        }
    }

    private void UIManagement()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(true);
            inGameUI.SetActive(false);

            Time.timeScale = 0f;
        }

        else if (!isPaused)
        {
            pauseMenu.SetActive(false);
            settingsPanel.gameObject.SetActive(false);
            inGameUI.SetActive(true);

            openedSettings = false;

            Time.timeScale = 1f;
        }

        if (openedSettings)
        {
            pauseMenu.SetActive(true);
            settingsPanel.gameObject.SetActive(true);
            inGameUI.SetActive(false);
        }

        else if (!openedSettings)
        {
            settingsPanel.gameObject.SetActive(false);
        }
    }

    private void ButtonManagement()
    {
        if (resumeButton.isPressed)
        {
            isPaused = false;
            resumeButton.isPressed = false;
            pauseMenu.SetActive(false);
            settingsPanel.gameObject.SetActive(false);

            settingsPanel.settingsState = Settings.SettingsState.MainPage;
        }

        if (restartButton.isPressed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (settingsButton.isPressed)
        {
            settingsButton.isPressed = false;
            
            if (openedSettings)
            {
                openedSettings = false;
            }

            else if (!openedSettings)
            {
                openedSettings = true;
            }
        }

        if (quitButton.isPressed)
        {
            Application.Quit();
        }
    }
}
