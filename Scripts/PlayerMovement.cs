using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Assignable")]
	public PlayerHealth playerHealth;

	public PlayerInput playerInput;

	public Transform playerCam;

	public Transform orientation;

	[Header("Movement")]
	public float extraGravity;

	public float moveSpeed = 12.5f;

	public float walkSpeed = 12.5f;

	public float sprintSpeed = 17.5f;

	public float movementMultiplier = 10f;

	public float airMultiplier = 0.4f;

	public float acceleration = 10f;

	[Header("Ground Check")]
	public LayerMask groundMask;

	public float maxSlopeAngle = 50f;

	public float footStepDistance;

	[Header("Bool Variables")]
	public bool isSliding;

	public bool isGrounded;

	public bool isSurf;

	public bool readyToMove;

	public bool readyToCrouch;

	public bool readyToJump;

	public bool cancellingGrounded;

	public bool cancellingSurf;

	public int surfCancel;

	public int groundCancel;

	public float delay = 5f;

	[Header("Jumping")]
	public float jumpCoolDown = 0.5f;

	public float jumpForce = 13f;

	public int jumps = 1;

	public int extraJumps;

	public int resetJumpCounter;

	private int jumpCounterResetTime = 10;

	[Header("FX")]
	public GameObject footstepFx;

	public GameObject playerJumpSmokeFx;

	public GameObject playerSmokeFx;

	public float fxDestroyTime;

	[Header("Rigidbody Drag")]
	public float groundDrag = 6f;

	public float airDrag = 2f;

	public float slideDrag = 1f;

	[Header("Crouch")]
	public Vector3 crouchScale = new Vector3(1f, 1f, 1f);

	public float crouchTilt;

	public float crouchPos = 0.65f;

	public float slideForce = 500f;

	[Header("Fall Damage")]
	public float fallSpeed;

	public float offSetPos;

	public float fallDamageVel;

	[Header("Others")]
	private Vector3 moveDirection;

	private Vector3 slopeMoveDirection;

	private Vector3 hitPoint;

	private Vector3 normalVector;

	private Vector3 playerScale;

	private Vector3 velocity;

	private Rigidbody rb;

	public static PlayerMovement Instance { get; set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		playerScale = transform.localScale;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		rb.freezeRotation = true;
	}

	private void Update()
	{
		ControlDrag();
		velocity = rb.velocity;
		if (isGrounded)
		{
			rb.WakeUp();
		}
		moveDirection = playerInput.moveDirection;
	}

	private void LateUpdate()
	{
		fallSpeed = rb.velocity.y;
	}

	private void FixedUpdate()
	{
		MovePlayer();
	}

	private void FootSteps()
	{
		if (!isSliding && isGrounded)
		{
			float num = rb.velocity.magnitude;
			if (num > 20f)
			{
				num = 20f;
			}
			footStepDistance += num;
			if (footStepDistance > 300f)
			{
				footStepDistance = 0f;
				Destroy(Instantiate(footstepFx, hitPoint, Quaternion.identity), fxDestroyTime);
			}
		}
	}

	private void StartCrouch()
	{
		if (!isSliding && readyToCrouch)
		{
			isSliding = true;
			readyToMove = false;
			transform.localScale = crouchScale;
			transform.position = new Vector3(gameObject.transform.position.x, transform.position.y - crouchPos, gameObject.transform.transform.position.z);
			if (!(rb.velocity.magnitude <= 0.5) && isGrounded)
			{
				rb.AddForce(orientation.transform.forward * slideForce);
				SlideAudio.Instance.PlayStartSlide();
			}
		}
	}

	private void StopCrouch()
	{
		if (isSliding)
		{
			isSliding = false;
			readyToMove = true;
			transform.localScale = playerScale;
			transform.position = new Vector3(gameObject.transform.position.x, transform.position.y + crouchPos, gameObject.transform.gameObject.transform.position.z);
		}
	}

	private void ControlDrag()
	{
		if (isSliding)
		{
			rb.AddForce(new Vector3(0f - velocity.x * slideDrag, 0f, 0f - velocity.z * slideDrag));
		}
		else if (isGrounded)
		{
			rb.AddForce(new Vector3(0f - velocity.x * groundDrag, 0f, 0f - velocity.z * groundDrag));
		}
		else if (!isGrounded)
		{
			rb.drag = airDrag;
		}
	}

	private void MovePlayer()
	{
		UpdateCollisionChecks();
		CheckInput();
		FootSteps();
		if (isGrounded)
		{
			Invoke(nameof(JumpCooldown), jumpCoolDown);
		}
		if (!isGrounded)
		{
			rb.AddForce(Vector3.down * extraGravity);
		}
		if (playerInput.jumpInput)
		{
			Jump();
		}
		if (playerInput.crouchInput && isGrounded && readyToJump)
		{
			rb.AddForce(Vector3.down * 60f);
		}
		if (isGrounded && readyToMove)
		{
			rb.AddForce(moveDirection.normalized * (moveSpeed * movementMultiplier), ForceMode.Acceleration);
		}
		else if (isGrounded && isSurf && readyToMove)
		{
			rb.AddForce(slopeMoveDirection.normalized * (moveSpeed * movementMultiplier), ForceMode.Force);
		}
		else if (!isGrounded && readyToMove)
		{
			rb.AddForce(moveDirection.normalized * (moveSpeed * movementMultiplier * airMultiplier), ForceMode.Acceleration);
		}
		if (!readyToJump)
		{
			resetJumpCounter++;
			if (resetJumpCounter >= jumpCounterResetTime)
			{
				ResetJump();
			}
		}
	}

	private void CheckInput()
	{
		if (playerInput.crouchInput && !isSliding)
		{
			StartCrouch();
		}
		if (!playerInput.crouchInput && isSliding)
		{
			StopCrouch();
		}
		if (playerInput.sprintInput && isGrounded)
		{
			moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
		}
		else
		{
			moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
		}
	}

	public Vector2 FindVelRelativeToLook()
	{
		float num = Mathf.DeltaAngle(orientation.transform.eulerAngles.y, Mathf.Atan2(rb.velocity.x, velocity.z) * 57.29578f);
		float num2 = 90f - num;
		double num3 = new Vector2(rb.velocity.x, velocity.z).magnitude;
		return new Vector2(y: (float)num3 * Mathf.Cos(num * ((float)Math.PI / 180f)), x: (float)num3 * Mathf.Cos(num2 * ((float)Math.PI / 180f)));
	}

	private void ResetJump()
	{
		readyToJump = true;
		CancelInvoke(nameof(JumpCooldown));
	}

	private void Jump()
	{
		if ((isGrounded || isSurf || (!isGrounded && jumps > 0)) && readyToJump)
		{
			if (isGrounded)
			{
				jumps = extraJumps;
			}
			rb.isKinematic = false;
			if (!isGrounded)
			{
				jumps--;
			}
			readyToJump = false;
			CancelInvoke(nameof(JumpCooldown));
			Invoke(nameof(JumpCooldown), 0.25f);
			resetJumpCounter = 0;
			rb.AddForce(Vector3.up * (jumpForce * 1.5f), ForceMode.Impulse);
			rb.AddForce(normalVector * (jumpForce * 0.5f), ForceMode.Impulse);
			if (rb.velocity.y < 0.5)
			{
				rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
			}
			else if (rb.velocity.y > 0.0)
			{
				rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
			}
			ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = Instantiate(playerJumpSmokeFx, transform.position, Quaternion.LookRotation(Vector3.up)).GetComponent<ParticleSystem>().velocityOverLifetime;
			velocityOverLifetime.x = velocity.x * 2f;
			velocityOverLifetime.z = velocity.z * 2f;
		}
	}

	private void JumpCooldown()
	{
		readyToJump = true;
	}

	private bool IsFloor(Vector3 v)
	{
		return Vector3.Angle(Vector3.up, v) < maxSlopeAngle;
	}

	private bool IsSurf(Vector3 v)
	{
		float num = Vector3.Angle(Vector3.up, v);
		if (num < 89.0)
		{
			return num > maxSlopeAngle;
		}
		return false;
	}

	private void OnCollisionEnter(Collision other)
	{
		int layer = other.gameObject.layer;
		Vector3 normal = other.contacts[0].normal;
		if (groundMask == (groundMask | (1 << layer)) && IsFloor(normal))
		{
			MoveCamera.Instance.BobOnce(new Vector3(0f, fallSpeed, 0f), offSetPos);
			hitPoint = other.contacts[0].point;
			GameObject obj = Instantiate(playerSmokeFx, hitPoint, Quaternion.LookRotation(transform.position - hitPoint));
			ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = obj.GetComponent<ParticleSystem>().velocityOverLifetime;
			velocityOverLifetime.x = velocity.x * 2f;
			velocityOverLifetime.z = velocity.z * 2f;
			Destroy(obj, fxDestroyTime);
			if (fallSpeed <= fallDamageVel)
			{
				playerHealth.Damage((int)(0f - fallSpeed), other.gameObject);
			}
		}
	}

	private void OnCollisionStay(Collision other)
	{
		int layer = other.gameObject.layer;
		if ((groundMask.value & (1 << layer)) != 1 << layer)
		{
			return;
		}
		for (int i = 0; i < other.contactCount; i++)
		{
			Vector3 normal = other.contacts[i].normal;
			normal = new Vector3(normal.x, Mathf.Abs(normal.y), normal.z);
			if (IsFloor(normal))
			{
				hitPoint = other.contacts[0].point;
				isSurf = Vector3.Angle(Vector3.up, normal) > 1.0;
				isGrounded = true;
				normalVector = normal;
				cancellingGrounded = false;
				slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, other.contacts[0].normal);
			}
			if (IsSurf(normal))
			{
				cancellingSurf = false;
			}
		}
	}

	private void UpdateCollisionChecks()
	{
		if (!cancellingGrounded)
		{
			cancellingGrounded = true;
		}
		else
		{
			groundCancel++;
			if (groundCancel > (double)delay)
			{
				StopGrounded();
			}
		}
		if (!cancellingSurf)
		{
			cancellingSurf = true;
			surfCancel = 1;
			return;
		}
		surfCancel++;
		if (!(surfCancel <= (double)delay))
		{
			StopSurf();
		}
	}

	private void StopGrounded()
	{
		isGrounded = false;
	}

	private void StopSurf()
	{
		isSurf = false;
	}

	public Vector3 GetVelocity()
	{
		return rb.velocity;
	}

	public Rigidbody GetRb()
	{
		return rb;
	}
}
