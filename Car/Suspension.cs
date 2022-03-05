using UnityEngine;

public class Suspension : MonoBehaviour
{
	[Header("Important Variables")] 
	public float grip;
	public float suspensionLength;
	public float restLength;
	public float restHeight;
	public float springTravel;
	public float springStiffness;
	public float damperStiffness;
	public float wheelRadius;
	public LayerMask collisionDetectionLayer;
	
	public enum WheelPosition{RightWheel, LeftWheel}

	public WheelPosition wheelPosition;

	[Header("Value Variables")] public float wheelMoveSpeed = 20f;
	public float wheelGetBackSpeed = 20f;
	public float wheelAngle;
	public float steeringAngle;
	public float traction;
	public float steerSpeed = 15f;
	public float steerAngle = 37f;
	public float lastCompression;

	[Header("Assignable Variables")] public Rigidbody rb;
	public CarMovement car;
	public GameObject wheel;
	public Transform currentWheel;
	public LayerMask groundLayer;

	[Header("Information Variables")] public bool isMotorized;
	public bool isSteerable;
	public bool isGrounded;

	[Header("Don't Care About")] public float hitHeight;
	public Vector3 hitPos;
	public Vector3 hitNormal;
	public Vector3 suspensionForce;
	public Vector3 wheelVelocity;
	private int _lastSkid;
	private float _minLength;
	private float _maxLength;
	private float _lastLength;
	private float _springLength;
	private float _springVelocity;
	private float _springForce;
	private float _damperForce;
	private Vector3 _startRot;

	private void Start()
	{
		_startRot = transform.localRotation.eulerAngles;
	}

	private void LateUpdate()
	{
		if (traction > 0.05f && hitPos != Vector3.zero && isGrounded)
		{
			if ((bool) Skidmarks.Instance)
			{
				_lastSkid = Skidmarks.Instance.AddSkidMark(hitPos + rb.velocity * Time.fixedDeltaTime, hitNormal, traction * 0.9f, _lastSkid);
			}
		}
		
		else
		{
			_lastSkid = -1;
		}
	}

	private void FixedUpdate()
	{
		ApplyingSuspension();
		WheelRotationCalculation();
		
		
		
		if (isGrounded)
			Debug.DrawRay(transform.position, -gameObject.transform.up * (_maxLength + suspensionLength), Color.green);
		
		if (!isGrounded)
			Debug.DrawRay(transform.position, -gameObject.transform.up * (_maxLength + suspensionLength), Color.red);
		
		Debug.DrawRay(currentWheel.gameObject.transform.position, currentWheel.transform.right * 25f, Color.blue);
	}

	private void ApplyingSuspension()
	{
		_maxLength = restLength + springTravel;
		_minLength = restLength - springTravel;

		if (Physics.CapsuleCast(transform.position, transform.position, wheelRadius, -transform.up, out var hit, _maxLength + suspensionLength, collisionDetectionLayer))
		{
			CalculateSuspension(hit);
			
			wheelVelocity = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
			
			var pointVelocity = XZVector(rb.GetPointVelocity(hitPos));
			var lateralVelocity = Vector3.Project(pointVelocity, transform.right);
			var longitudinalVelocity = Vector3.Project(pointVelocity, transform.forward);
			
			print(longitudinalVelocity);

			var frictionForce = lateralVelocity * (rb.mass * grip);
            
			if (isGrounded)
			{
				rb.AddForceAtPosition(-frictionForce, hitPos);
				
				if (hit.rigidbody != null)
				{
					hit.rigidbody.AddForceAtPosition(-suspensionForce, hitPos);
				}
			}

			rb.AddForceAtPosition(suspensionForce, hitPos);
		}

		else
		{
			isGrounded = false;
			hitHeight = suspensionLength + restHeight;
		}
	}

	private void CalculateSuspension(RaycastHit hit)
	{
		hitPos = hit.point;
		hitNormal = hit.normal;
		hitHeight = hit.distance;
		
		isGrounded = true;
		
		_lastLength = _springLength;
		_springLength = hit.distance - suspensionLength;
		_springLength = Mathf.Clamp(_springLength, _minLength, _maxLength);
		_springVelocity = (_lastLength - _springLength) / Time.fixedDeltaTime;
		_springForce = springStiffness * (restLength - _springLength);
		_damperForce = damperStiffness * _springVelocity;

		suspensionForce = (_springForce + _damperForce) * transform.up;
	}

	private void WheelRotationCalculation()
	{
		if (isSteerable)
		{
			wheelAngle = Mathf.Lerp(wheelAngle, steeringAngle, steerSpeed * Time.deltaTime);
		}

		var transformLocalRotation = transform.localRotation;

		Vector3 wheelRot = new Vector3(gameObject.transform.localRotation.x, wheelAngle, transform.gameObject.transform.localRotation.z);

		gameObject.transform.localRotation = Quaternion.Euler(_startRot + wheelRot);
		
		transformLocalRotation.eulerAngles = _startRot + wheelRot;
	}

	private static Vector3 XZVector(Vector3 vector3)
	{
		return new Vector3(vector3.x, 0f, vector3.z);
	}
}