using System.Collections;
using TMPro;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public enum TransmissionMode
    {
        Automatic,
        Manual
    }
    
    public enum GearMode
    {
        Park,
        Reverse,
        Neutral,
        Drive
    }

    [Header("Vehicle Settings")] public bool startedTheVehicle;
    public TransmissionMode transmissionMode;
    public GearMode gearMode;
    
    public float maxRpm;
    public float airResistance;
    public float brakeForce;

    [Header("Vehicle Information")]
    public bool isShiftingGear;
    
    public int vehicleSpeed;
    public float engineRpm;
    public float smoothEngineRpm;
    
    [Header("Audio Management")] 
    public AudioSource engineSound;
    
    [Header("UI Variables")]
    public TextMeshProUGUI speedIndicator;
    public TextMeshProUGUI gearIndicator;
    
    public Speedometer tachometer;
    public Speedometer speedometer;

    [Header("Gear Management")]
    public Gear[] driveGears;
    public Gear reverseGear;
    public Suspension[] wheels;

    public float differentialGearRatio;
    public float gearShiftDelay;

    public Gear currentGear;
    public int currentGearNum;
    
    [Header("Input Management")]
    public float horizontalMovement;
    public float verticalMovement;
    
    public bool isBreaking;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.sleepThreshold = 0f;
    }

    private void FixedUpdate()
    {
        VehicleMovement();
        RpmManagement();
        GearManagement();
        UIManagement();
        AudioManagement();

        vehicleSpeed = (int)(XZVector(_rb.velocity).magnitude * 3.6f);
        
        smoothEngineRpm = Mathf.Lerp(smoothEngineRpm, engineRpm, 0.05f);
    }

    private void AudioManagement()
    {
        engineSound.pitch = smoothEngineRpm / maxRpm * 3f;
    }

    private void UIManagement()
    {
        speedIndicator.text = vehicleSpeed.ToString();
        tachometer.value = (int)smoothEngineRpm;    
        speedometer.value = vehicleSpeed;
        
        if (gearMode is GearMode.Drive or GearMode.Reverse)
        {
            gearIndicator.text = currentGear.gearName;
        }
        
        else if (gearMode == GearMode.Neutral)
        {
            gearIndicator.text = "N";
        }
        
        else if (gearMode == GearMode.Park)
        {
            gearIndicator.text = "P";
        }
    }

    private void RpmManagement()
    {
        foreach (var everyWheel in wheels)
        {
            var wheelEngineRpm = CalculateEngineRpm(everyWheel.wheelRpm, currentGear);

            everyWheel.wheelEngineRpm = wheelEngineRpm;
            
            if (everyWheel.isMotorized)
            {
                if (isShiftingGear)
                {
                    engineRpm = Mathf.Lerp(engineRpm, wheelEngineRpm, 250f);
                }

                else if (verticalMovement != 0f)
                {
                    if (startedTheVehicle)
                    {
                        engineRpm += currentGear.rpmIncrease;
                    }
                }
                
                engineRpm = Mathf.Lerp(engineRpm, wheelEngineRpm, 0.01f);
            }
        }

        if (engineRpm > maxRpm + 1000f)
        {
            engineRpm = Mathf.Lerp(engineRpm, 0f, 0.1f);
        }
        
        if (startedTheVehicle && verticalMovement == 0f)
        {
            engineRpm = Mathf.Lerp(engineRpm, 1500f, 0.25f);
        }
    }

    private void GearManagement()
    {
        if (gearMode == GearMode.Drive && !isShiftingGear)
        {
            if (transmissionMode == TransmissionMode.Manual)
            {
                ManualTransmission();
            }

            if (transmissionMode == TransmissionMode.Automatic)
            {
                AutomaticTransmission();
            }
        }
        
        if (gearMode == GearMode.Drive)
        {
            StartCoroutine(ShiftGear(driveGears[currentGearNum]));
        }
        
        if (gearMode == GearMode.Reverse)
        {
            currentGearNum = 0;
            
            StartCoroutine(ShiftGear(reverseGear));
        }
    }

    private void ManualTransmission()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentGearNum < driveGears.Length - 1)
            {
                currentGearNum++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentGearNum > 0)
            {
                currentGearNum--;
            }
        }
    }

    private void AutomaticTransmission()
    {
        if (engineRpm <= currentGear.minimumRpm)
        {
            if (currentGearNum > 0)
            {
                currentGearNum--;
            }
        }
        
        else if (engineRpm >= currentGear.maximumRpm)
        {
            if (currentGearNum < driveGears.Length - 1)
            {
                currentGearNum++;
            }
        }
    }

    private IEnumerator ShiftGear(Gear nextGear)
    {
        if (currentGear != nextGear)
        {
            if (!isShiftingGear)
            {
                isShiftingGear = true;

                currentGear = nextGear;

                yield return new WaitForSeconds(gearShiftDelay);

                isShiftingGear = false;
            }
        }
    }

    private void FrictionAndForces()
    {
        var localVelocity = transform.InverseTransformDirection(_rb.velocity);
        
        if (verticalMovement == 0f || !isShiftingGear || isBreaking)
        {
            _rb.AddForce(gameObject.transform.forward * (-localVelocity.z * 0.5f), ForceMode.Force);
        }
        
        if (verticalMovement != 0f && !isShiftingGear && !isBreaking)
        {
            _rb.AddForce(gameObject.transform.forward * (-localVelocity.z * airResistance), ForceMode.Force);
        }
    }

    private void VehicleMovement()
    {
        VehicleSteering();
        FrictionAndForces();

        foreach (var everyWheel in wheels)
        {
            if (gearMode == GearMode.Park)
            {
                everyWheel.WheelBrake();
            }

            if (isBreaking)
            {
                if (everyWheel.isBreakable)
                {
                    everyWheel.WheelBrake();
                }
            }

            if (everyWheel.isMotorized && everyWheel.isGrounded)
            {
                if (!isShiftingGear && !isBreaking)
                {
                    if (gearMode == GearMode.Drive)
                    {
                        var force = everyWheel.transform.forward * CalculateForce(engineRpm, everyWheel.wheelRadius, currentGear) / 3600f;
                        _rb.AddForceAtPosition(force, everyWheel.hitPoint, ForceMode.Force);
                    }
                    
                    if (gearMode == GearMode.Reverse)
                    {
                        var force = everyWheel.transform.forward * CalculateForce(engineRpm, everyWheel.wheelRadius, reverseGear) / 3600f;
                        _rb.AddForceAtPosition(force, everyWheel.hitPoint, ForceMode.Force);
                    }
                }
            }
        }
    }

    private void VehicleSteering()
    {
        foreach (var everyWheel in wheels)
        {
            if (everyWheel.isSteerable)
            {
                everyWheel.steeringAngle = everyWheel.steerAngle * horizontalMovement;
            }
        }
    }

    private static Vector3 XZVector(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
    }
    
    public float CalculateForce(float inputRpm, float wheelRadius, Gear gear)
    {
        var wheelPerimeter = wheelRadius * 2f * Mathf.PI;

        var calculatedForce = wheelPerimeter * inputRpm * 60f / gear.gearRatio / differentialGearRatio * verticalMovement;

        return calculatedForce;
    }
    
    public float CalculateWheelRpm(float inputRpm, Gear gear)
    {
        var calculatedWheelRpm = verticalMovement * inputRpm * 60f / gear.gearRatio / differentialGearRatio;

        return calculatedWheelRpm;
    }
    
    private float CalculateEngineRpm(float inputWheelRpm, Gear gear)
    {
        var calculatedEngineRpm = inputWheelRpm * gear.gearRatio * differentialGearRatio;

        return calculatedEngineRpm;
    }
}