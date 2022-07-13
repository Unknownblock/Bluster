using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum InputType
    {
        Desktop,
        Handheld
    }

    public enum SteeringType
    {
        ArrowSteering,
        SteeringWheel,
        GyroscopeSteering
    }
    
    public enum AccelerationType
    {
        PedalAcceleration,
        GyroscopeAcceleration
    }

    [Header("Main Input")] 
    public InputType inputType;
    public SteeringType steeringType;
    public AccelerationType accelerationType;

    [Header("UI")] 
    public GameObject touchUI;

    [Header("Keyboard Input")] 
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode brakeKey = KeyCode.Space;

    [Header("Input Management")] 
    public Vehicle vehicle;
    public SteeringWheel steeringWheel;
    public ArrowSteering arrowSteering;
    public CustomButton accelerationPedal;
    public CustomButton brakePedal;

    public float horizontalMovement;
    public float verticalMovement;

    public bool isBreaking;

    private Rigidbody vehicleRb;

    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        vehicleRb = vehicle.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Instance.inputType == InputType.Handheld)
        {
            TouchInputManagement();

            touchUI.SetActive(true);
        }

        if (Instance.inputType == InputType.Desktop)
        {
            DesktopInputManagement();

            touchUI.SetActive(false);
        }

        vehicle.verticalMovement = verticalMovement;
        vehicle.horizontalMovement = horizontalMovement;
        vehicle.isBreaking = isBreaking;
    }

    private void TouchInputManagement()
    {
        verticalMovement = 0f;

        if (steeringType == SteeringType.SteeringWheel)
        {
            horizontalMovement = steeringWheel.steeringInput;
            steeringWheel.currentReleaseSpeed = vehicleRb.velocity.magnitude;

            steeringWheel.gameObject.SetActive(true);
            arrowSteering.gameObject.SetActive(false);
        }

        else if (steeringType == SteeringType.ArrowSteering)
        {
            horizontalMovement = arrowSteering.steeringInput;

            steeringWheel.gameObject.SetActive(false);
            arrowSteering.gameObject.SetActive(true);
        }

        else if (steeringType == SteeringType.GyroscopeSteering)
        {
            horizontalMovement = Input.acceleration.x;

            steeringWheel.gameObject.SetActive(false);
            arrowSteering.gameObject.SetActive(false);
        }

        if (accelerationType == AccelerationType.GyroscopeAcceleration)
        {
            if (-Input.acceleration.z > 0.1f)
            {
                verticalMovement = -Input.acceleration.z;
            }

            isBreaking = -Input.acceleration.z < -0.1f;

            accelerationPedal.gameObject.SetActive(false);
            brakePedal.gameObject.SetActive(false);
        }

        else if (accelerationType == AccelerationType.PedalAcceleration)
        {
            isBreaking = brakePedal.isPressed;
            
            if (vehicle.gearMode == Vehicle.GearMode.Reverse && accelerationPedal.isPressed)
            {
                verticalMovement -= 1f;
            }

            else if (vehicle.gearMode == Vehicle.GearMode.Drive && accelerationPedal.isPressed)
            {
                verticalMovement += 1f;
            }

            else if (vehicle.gearMode == Vehicle.GearMode.Neutral && accelerationPedal.isPressed)
            {
                verticalMovement += 1f;
            }

            else if (vehicle.gearMode == Vehicle.GearMode.Park && accelerationPedal.isPressed)
            {
                verticalMovement += 1f;
            }
            
            accelerationPedal.gameObject.SetActive(true);
            brakePedal.gameObject.SetActive(true);
        }
    }

    private void DesktopInputManagement()
    {
        verticalMovement = 0f;

        if (PauseMenu.Instance.isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else if (!PauseMenu.Instance.isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        horizontalMovement = 0f;

        isBreaking = Input.GetKey(Instance.brakeKey);

        if (Input.GetKey(rightKey))
        {
            horizontalMovement += 1f;
        }

        if (Input.GetKey(leftKey))
        {
            horizontalMovement -= 1f;
        }

        if (Input.GetKey(forwardKey) && !vehicle.isShiftingGear)
        {
            vehicle.gearMode = Vehicle.GearMode.Drive;
            verticalMovement += 1f;
        }

        if (Input.GetKey(backwardKey) && !vehicle.isShiftingGear)
        {
            vehicle.gearMode = Vehicle.GearMode.Reverse;
            verticalMovement -= 1f;
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (vehicle.transmissionMode == Vehicle.TransmissionMode.Automatic)
            {
                vehicle.transmissionMode = Vehicle.TransmissionMode.Manual;
            }
            
            else if (vehicle.transmissionMode == Vehicle.TransmissionMode.Manual)
            {
                vehicle.transmissionMode = Vehicle.TransmissionMode.Automatic;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            vehicle.gearMode = Vehicle.GearMode.Neutral;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            vehicle.gearMode = Vehicle.GearMode.Park;
        }
    }
}
