using UnityEditor;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Assignable Variables")]
    public float moveSpeed;
    public float engineForce;

    public Suspension[] wheelPositions;

    [Header("Information Variables")] 
    public bool isGrounded;
    public bool isBreaking;

    public float horizontalMovement;
    public float verticalMovement;

    private Rigidbody _rb;

    public void Print(string text)
    {
        print(text);
    }
    
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

        foreach (var everyWheel in wheelPositions)
        {
            var wheelRb = everyWheel.currentWheel.GetComponent<Rigidbody>();
            
            var pointVelocity = XZVector(_rb.GetPointVelocity(everyWheel.hitPos));
            
            Vector3 wheelVelocity = everyWheel.transform.InverseTransformDirection(pointVelocity);

            wheelRb.maxAngularVelocity = engineForce * 2f;

            moveSpeed = wheelRb.velocity.magnitude * verticalMovement;
            
            if (everyWheel.isGrounded)
            {
                _rb.AddForceAtPosition(transform.forward * (moveSpeed - 1f), everyWheel.hitPos);
            }
            
            if (everyWheel.isMotorized)
            {
                wheelRb.AddTorque(everyWheel.currentWheel.right * (verticalMovement * engineForce));
            }
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
        foreach (Suspension everyWheel in wheelPositions)
        {
            if (everyWheel.isSteerable)
            {
                everyWheel.steeringAngle = everyWheel.steerAngle * horizontalMovement;
            }
        }
    }

    private void InitWheels()
    {
        foreach (Suspension everyWheel in wheelPositions)
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
        foreach (var everyWheel in wheelPositions)
        {
            var wantedWheelPos = everyWheel.transform.position + -everyWheel.gameObject.transform.up * (everyWheel.hitHeight + -everyWheel.wheelRadius);
            everyWheel.currentWheel.transform.position = wantedWheelPos;
            everyWheel.currentWheel.localScale = Vector3.one * (everyWheel.wheelRadius * 2f);
        }
    }

    private void CheckGrounded()
    {
        isGrounded = false;

        foreach (var everyWheel in wheelPositions)
        {
            if (everyWheel.isGrounded)
            {
                isGrounded = true;
            }
        }
    }
}