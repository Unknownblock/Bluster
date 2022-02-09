using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public Suspension[] wheelPositions;

    public GameObject wheel;
    
    private float dir;

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
        dir = Mathf.Sign(transform.InverseTransformDirection(vector).z);
    }
    
    private void Update()
    {
        MoveWheels();
    }

    private void InitWheels()
    {
        Suspension[] array = wheelPositions;
        
        foreach (Suspension suspension in array)
        {
            suspension.currentWheel = Instantiate(wheel).transform;
            suspension.currentWheel.parent = suspension.transform;
            suspension.currentWheel.transform.localPosition = Vector3.zero;
            suspension.currentWheel.gameObject.transform.localRotation = Quaternion.identity;
            suspension.currentWheel.localScale = Vector3.one * suspension.suspensionLength * 2f;
        }
    }
    
    private Vector3 XZVector(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
    }
    
    private void MoveWheels()
    {
        Suspension[] array = wheelPositions;
        foreach (Suspension obj in array)
        {
            float num = obj.suspensionLength ;
            float y = Mathf.Lerp(b: 0f - obj.hitHeight + num, a: obj.currentWheel.transform.localPosition.y, t: Time.deltaTime * 20f);
            float num2 = 0.2f * obj.suspensionLength  * 2f;
            if (obj.transform.localPosition.x < 0f)
            {
                num2 = 0f - num2;
            }
            num2 = 0f;
            obj.currentWheel.transform.localPosition = new Vector3(num2, y, 0f);
            obj.currentWheel.Rotate(Vector3.right, XZVector(_rb.velocity).magnitude * 1f * dir);
            obj.currentWheel.localScale = Vector3.one * (obj.suspensionLength * 2f);
            obj.transform.localScale = Vector3.one / transform.localScale.x;
        }
    }
}