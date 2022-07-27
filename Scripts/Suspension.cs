using System;
using UnityEngine;

public class Suspension : MonoBehaviour
{
	public enum WheelState
	{
		Normal,
		Drifting,
		Braking,
		Slipping
	}

	public WheelState wheelState;
	
	[Header("Suspension Settings")]
	public float suspensionLength;
	public float restLength;
	public float restHeight;
	public float springTravel;
	public float springStiffness;
	public float damperStiffness;
	public float wheelRadius;

	[Header("Suspension Information")]
	public float wheelRpm;
	
	[Header("Friction Settings")] 
	public float slipRatio;
	public float slipAmount;
	public float slipFriction;
	public float normalGrip;
	public float slipGrip;
	public float grip;

	[Header("Visual Settings")]
	public float skidIntensity = 20.0f;
	public int lastSkid = -1;

	public float smokeIntensity;
	public ParticleSystem smoke;

	[Header("WheelLock Settings")] 
	public float angularDrag;
	public Vector3 wheelLock;

	[Header("Steering Settings")] 
	public float wheelAngle;
	public float steeringAngle;
	public float steerSpeed = 15f;
	public float steerAngle = 37f;

	[Header("Assignable Variables")]
	public Rigidbody vehicleRb;

	public GameObject wheelPrefab;
	
	public Transform attachedWheel;
	public LayerMask groundLayer;

	[Header("Boolean Variables")] 
	public bool isSkidding;
	public bool isMotorized;
	public bool isSteerable;
	public bool isBreakable;
	public bool isGrounded;
	public bool canRotate;

	[Header("Others")] 
	public Vector3 wheelLocalVelocity;
	private Vector3 wheelPointVelocity;
	private Vector3 startRot;
	private Vector3 frictionForce;
	public Vector3 hitPoint;
	private Vector3 hitNormal;
	private float hitHeight;
	private float _minLength;
	private float _maxLength;
	private float _lastLength;
	private float _springLength;
	private float _springVelocity;
	private float _springForce;
	private float _damperForce;

	private void Awake()
	{
		InitWheels();

		startRot = transform.localRotation.eulerAngles;
	}
	
	private void FixedUpdate()
	{
		WheelManagement();
		FrictionAndDrift();
		StateManager();
		SkidMarksManagement();
		ApplyingSuspension();

		var wheelVelocity = vehicleRb.GetPointVelocity(transform.position);
		var wheelDirectionVelocity = transform.InverseTransformDirection(wheelVelocity);

		if (canRotate && isGrounded)
		{
			var wheelRollRpm = wheelDirectionVelocity.z / wheelRadius * 60f;
			wheelRpm = wheelRollRpm;
		}

		else
		{
			wheelRpm = Mathf.Lerp(wheelRpm, 0f, angularDrag * Time.deltaTime);
		}
	}

	private void ApplyingSuspension()
	{
		_maxLength = restLength + springTravel;
		_minLength = restLength - springTravel;

		if (Physics.CapsuleCast(transform.position, transform.position, wheelRadius, -transform.up, out var hit, _maxLength + suspensionLength, groundLayer))
		{
			vehicleRb.AddForceAtPosition(CalculateSuspension(hit), hitPoint);

			if (hit.rigidbody != null)
			{
				hit.rigidbody.AddForceAtPosition(-CalculateSuspension(hit), hitPoint);
			}
		}

		else
		{
			isGrounded = false;
			hitHeight = suspensionLength + restHeight;
			hitPoint = Vector3.zero;
			hitNormal = Vector3.zero;
		}
	}

	private void StateManager()
	{
		var lateralVelocity = gameObject.transform.right * wheelLocalVelocity.x;
		
		if (isGrounded)
		{
			if (slipRatio > slipAmount && isMotorized)
			{
				wheelState = WheelState.Slipping;
			}

			else if ((InputManager.Instance.isBreaking && isBreakable) || Vehicle.Instance.gearMode == Vehicle.GearMode.Park)
			{
				wheelState = WheelState.Braking;
			}

			else if (Mathf.Abs(lateralVelocity.magnitude) > slipFriction)
			{
				wheelState = WheelState.Drifting;
			}

			else
			{
				wheelState = WheelState.Normal;
			}
		}
		
		if (wheelState == WheelState.Normal)
		{
			isSkidding = false;
			grip = normalGrip;
			canRotate = true;

			var emission = smoke.emission;

			emission.rateOverTime = 0f;

			smoke.transform.position = hitPoint;
		}
		
		else if (wheelState == WheelState.Slipping)
		{
			isSkidding = true;
			grip = slipGrip;
			canRotate = true;

			var emission = smoke.emission;
			var velocityOverLifetimeModule = smoke.velocityOverLifetime;

			emission.rateOverTime = Mathf.Abs(slipRatio) * smokeIntensity;

			smoke.transform.position = hitPoint;
		}
		
		else if (wheelState == WheelState.Drifting)
		{
			isSkidding = true;
			grip = slipGrip;
			canRotate = true;

			var emission = smoke.emission;

			emission.rateOverTime = vehicleRb.velocity.magnitude * smokeIntensity;

			smoke.transform.position = hitPoint;
		}

		else if (wheelState == WheelState.Braking)
		{
			isSkidding = true;
			grip = slipGrip;
			canRotate = false;

			var emission = smoke.emission;

			emission.rateOverTime = vehicleRb.velocity.magnitude * smokeIntensity;

			smoke.transform.position = hitPoint;
		}
	}
	
	public void Brake()
	{
		if (isGrounded)
		{
			vehicleRb.AddForceAtPosition(transform.forward * (Time.deltaTime * (-wheelLocalVelocity.z)) * Vehicle.Instance.brakeForce, hitPoint, ForceMode.Force);
		}
	}

	private void FrictionAndDrift()
	{
		wheelPointVelocity = XZVector(vehicleRb.GetPointVelocity(hitPoint));
		wheelLocalVelocity = transform.InverseTransformDirection(wheelPointVelocity);

		var lateralVelocity = gameObject.transform.right * wheelLocalVelocity.x;

		frictionForce = -lateralVelocity * (vehicleRb.mass * grip);
		
		if (isGrounded)
		{
			vehicleRb.AddForceAtPosition(frictionForce, hitPoint);
			
			slipRatio = (Vehicle.Instance.CalculateForce(Vehicle.Instance.engineRpm, wheelRadius, Vehicle.Instance.currentGear) / 3600f - wheelLocalVelocity.z) / wheelLocalVelocity.z;
		}
	}

	private void InitWheels()
	{
		if (attachedWheel == null)
		{
			attachedWheel = Instantiate(wheelPrefab).transform;
			attachedWheel.parent = transform;
			attachedWheel.name = attachedWheel.parent.name + " Wheel";
		}
	}

	private void WheelManagement()
	{
		var wantedWheelPos = transform.position + -gameObject.transform.up * hitHeight;
		attachedWheel.transform.position = wantedWheelPos;
		attachedWheel.localScale = Vector3.one * (wheelRadius * 2f);

		if (canRotate)
		{
			attachedWheel.Rotate(Vector3.right, wheelRpm / 60f);

			if (isMotorized)
			{
				attachedWheel.Rotate(Vector3.right, Vehicle.Instance.CalculateWheelRpm(Vehicle.Instance.engineRpm, Vehicle.Instance.currentGear));
			}
		}

		if (isSteerable)
		{
			wheelAngle = Mathf.Lerp(wheelAngle, steeringAngle, steerSpeed * Time.deltaTime);
		}

		var wheelRot = transform.localRotation.eulerAngles;
		wheelRot.y = wheelAngle;

		var currentWheelRot = attachedWheel.transform.localRotation;

		currentWheelRot.y = wheelLock.y;
		currentWheelRot.z = wheelLock.z;

		gameObject.transform.localRotation = Quaternion.Euler(wheelRot + startRot);
		attachedWheel.gameObject.transform.localRotation = currentWheelRot;
	}

	private Vector3 CalculateSuspension(RaycastHit hit)
	{
		hitPoint = hit.point;
		hitNormal = hit.normal;
		hitHeight = hit.distance;

		isGrounded = true;

		_lastLength = _springLength;
		_springLength = hit.distance - suspensionLength;
		_springLength = Mathf.Clamp(_springLength, _minLength, _maxLength);
		_springVelocity = (_lastLength - _springLength) / Time.fixedDeltaTime;
		_springForce = springStiffness * (restLength - _springLength);
		_damperForce = damperStiffness * _springVelocity;

		return (_springForce + _damperForce) * transform.up;
	}

	private static Vector3 XZVector(Vector3 vector3)
	{
		return new Vector3(vector3.x, 0f, vector3.z);
	}
	
	private void SkidMarksManagement()
	{
		var velocity = XZVector(vehicleRb.velocity).magnitude;
		
		if (isSkidding) 
        {
	        var intensity = Mathf.Clamp01(velocity / skidIntensity);
	        var skidPoint = hitPoint + vehicleRb.velocity * Time.deltaTime;
	        lastSkid = SkidMarks.Instance.AddSkidMark(skidPoint, hitNormal, intensity, lastSkid);
        }

        else 
        {
	        lastSkid = -1;
        }
	}
}