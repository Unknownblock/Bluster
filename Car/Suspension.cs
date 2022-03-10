using UnityEngine;

public class Suspension : MonoBehaviour
{
	[Header("Important Variables")] 
	public float grip;
	public float driftBalance;
	public float wheelMoveSpeed;
	public float suspensionLength;
	public float restLength;
	public float restHeight;
	public float springTravel;
	public float springStiffness;
	public float damperStiffness;
	public float wheelRadius;
	public LayerMask collisionDetectionLayer;
	
	[Header("Value Variables")]
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

	[Header("Information Variables")] 
	public bool isMotorized;
	public bool isSteerable;
	public bool isBreakable;
	public bool isGrounded;

	[Header("Don't Care About")] public GameObject hitObject;
	public Vector3 longitudinalAngularVelocity;
	public Vector3 suspensionForce;
	public Vector3 wheelVelocity;	
	public Vector3 hitPos;
	public Vector3 hitNormal;
	public Vector3 startRot;
	public float hitHeight;
	private int _lastSkid;
	private float _minLength;
	private float _maxLength;
	private float _lastLength;
	private float _springLength;
	private float _springVelocity;
	private float _springForce;
	private float _damperForce;

	private void Start()
	{
		startRot = transform.localRotation.eulerAngles;
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

	private void OnDrawGizmos()
	{
		if (isGrounded)
		{
			Gizmos.color = Color.green;
		}

		if (!isGrounded)
		{
			Gizmos.color = Color.red;
		}
		
		Gizmos.DrawSphere(hitPos, 0.1f);
		Gizmos.DrawRay(gameObject.transform.position, -transform.up * (_lastLength + suspensionLength));
	}

	private void FixedUpdate()
	{
		ApplyingSuspension();
		WheelRotationCalculation();
	}

	private void ApplyingSuspension()
	{
		_maxLength = restLength + springTravel;
		_minLength = restLength - springTravel;

		if (Physics.SphereCast(transform.position, wheelRadius, -transform.up, out var hit, _maxLength + suspensionLength, collisionDetectionLayer))
		{
			CalculateSuspension(hit);
			FrictionAndDrift();
			
			wheelVelocity = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
			
			var pointVelocity = XZVector(rb.GetPointVelocity(hitPos));
			var angularVelocity = currentWheel.GetComponent<Rigidbody>().angularVelocity;
			
			longitudinalAngularVelocity = Vector3.Project(angularVelocity, car.transform.forward);

			if (isGrounded)
			{
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

	private void FrictionAndDrift()
	{
		var pointVelocity = XZVector(rb.GetPointVelocity(hitPos));
		
		var lateralVelocity = Vector3.Project(pointVelocity, transform.right);
		
		var frictionForce = lateralVelocity * (rb.mass * grip);
		
		Vector3 localVel = transform.InverseTransformDirection(pointVelocity);

		if (localVel.x <= -25f || localVel.x >= 25f)
		{
			rb.AddForceAtPosition(-transform.up * driftBalance, gameObject.transform.position);
			print("Drift");
		}

		if (isGrounded)
		{
			rb.AddForceAtPosition(-transform.right * (localVel.x * grip), hitPos);
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
		
		Vector3 wheelRot = transform.localRotation.eulerAngles;
		wheelRot.y = wheelAngle;
		
		Quaternion currentWheelRot = currentWheel.transform.localRotation;
		
		currentWheelRot.y = 0f;
		currentWheelRot.z = 0f;

		gameObject.transform.localRotation = Quaternion.Euler(wheelRot + startRot);
		currentWheel.gameObject.transform.localRotation = currentWheelRot;
	}

	private static Vector3 XZVector(Vector3 vector3)
	{
		return new Vector3(vector3.x, 0f, vector3.z);
	}
}