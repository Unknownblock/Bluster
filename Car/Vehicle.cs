using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [Header("Assignable Variables")] 
    public float rollingMovementForce;
    public float airMovementForce;
    public float speedLimit;
    public float moveSpeed;
    public float engineForce;
    public float breakForce;

    public float carDrag;

    public Suspension[] wheels;

    [Header("Information Variables")] 
    public bool isGrounded;
    public bool isDetectingCollision;
    public bool isBreaking;

    public float horizontalMovement;
    public float verticalMovement;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Movement();
        WheelManaging();
        MyInput();
        CheckGrounded();
    }

    private void Movement()
    {
        Steering();
        
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);

        _rb.AddForce(transform.forward * -localVelocity.z * carDrag);

        if (!isGrounded)
        {
            _rb.AddTorque(transform.forward * (airMovementForce * horizontalMovement));
            _rb.AddTorque(gameObject.transform.right * (airMovementForce * verticalMovement));
        }

        foreach (var everyWheel in wheels)
        {
            var pointVelocity = XZVector(_rb.GetPointVelocity(everyWheel.hitPos));
            
            Vector3 wheelAngularVelocity = everyWheel.transform.InverseTransformDirection(everyWheel.wheelRigidbody.angularVelocity);
            Vector3 wheelVelocity = everyWheel.transform.InverseTransformDirection(everyWheel.wheelRigidbody.velocity);
            Vector3 vehicleWheelVelocity = everyWheel.transform.InverseTransformDirection(pointVelocity);
            
            moveSpeed = wheelVelocity.z;

            if (everyWheel.isGrounded && _rb.velocity.magnitude < speedLimit)
            {
                _rb.AddForceAtPosition(everyWheel.transform.forward * moveSpeed, everyWheel.hitPos);
            }
            
            if (everyWheel.isMotorized)
            {
                everyWheel.wheelRigidbody.AddTorque(everyWheel.currentWheel.right * (engineForce * verticalMovement));
            }

            if (everyWheel.isBreakable && everyWheel.isGrounded && isBreaking)
            {
                Vector3 wheelLocalVelocity = transform.InverseTransformDirection(_rb.GetPointVelocity(everyWheel.transform.position));

                _rb.AddForce(everyWheel.transform.forward * -wheelLocalVelocity.z * breakForce);
                everyWheel.wheelRigidbody.AddTorque(everyWheel.currentWheel.right * (-wheelAngularVelocity.x));
                
                everyWheel.Drift();
            }

            else
            {
                everyWheel.EndDrift();
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

    private Vector3 XZVector(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
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