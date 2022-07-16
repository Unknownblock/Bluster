
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public enum MissionState
    {
        Normal,
        Failed,
        Completed
    }
    
    [Header("Information Variables")]
    public MissionState missionState;
    
    public float parkDistance;
    public float parkSpeed;
    public float remainingTime;

    [Header("UI Variables")] public TextMeshProUGUI healthUI;
    public TextMeshProUGUI remainingTimeUI;
    
    public GameObject failUI;
    public GameObject completedUI;
    public GameObject inGameUI;
    public CustomButton quitButton;
    public CustomButton restartButton;
    
    [Header("Assignable Variable")]
    public GameObject parkingSpot;
    public Vehicle vehicle;
    public VehicleCollision vehicleCollision;
    
    public static MissionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UIManagement();
        ButtonManagement();

        remainingTime -= Time.deltaTime;
        
        var XZVehiclePos = XZVector(vehicle.gameObject.transform.position);

        var distance = Vector3.Distance(XZVehiclePos, parkingSpot.transform.position);

        if (distance <= parkDistance)
        {
            if (vehicle.vehicleSpeed <= parkSpeed)
            {
                missionState = MissionState.Completed;
                PauseMenu.Instance.canPause = false;
                
                if (vehicleCollision.currentHealth > 50f)
                {
                    missionState = MissionState.Completed;
                }
                
                if (vehicleCollision.currentHealth <= 50f)
                {
                    missionState = MissionState.Failed;
                }
            }
        }
        
        else if (remainingTime <= 0f || vehicleCollision.currentHealth <= 10f)
        {
            remainingTime = 0f;
            missionState = MissionState.Failed;
        }
        
        else if (missionState == MissionState.Normal)
        {
            PauseMenu.Instance.canPause = true;
        }
    }

    private void UIManagement()
    {
        var minutes = Mathf.Floor(remainingTime / 60);
        var seconds = Mathf.RoundToInt(remainingTime % 60);

        remainingTimeUI.text = $"{minutes:00}:{seconds:00}";
        healthUI.text = vehicleCollision.currentHealth.ToString();
        
        if (missionState == MissionState.Normal)
        {
            completedUI.SetActive(false);
            failUI.SetActive(false);
            inGameUI.SetActive(true);
            quitButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);

            Time.timeScale = 1f;
        }
        
        else if (missionState == MissionState.Completed)
        {
            completedUI.SetActive(true);
            failUI.SetActive(false);
            inGameUI.SetActive(false);
            quitButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            
            Time.timeScale = 0f;
        }
        
        else if (missionState == MissionState.Failed)
        {
            completedUI.SetActive(false);
            failUI.SetActive(true);
            inGameUI.SetActive(false);
            quitButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            
            Time.timeScale = 0f;
        }
    }

    private void ButtonManagement()
    {
        if (restartButton.isPressed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        if (quitButton.isPressed)
        {
            Application.Quit();
        }
    }
    
    private static Vector3 XZVector(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
    }
}
