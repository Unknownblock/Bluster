using System.Collections;
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

    [Header("Vehicle Settings")]
    public TransmissionMode transmissionMode;
    public GearMode gearMode;
    
    public float airResistance;
    public float normalAirResistance;
    public float brakeForce;
    
    [Header("Engine Settings")]
    public bool startedTheVehicle;

    public float brakeVelocity;
    public float hpRatio;
    public float engineHorsePower;
    
    public float engineStartDuration;
    public float currentEngineStartTime;
    public float maxRpm;

    [Header("Vehicle Information")]
    public bool isShiftingGear;
    
    public int vehicleSpeed;

    public float wheelEngineRpm;
    public float engineRpm;
    public float smoothEngineRpm;
    
    [Header("Audio Management")] 
    public AudioSource engineSound;

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

    public static Vehicle Instance { get; private set; }

    public Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.sleepThreshold = 0f;

        Instance = this;
    }

    private void FixedUpdate()
    {
        VehicleManagement();
        RpmManagement();
        GearManagement();
        AudioManagement();
        ManagingOthers();

        engineHorsePower = engineRpm * hpRatio;
    }

    private void ManagingOthers()
    {
        vehicleSpeed = (int)(XZVector(_rb.velocity).magnitude * 3.6f);
        
        smoothEngineRpm = Mathf.Lerp(smoothEngineRpm, engineRpm, 0.1f);
    }

    private void AudioManagement()
    {
        engineSound.pitch = smoothEngineRpm / maxRpm * 3f;
    }
    
    private void VehicleManagement()
    {
        FrictionAndForces();

        if (_rb.velocity.magnitude < 0.5f)
        {
            if (verticalMovement == 0f)
            {
                _rb.velocity = Vector3.zero;
            }
        }

        foreach (var everyWheel in wheels)
        {
            if (gearMode == GearMode.Park)
            {
                everyWheel.Brake();
            }

            else if (_rb.velocity.magnitude <= brakeVelocity && verticalMovement == 0f)
            {
                if (everyWheel.isBreakable)
                {
                    everyWheel.Brake();
                }
            }
            
            else if (isBreaking)
            {
                if (everyWheel.isBreakable)
                {
                    everyWheel.Brake();
                }
            }

            if (everyWheel.isMotorized && everyWheel.isGrounded)
            {
                if (!isShiftingGear && !isBreaking && startedTheVehicle)
                {
                    if (gearMode == GearMode.Drive)
                    {
                        var force = everyWheel.transform.forward * CalculateForce(engineRpm, everyWheel.wheelRadius, currentGear) / 3600f;
                        
                        if (_rb.velocity.magnitude < force.magnitude)
                        {
                            _rb.AddForceAtPosition(force, everyWheel.hitPoint, ForceMode.Force);
                        }
                    }
                    
                    if (gearMode == GearMode.Reverse)
                    {
                        var force = everyWheel.transform.forward * CalculateForce(engineRpm, everyWheel.wheelRadius, reverseGear) / 3600f;
                        
                        if (_rb.velocity.magnitude < force.magnitude)
                        {
                            _rb.AddForceAtPosition(force, everyWheel.hitPoint, ForceMode.Force);
                        }
                    }
                }
            }
            
            if (everyWheel.isSteerable)
            {
                everyWheel.steeringAngle = everyWheel.steerAngle * horizontalMovement;
            }

            if (verticalMovement == -1)
            {
                if (everyWheel.wheelLocalVelocity.z > brakeForce)
                {
                    if (everyWheel.isBreakable)
                    {
                        everyWheel.Brake();
                    }
                }

                if (everyWheel.wheelLocalVelocity.z <= brakeForce)
                {
                    gearMode = GearMode.Reverse;
                }
            }

            else if (verticalMovement == 1)
            {
                gearMode = GearMode.Drive;
            }
        }
    }

    private void RpmManagement()
    {
        foreach (var everyWheel in wheels)
        {
            if (engineHorsePower > 0f)
            { 
                wheelEngineRpm = CalculateEngineRpm(Mathf.Abs(everyWheel.wheelRpm), currentGear) / (engineHorsePower / 100f);
            }

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
                
                if (everyWheel.isGrounded)
                {
                    engineRpm = Mathf.Lerp(engineRpm, wheelEngineRpm, 0.005f);
                }

                if (verticalMovement == 0)
                {
                    engineRpm = Mathf.Lerp(engineRpm, wheelEngineRpm, 0.1f);
                }
            }
        }

        if (engineRpm > maxRpm + 500f)
        {
            engineRpm = Mathf.Lerp(engineRpm, 0f, 0.1f);
        }
        
        if (startedTheVehicle && verticalMovement == 0f)
        {
            engineRpm = Mathf.Lerp(engineRpm, 1000f, 0.1f);
        }
    }

    private void GearManagement()
    {
        if (gearMode == GearMode.Drive && !isShiftingGear)
        {
            if (transmissionMode == TransmissionMode.Automatic)
            {
                if (engineRpm <= currentGear.minimumRpm || wheelEngineRpm <= currentGear.minimumRpm)
                {
                    if (currentGearNum > 0)
                    {
                        currentGearNum--;
                    }
                }
            
                else if (engineRpm >= currentGear.maximumRpm && wheelEngineRpm >= currentGear.maximumRpm)
                {
                    if (currentGearNum < driveGears.Length - 1)
                    {
                        currentGearNum++;
                    }
                }
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
        foreach (var everyWheel in wheels)
        {
            if (everyWheel.isGrounded)
            {
                var localVelocity = transform.InverseTransformDirection(_rb.velocity);

                _rb.AddForce(gameObject.transform.forward * (-localVelocity.z * normalAirResistance), ForceMode.Force);
            }
        }
    }

    private static Vector3 XZVector(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
    }
    
    public float CalculateForce(float inputRpm, float wheelRadius, Gear gear)
    {
        var calculatedForce = wheelRadius * inputRpm * 60f / gear.gearRatio / differentialGearRatio * engineHorsePower / 100f * verticalMovement;

        return calculatedForce;
    }
    
    public float CalculateWheelRpm(float inputRpm, Gear gear)
    {
        var calculatedWheelRpm = inputRpm * 60f / gear.gearRatio / differentialGearRatio * verticalMovement;

        return calculatedWheelRpm;
    }
    
    private float CalculateEngineRpm(float inputWheelRpm, Gear gear)
    {
        var calculatedEngineRpm = inputWheelRpm * gear.gearRatio * differentialGearRatio;

        return calculatedEngineRpm;
    }
}