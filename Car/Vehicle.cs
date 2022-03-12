using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [Header("Assignable Variables")] 
    public float rollingMovementForce;
    public float airMovementForce;
    public float speedLimit;
    public float moveSpeed;
    public float engineForce;

    public Suspension[] wheels;

    [Header("Information Variables")] 
    public bool isGrounded;
    public bool isDetectingCollision;
    public bool isBreaking;

    public float horizontalMovement;
    public float verticalMovement;

    private Rigidbody _rb;

    private void Awake()
    {
        InitWheels();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Movement();
        WheelManaging();
        MyInput();
        CheckGrounded();
    }

    private void Update()
    {
        MoveWheels();
    }

    private void Movement()
    {
        Steering();

        if (!isGrounded)
        {
            _rb.AddTorque(transform.forward * (airMovementForce * horizontalMovement));
            _rb.AddTorque(gameObject.transform.right * (airMovementForce * verticalMovement));
        }

        foreach (var everyWheel in wheels)
        {
            var pointVelocity = XZVector(_rb.GetPointVelocity(everyWheel.hitPos));
            
            Vector3 wheelVelocity = everyWheel.transform.InverseTransformDirection(everyWheel.wheelRigidbody.velocity);
            Vector3 vehicleWheelVelocity = everyWheel.transform.InverseTransformDirection(pointVelocity);
            
            moveSpeed = wheelVelocity.z;

            if (everyWheel.isGrounded)
            {
                everyWheel.wheelRigidbody.AddTorque(everyWheel.currentWheel.right * (vehicleWheelVelocity.z * 2f));
                _rb.AddForceAtPosition(everyWheel.transform.forward * (moveSpeed - 1f), everyWheel.hitPos);
            }
            
            if (everyWheel.isMotorized)
            {
                everyWheel.wheelRigidbody.AddTorque(everyWheel.currentWheel.right * (engineForce * verticalMovement));
            }

            if (_rb.velocity.magnitude >= speedLimit)
            {
                _rb.AddForce(-_rb.velocity);
            }
        }
    }

    private void WheelManaging()
    {
        foreach (var everyWheel in wheels)
        {
            everyWheel.wheelRigidbody.maxAngularVelocity = engineForce * 2f;
            everyWheel.wheelRigidbody.detectCollisions = true;
            everyWheel.wheelRigidbody.sleepThreshold = 0f;
            
            _rb.sleepThreshold = 0f;
        }
    }

    public void GenerateWheels()
    {
        foreach (var everyWheel in wheels)
        {
            var rigidbodyComponent = everyWheel.currentWheel.gameObject.GetComponent<Rigidbody>();
            var colliderComponent = everyWheel.currentWheel.gameObject.GetComponent<MeshCollider>();
            
            if (rigidbodyComponent == null)
            {
                everyWheel.currentWheel.gameObject.AddComponent<Rigidbody>();
            }

            if (colliderComponent == null)
            {
                everyWheel.currentWheel.gameObject.AddComponent<MeshCollider>();
            }

            const RigidbodyConstraints rigidbodyConstraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            rigidbodyComponent.constraints = rigidbodyConstraints;

            colliderComponent.convex = true;
        }
    }

    private void MyInput()
    {
        horizontalMovement = 0f;
        verticalMovement = 0f;

        //Horizontal Movement
        if (Input.GetKey(InputManager.Instance.rightKey))
        {
            horizontalMovement += 1f;
        }

        if (Input.GetKey(InputManager.Instance.leftKey))
        {
            horizontalMovement -= 1f;
        }

        //Vertical Movement
        if (Input.GetKey(InputManager.Instance.forwardKey))
        {
            verticalMovement += 1f;
        }

        if (Input.GetKey(InputManager.Instance.backwardKey))
        {
            verticalMovement -= 1f;
        }
        
        isBreaking = Input.GetKey(InputManager.Instance.jumpKey);
    }

    private void Steering()
    {
        foreach (Suspension everyWheel in wheels)
        {
            if (everyWheel.isSteerable)
            {
                everyWheel.steeringAngle = everyWheel.steerAngle * horizontalMovement;
            }
        }
    }

    private void InitWheels()
    {
        foreach (Suspension everyWheel in wheels)
        {
            if (everyWheel.currentWheel == null)
            {
                everyWheel.currentWheel = Instantiate(everyWheel.wheel).transform;
                everyWheel.currentWheel.parent = everyWheel.transform;
                everyWheel.currentWheel.name = everyWheel.currentWheel.parent.name + " Current Wheel";
            }
        }
    }

    private Vector3 XZVector(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
    }

    private void MoveWheels()
    {
        foreach (var everyWheel in wheels)
        {
            var wantedWheelPos = everyWheel.transform.position + -everyWheel.gameObject.transform.up * (everyWheel.hitHeight + -everyWheel.wheelRadius);
            everyWheel.currentWheel.transform.position = wantedWheelPos;
            everyWheel.currentWheel.localScale = Vector3.one * (everyWheel.wheelRadius * 2f);
        }
    }

    private void CheckGrounded()
    {
        isGrounded = false;

        foreach (var everyWheel in wheels)
        {
            if (everyWheel.isGrounded)
            {
                isGrounded = true;
            }
        }
    }
}