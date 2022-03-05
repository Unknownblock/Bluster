using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public enum DriveType {FrontWheelDrive, RearWheelDrive}

    public DriveType driveType;

    [Header("Assignable Variables")] public float wheelBase;
    public float rearTrack;
    public float turnRadius;
    public float jumpForce;
    public float brakeForce;
    public float moveSpeed;
    public float turnSpeed;

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
        MoveWheels();
        Movement();
        MyInput();
        CheckGrounded();
    }

    private void Movement()
	{
        Vector3 unused1 = XZVector(_rb.velocity);
        Vector3 localVel = transform.InverseTransformDirection(XZVector(_rb.velocity));
        Vector3 unused = transform.InverseTransformDirection(XZVector(_rb.angularVelocity));
        Steering();
        
        if (isGrounded && Input.GetKeyDown(InputManager.Instance.jumpKey))
        {
            _rb.AddForce(transform.up * jumpForce);
        }

        foreach (var everyWheel in wheelPositions)
        {
            if (everyWheel.isMotorized)
            {
                
            }
            
            if (!everyWheel.isSteerable && everyWheel.isGrounded)
            {
                _rb.AddForceAtPosition(-transform.right * (turnSpeed * localVel.z * 0.001f * horizontalMovement), everyWheel.hitPos);
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

    private void Steering()
    {
        foreach (Suspension everyWheel in wheelPositions)
        {
            if (everyWheel.isSteerable)
            {
                if (horizontalMovement > 0)
                {
                    if (everyWheel.wheelPosition == Suspension.WheelPosition.LeftWheel)
                    {
                        everyWheel.steeringAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + rearTrack / 2)) * horizontalMovement;
                    }

                    if (everyWheel.wheelPosition == Suspension.WheelPosition.RightWheel)
                    {
                        everyWheel.steeringAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - rearTrack / 2)) * horizontalMovement;
                    }
                }

                else if (horizontalMovement < 0)
                {
                    if (everyWheel.wheelPosition == Suspension.WheelPosition.LeftWheel)
                    {
                        everyWheel.steeringAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - rearTrack / 2)) * horizontalMovement;
                    }

                    if (everyWheel.wheelPosition == Suspension.WheelPosition.RightWheel)
                    {
                        everyWheel.steeringAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + rearTrack / 2)) * horizontalMovement;
                    }
                }

                else
                {
                    if (everyWheel.wheelPosition == Suspension.WheelPosition.LeftWheel)
                    {
                        everyWheel.steeringAngle = 0f;
                    }

                    if (everyWheel.wheelPosition == Suspension.WheelPosition.RightWheel)
                    {
                        everyWheel.steeringAngle = 0f;
                    }
                }
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
            }
        }
    }
    
    private Vector3 XZVector(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
    }
    
    private void MoveWheels()
    {
        Vector3 velocity = XZVector(_rb.velocity);
        
        foreach (var everyWheel in wheelPositions)
        {
            float yPos = -everyWheel.hitHeight + everyWheel.wheelRadius * 1.15f;
            everyWheel.currentWheel.transform.localPosition = new Vector3(0, yPos, 0f);
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