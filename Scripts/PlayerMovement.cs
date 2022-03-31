using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Collision otherObjectCollision;
	
	[Header("Assignable")]
	public PlayerHealth playerHealth;

	public PlayerInput playerInput;

	public Transform playerCam;

	public Transform orientation;

	[Header("Movement")]
	public float extraGravity;
	public float currentMoveSpeed = 12.5f;
	public float walkSpeed = 12.5f;
	public float sprintSpeed = 17.5f;
	public float movementMultiplier = 10f;
	public float airMultiplier = 0.4f;
	public float acceleration = 10f;

	[Header("Wall Running")] 
	public LayerMask wallRunLayer;
	public float wallRunRotation;
	public float currentWallRunTilt;

	[Header("Ground Check")]
	public LayerMask groundMask;
	public float maxSlopeAngle = 50f;
	public float footStepDistance;

	[Header("Bool Variables")]
	public bool isSliding;

	public bool isWallRunning;
	public bool isGrounded;
	public bool isSurf;

	public bool readyToMove;
	public bool readyToCrouch;
	public bool readyToJump;
	
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

	public static PlayerMovement Instance { get; private set; }

	private void Awake()
	{
		//Setting This To a Singleton
		Instance = this;
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>(); //Players Rigidbody
		playerScale = transform.localScale; //Player Start Scale
		rb.freezeRotation = true; //Freezing The Rotation Of Rigidbody
	}

	private void Update()
	{
		ControlDrag();
		WallRunning();
		
		velocity = rb.velocity; //Velocity Equals Player Rigidbody Velocity 

		//Always Waking Up The Rigidbody
		rb.sleepThreshold = 0f;
		
		moveDirection = playerInput.moveDirection; //Setting PlayerMovements Move Direction
	}

	private void LateUpdate()
	{
		fallSpeed = rb.velocity.y; //Late Updates Fall Speed
	}

	private void FixedUpdate()
	{
		MovePlayer();
	}

	private void FootSteps()
	{
		if (!isSliding && isGrounded)
		{
			float velocityMagnitude = rb.velocity.magnitude;
			
			if (velocityMagnitude > 20f)
			{
				velocityMagnitude = 20f;
			}
			
			footStepDistance += velocityMagnitude;
			
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
			isSliding = true; //IsSliding Equals True
			
			readyToMove = false; //Ready To Move Equals False

			//Setting The LocalScale To Start Player Scale
			transform.localScale = crouchScale;
			
			//Setting The Crouch Position
			transform.gameObject.transform.position = new Vector3(gameObject.transform.position.x, transform.position.y - crouchPos, gameObject.transform.transform.position.z);
			
			
			//Adding Force Forward
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
			//IsSliding Equals False
			isSliding = false;
			
			//Setting The LocalScale To Start Player Scale
			transform.localScale = playerScale;

			//Ready To Move Equals True
			readyToMove = true;

			//Setting The Normal Player Position
			transform.position = new Vector3(gameObject.transform.position.x, transform.position.y + crouchPos, gameObject.transform.gameObject.transform.position.z);
		}
	}

	private void ControlDrag()
	{
		//Setting The Rigidbody Drag
		if (isGrounded)
		{
			rb.drag = airDrag;
			
			if (isSliding)
				rb.AddForce(new Vector3(-velocity.x, 0, -velocity.z) * slideDrag);

			if (!isSliding)
				rb.AddForce(new Vector3(-velocity.x, 0, -velocity.z) * groundDrag);
		}
	}

	private void MovePlayer()
	{
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
			rb.AddForce(moveDirection.normalized * (currentMoveSpeed * movementMultiplier), ForceMode.Acceleration);
		}
		
		else if (isGrounded && isSurf && readyToMove)
		{
			rb.AddForce(slopeMoveDirection.normalized * (currentMoveSpeed * movementMultiplier), ForceMode.Force);
		}
		
		else if (!isGrounded && readyToMove)
		{
			rb.AddForce(moveDirection.normalized * (currentMoveSpeed * movementMultiplier * airMultiplier), ForceMode.Acceleration);
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
			currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, sprintSpeed, acceleration * Time.deltaTime);
		}
		
		else
		{
			currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, walkSpeed, acceleration * Time.deltaTime);
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
			
			rb.drag = airDrag;
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

	private bool IsFloor(Vector3 normal)
	{
		return Vector3.Angle(Vector3.up, normal) < maxSlopeAngle;
	}
	
	private bool IsWall(Vector3 normal)
	{
		return Math.Abs(90f - Vector3.Angle(Vector3.up, normal)) < 0.1f;
	}

	private void OnCollisionEnter(Collision other)
	{
		int layer = other.gameObject.layer;
		Vector3 normal = other.contacts[0].normal;
		
		if (IsFloor(normal) && (groundMask.value & (1 << layer)) == 1 << layer)
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
				playerHealth.Damage((int)-fallSpeed, other.gameObject);
			}
		}
		
		if (IsWall(normal))
		{
			Vector3 normalized = velocity.normalized;
			Vector3 vector = transform.position + Vector3.up * 1.6f;
			
			if (!Physics.Raycast(vector, normalized, 1.3f, groundMask) && Physics.Raycast(vector + normalized * 1.3f, Vector3.down, out var hitInfo, 3f, groundMask))
			{
				Vector3 vector2 = hitInfo.point + Vector3.up * gameObject.transform.localScale.y * 0.5f;
				MoveCamera.Instance.vaultOffset += transform.position - vector2;
				gameObject.transform.position = vector2;
				rb.velocity = velocity;
			}
		}
	}

	private void WallRunning()
	{
		if (isWallRunning)
		{
			for (int i = 0; i < otherObjectCollision.contactCount; i++)
			{
				
				Vector3 hitNormal = otherObjectCollision.contacts[i].normal;
				hitNormal = new Vector3(hitNormal.x, Mathf.Abs(hitNormal.y), hitNormal.z);
				
				if (IsWall(hitNormal))
				{
					var direction = orientation.transform.InverseTransformDirection(otherObjectCollision.contacts[i].point);
					currentWallRunTilt = wallRunRotation * direction.normalized.x;
				}
			}
		}

		else
		{
			currentWallRunTilt = 0f;
		}
	}

	private void OnCollisionStay(Collision other)
	{
		int layer = other.gameObject.layer;

		otherObjectCollision = other;

		for (int i = 0; i < other.contactCount; i++)
		{
			Vector3 hitNormal = other.contacts[i].normal;
			hitNormal = new Vector3(hitNormal.x, Mathf.Abs(hitNormal.y), hitNormal.z);

			if (IsFloor(hitNormal) && (groundMask.value & (1 << layer)) == 1 << layer)
			{
				hitPoint = other.contacts[i].point;
				isSurf = Vector3.Angle(Vector3.up, hitNormal) > 1.0;
				isGrounded = true;
				normalVector = hitNormal;
				slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, other.contacts[0].normal);
			}
			
			if (IsWall(hitNormal) && (wallRunLayer.value & (1 << layer)) == 1 << layer)
			{
				isWallRunning = true;
			}
		}
	}

	private void OnCollisionExit()
	{
		isGrounded = false;
		isSurf = false;
		isWallRunning = false;
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
