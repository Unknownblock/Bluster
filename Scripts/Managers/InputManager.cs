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

    public enum TransmissionType
    {
        AutomaticTransmission,
        ManualTransmission,
        TiptronicTransmission,
        SimpleTransmission
    }

    [Header("Main Input")] 
    public InputType inputType;
    public SteeringType steeringType;
    public AccelerationType accelerationType;
    public TransmissionType transmissionType;

    [Header("UI")] 
    public GameObject touchUI;

    [Header("Keyboard Input")] 
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode startEngineKey = KeyCode.F;
    public KeyCode gearUp = KeyCode.E;
    public KeyCode gearDown = KeyCode.Q;
    public KeyCode brakeKey = KeyCode.Space;

    [Header("Input Management")]
    public SteeringWheel steeringWheel;
    public ArrowSteering arrowSteering;
    public GearShifter automaticGearShifter;
    public GearShifter manualGearShifter;
    public CustomButton accelerationPedal;
    public CustomButton brakePedal;

    public CustomButton startEngineButton;
    
    public CustomButton positiveGearButton;
    public CustomButton negativeGearButton;

    public float horizontalMovement;
    public float verticalMovement;
    
    public float mouseX;
    public float mouseY;

    public bool isBreaking;

    private Rigidbody vehicleRb;

    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        vehicleRb = Vehicle.Instance.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        AccelerationInput();
        SteeringInput();
        GearInputManagement();
        UIManagement();

        mouseX = Input.GetAxisRaw("Mouse X");
		mouseY = Input.GetAxisRaw("Mouse Y");

        Vehicle.Instance.verticalMovement = verticalMovement;
        Vehicle.Instance.horizontalMovement = horizontalMovement;
        Vehicle.Instance.isBreaking = isBreaking;
    }

    private void UIManagement()
    {
        if (inputType == InputType.Desktop)
        {
            touchUI.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (inputType == InputType.Handheld)
        {
            touchUI.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void AccelerationInput()
    {
        verticalMovement = 0f;

        if (inputType == InputType.Desktop)
        {
            if (Input.GetKey(forwardKey))
            {
                verticalMovement++;
            }

            if (Input.GetKey(backwardKey))
            {
                verticalMovement--;
            }
        }

        else if (inputType == InputType.Handheld)
        {
            TouchAcceleration();
        }
    }

    private void SteeringInput()
    {
        horizontalMovement = 0f;
        
        if (inputType == InputType.Desktop)
        {
            if (Input.GetKey(rightKey))
            {
                horizontalMovement++;
            }

            if (Input.GetKey(leftKey))
            {
                horizontalMovement--;
            }

            isBreaking = Input.GetKey(brakeKey);
        }

        else if (inputType == InputType.Handheld)
        {
            TouchSteering();
        }
    }

    private void TouchAcceleration()
    {
        if (accelerationType == AccelerationType.PedalAcceleration)
        {
            accelerationPedal.gameObject.SetActive(true);
            brakePedal.gameObject.SetActive(true);

            if (transmissionType == TransmissionType.SimpleTransmission)
            {
                if (accelerationType == AccelerationType.PedalAcceleration)   
                {
                    if (accelerationPedal.isPressed)
                    {
                        verticalMovement++;
                    }

                    if (brakePedal.isPressed)
                    {
                        verticalMovement--;
                    }
                }
            }

            else if (transmissionType != TransmissionType.SimpleTransmission)
            {
                if (accelerationPedal.isPressed)
                {
                    if (Vehicle.Instance.gearMode != Vehicle.GearMode.Reverse)
                    {
                        verticalMovement++;
                    }

                    else if (Vehicle.Instance.gearMode == Vehicle.GearMode.Reverse)
                    {
                        verticalMovement--;
                    }
                }

                isBreaking = brakePedal.isPressed;
            }
        }

        else if (accelerationType == AccelerationType.GyroscopeAcceleration)
        {
            accelerationPedal.gameObject.SetActive(false);
            brakePedal.gameObject.SetActive(false);

            verticalMovement = Input.acceleration.z;


            if (verticalMovement > 0f)
            {
                Vehicle.Instance.gearMode = Vehicle.GearMode.Drive;
            }

            else if (verticalMovement < 0f)
            {
                Vehicle.Instance.gearMode = Vehicle.GearMode.Reverse;
            }
        }
    }

    private void TouchSteering()
    {
        if (steeringType == SteeringType.SteeringWheel)
        {
            steeringWheel.gameObject.SetActive(true);
            arrowSteering.gameObject.SetActive(false);

            horizontalMovement = steeringWheel.steeringInput;
        }

        else if (steeringType == SteeringType.ArrowSteering)
        {
            steeringWheel.gameObject.SetActive(false);
            arrowSteering.gameObject.SetActive(true);

            horizontalMovement = arrowSteering.steeringInput;
        }

        else if (steeringType == SteeringType.GyroscopeSteering)
        {
            steeringWheel.gameObject.SetActive(false);
            arrowSteering.gameObject.SetActive(false);

            horizontalMovement = Input.acceleration.x;
        }
    }

    private void GearInputManagement()
    {
        if (inputType == InputType.Desktop)
        {
            if (Input.GetKeyDown(gearUp))
            {
                if (Vehicle.Instance.currentGearNum < Vehicle.Instance.driveGears.Length - 1)
                {
                    Vehicle.Instance.currentGearNum++;
                }
            }

            else if (Input.GetKeyDown(gearDown))
            {
                if (Vehicle.Instance.currentGearNum > 0)
                {
                    Vehicle.Instance.currentGearNum--;
                }
            }
        }

        else if (inputType == InputType.Handheld)
        {
            TouchTransmission();
        }
    }

    private void TouchTransmission()
    {
        if (transmissionType == TransmissionType.TiptronicTransmission)
        {
            positiveGearButton.gameObject.SetActive(true);
            negativeGearButton.gameObject.SetActive(true);

            automaticGearShifter.gameObject.SetActive(true);
            manualGearShifter.gameObject.SetActive(false);

            if (positiveGearButton.isPressed)
            {
                if (Vehicle.Instance.currentGearNum < Vehicle.Instance.driveGears.Length - 1)
                {
                    Vehicle.Instance.currentGearNum++;
                }
            }

            else if (negativeGearButton.isPressed)
            {
                if (Vehicle.Instance.currentGearNum > 0)
                {
                    Vehicle.Instance.currentGearNum--;
                }
            }
        }

        else if (transmissionType == TransmissionType.AutomaticTransmission)
        {
            positiveGearButton.gameObject.SetActive(false);
            negativeGearButton.gameObject.SetActive(false);

            automaticGearShifter.gameObject.SetActive(true);
            manualGearShifter.gameObject.SetActive(false);

            Vehicle.Instance.transmissionMode = Vehicle.TransmissionMode.Automatic;
        }

        else if (transmissionType == TransmissionType.ManualTransmission)
        {
            positiveGearButton.gameObject.SetActive(false);
            negativeGearButton.gameObject.SetActive(false);

            automaticGearShifter.gameObject.SetActive(false);
            manualGearShifter.gameObject.SetActive(true);

            Vehicle.Instance.transmissionMode = Vehicle.TransmissionMode.Manual;
        }

        else if (transmissionType == TransmissionType.SimpleTransmission)
        {
            positiveGearButton.gameObject.SetActive(false);
            negativeGearButton.gameObject.SetActive(false);

            automaticGearShifter.gameObject.SetActive(false);
            manualGearShifter.gameObject.SetActive(false);

            Vehicle.Instance.transmissionMode = Vehicle.TransmissionMode.Automatic;
        }
    }
}
