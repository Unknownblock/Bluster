using TMPro;
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

	[Header("Current Wheel Settings")]
	public Vector3 currentWheelLock;

	[Header("Value Variables")]
	public float wheelAngle;
	public float steeringAngle;
	public float traction;
	public float steerSpeed = 15f;
	public float steerAngle = 37f;
	public float lastCompression;

	[Header("Assignable Variables")] 
	public GameObject wheel;
	public Transform currentWheel;
	public Rigidbody vehicleRb;
	public Rigidbody wheelRigidbody;
	public LayerMask groundLayer;

	[Header("Information Variables")] 
	public bool isMotorized;
	public bool isSteerable;
	public bool isBreakable;
	public bool isGrounded;

	[Header("Don't Care About")]
	public Vector3 suspensionForce;
	public Vector3 wheelVelocity;	
	public Vector3 hitPos;
	public Vector3 hitNormal;
	public float hitHeight;
	public Vector3 startRot;
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
				_lastSkid = Skidmarks.Instance.AddSkidMark(hitPos + vehicleRb.velocity * Time.fixedDeltaTime, hitNormal, traction * 0.9f, _lastSkid);
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

		if (currentWheel.GetComponent<Rigidbody>() != null)
		{
			wheelRigidbody = currentWheel.GetComponent<Rigidbody>();
		}
		
		else if (currentWheel.GetChild(0).GetComponent<Rigidbody>() != null)
		{
			wheelRigidbody = currentWheel.GetChild(0).GetComponent<Rigidbody>();
		}
	}

	private void ApplyingSuspension()
	{
		_maxLength = restLength + springTravel;
		_minLength = restLength - springTravel;

		if (Physics.CapsuleCast(transform.position, transform.position, wheelRadius, -transform.up, out var hit, _maxLength + suspensionLength, groundLayer))
		{
			CalculateSuspension(hit);
			FrictionAndDrift();
			
			wheelVelocity = transform.InverseTransformDirection(vehicleRb.GetPointVelocity(hit.point));
			
			if (isGrounded)
			{
				if (hit.rigidbody != null)
				{
					hit.rigidbody.AddForceAtPosition(-suspensionForce, hitPos);
				}
			}
			
			vehicleRb.AddForceAtPosition(suspensionForce, hitPos);
		}

		else
		{
			isGrounded = false;
			hitHeight = suspensionLength + restHeight;
		}
	}

	private void FrictionAndDrift()
	{
		var pointVelocity = XZVector(vehicleRb.GetPointVelocity(hitPos));
		
		var lateralVelocity = Vector3.Project(pointVelocity, transform.right);
		
		var frictionForce = lateralVelocity * (vehicleRb.mass * grip);
		
		Vector3 localVel = transform.InverseTransformDirection(pointVelocity);

		if (isGrounded)
		{
			vehicleRb.AddForceAtPosition(-transform.right * (localVel.x * grip), hitPos);
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
		
		currentWheelRot.y = 0f + currentWheelLock.y;
		currentWheelRot.z = 0f + currentWheelLock.z;

		gameObject.transform.localRotation = Quaternion.Euler(wheelRot + startRot);
		currentWheel.gameObject.transform.localRotation = currentWheelRot;
	}

	private static Vector3 XZVector(Vector3 vector3)
	{
		return new Vector3(vector3.x, 0f, vector3.z);
	}
}