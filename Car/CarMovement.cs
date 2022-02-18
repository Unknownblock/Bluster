using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public enum DriveType {FrontWheelDrive, RearWheelDrive}

    public DriveType driveType;

    [Header("Assignable Variables")] 
    public float wheelMovingSpeed;
    public float brakeForce;
    public float moveSpeed;
    public float turnSpeed;
    public float grip;
    
    public Suspension[] wheelPositions;

    public bool isGrounded;
    
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
        Vector3 vector = XZVector(_rb.velocity);

        MoveWheels(Mathf.Sign(transform.InverseTransformDirection(vector).z));
        Movement();
    }
    
    private void Update()
    {
        MyInput();
        CheckGrounded();
    }
    
    private void Movement()
	{
        Vector3 vector = XZVector(_rb.velocity);
        Vector3 localVel = transform.InverseTransformDirection(XZVector(_rb.velocity));
        Vector3 localAngularVel = transform.InverseTransformDirection(XZVector(_rb.angularVelocity));
        float speed = vector.magnitude * 3.6f * Mathf.Sign(transform.InverseTransformDirection(vector).z);
        Steering(speed);
        
        print(localVel);

        if (Input.GetKey(InputManager.Instance.jumpKey))
        {
            _rb.AddForce(-_rb.velocity * brakeForce);
        }

        foreach (var everyWheel in wheelPositions)
        {
            var pointVelocity = XZVector(_rb.GetPointVelocity(everyWheel.hitPos));
            var lateralVelocity = Vector3.Project(pointVelocity, everyWheel.transform.right);

            var frictionForce = lateralVelocity;
            
            _rb.AddForceAtPosition(-frictionForce * grip, everyWheel.hitPos);
            
            if (everyWheel.isRearWheel && everyWheel.isGrounded)
            {
                _rb.AddForceAtPosition(everyWheel.transform.forward * (moveSpeed * verticalMovement), everyWheel.hitPos);
            }
            
            if (!everyWheel.isRearWheel && everyWheel.isGrounded && localVel.z > 5)
            {
                _rb.AddForceAtPosition(-everyWheel.transform.right * (turnSpeed * horizontalMovement * (localVel.z * 0.025f)), everyWheel.hitPos);
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
    }
    
    private void Steering(float speed)
    {
        foreach (Suspension everyWheel in wheelPositions)
        {
            if (!everyWheel.isRearWheel)
            {
                everyWheel.steeringAngle = horizontalMovement * (everyWheel.steerAngle - Mathf.Clamp(speed * 0.35f - 2f, 0f, 17f));
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
                everyWheel.currentWheel.transform.localPosition = Vector3.zero;
                everyWheel.currentWheel.gameObject.transform.localRotation = Quaternion.identity;
                everyWheel.currentWheel.localScale = Vector3.one * everyWheel.wheelRadius * 2f;
            }
        }
    }
    
    private Vector3 XZVector(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
    }
    
    private void MoveWheels(float direction)
    {
        foreach (Suspension everyWheel in wheelPositions)
        {
            float yPos = Mathf.Lerp(-everyWheel.hitHeight + everyWheel.wheelRadius + everyWheel.addedWheelPosition, everyWheel.currentWheel.transform.localPosition.y, Time.deltaTime * everyWheel.wheelGetBackSpeed);
            everyWheel.currentWheel.transform.localPosition = Vector3.Lerp(everyWheel.currentWheel.gameObject.transform.localPosition, new Vector3(0, yPos, 0f), Time.deltaTime * everyWheel.wheelMoveSpeed);
            everyWheel.currentWheel.Rotate(Vector3.right, XZVector(_rb.velocity).magnitude * direction);
            everyWheel.currentWheel.localScale = Vector3.one * (everyWheel.wheelRadius * 2f);
            everyWheel.transform.localScale = Vector3.one / transform.localScale.x;
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