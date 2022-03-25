using UnityEngine;

public class AIVehicle : MonoBehaviour
{
    [Header("PathFinding")]
    public float nodeDistance;
    public int currentNode;
    public Vector3 targetPosition;
    public Transform targetObject;
    public PathFinding pathFinding;
    
    [Header("Assignable Variables")]
    public float speedLimit;
    public Vector3 moveSpeed;
    public float engineForce;
    public float breakForce;

    public float carDrag;

    public Suspension[] wheels;

    [Header("Information Variables")] 
    public bool isGrounded;
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
        PathManaging();
    }
    
    private void MyInput()
    {
        var distance = transform.position - targetPosition;

        var input = transform.InverseTransformDirection(distance);

        horizontalMovement = -input.normalized.x;
        verticalMovement = -input.normalized.z;
    }

    private void PathManaging()
    {
        if (targetObject != null)
        {
            pathFinding.targetGameObject = targetObject;
            var path = pathFinding.grid.path;

            if (Vector3.Distance(transform.position, path[currentNode].worldPosition) > nodeDistance)
            {
                if (currentNode < path.Count - 1 && currentNode > 0)
                {
                    targetPosition = path[currentNode].worldPosition;
                }
            }

            else
            {
                if (currentNode < path.Count - 1)
                {
                    currentNode++;
                    
                    if (currentNode < path.Count - 1 && currentNode > 0)
                    {
                        targetPosition = path[currentNode].worldPosition;
                    }
                }
            }
        }
    }

    private void Movement()
    {
        Steering();
        
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);

        _rb.AddForce(transform.forward * (-localVelocity.z * carDrag));

        foreach (var everyWheel in wheels)
        {
            var pointVelocity = XZVector(_rb.GetPointVelocity(everyWheel.hitPos));

            Vector3 wheelAngularVelocity = everyWheel.transform.InverseTransformDirection(everyWheel.wheelRigidbody.angularVelocity);
            Vector3 wheelVelocity = everyWheel.transform.InverseTransformDirection(everyWheel.wheelRigidbody.velocity);
            Vector3 vehicleWheelVelocity = everyWheel.transform.InverseTransformDirection(pointVelocity);

            moveSpeed = XZVector(everyWheel.wheelRigidbody.velocity);
            
            if (everyWheel.isMotorized)
            {
                everyWheel.wheelRigidbody.AddTorque(everyWheel.currentWheel.right * (engineForce * verticalMovement));
                
                if (everyWheel.isGrounded && _rb.velocity.magnitude < speedLimit)
                {
                    _rb.AddForceAtPosition(moveSpeed, everyWheel.hitPos);
                }
            }

            if (everyWheel.isGrounded)
            {
                if (everyWheel.isBreakable && isBreaking)
                {
                    Vector3 wheelLocalVelocity = transform.InverseTransformDirection(_rb.GetPointVelocity(everyWheel.transform.position));

                    _rb.AddForce(everyWheel.transform.forward * (-wheelLocalVelocity.z * breakForce));

                    everyWheel.Drift();
                }

                else if (everyWheel.normalFrictionForce.magnitude >= everyWheel.driftFriction)
                {
                    everyWheel.Drift();
                }

                else
                {
                    everyWheel.EndDrift();
                }
            }
            
            else
            {
                everyWheel.EndDrift();
            }

            if (everyWheel.isBreakable && isBreaking)
            {
                everyWheel.wheelRigidbody.angularVelocity = Vector3.zero;
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
