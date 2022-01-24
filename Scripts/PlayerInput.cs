using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[Header("Inputs")]
	public bool jumpInput;

	public bool crouchInput;

	public bool sprintInput;

	public bool reloadInput;

	public float mouseX;

	public float mouseY;

	public float xRotation;

	public float yRotation;

	public float sensitivity = 50f;

	public float cameraLockRotation;

	public Vector3 cameraRot;

	public Vector3 currentRotation;

	[Header("Tilt")]
	public float tilt;

	public float desiredTilt;

	public float tiltSmoothTime;

	[Header("Others")]
	public Vector3 moveDirection;

	public float horizontalMovement;

	public float verticalMovement;

	public static PlayerInput Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		PlayerMovement.Instance = GetComponent<PlayerMovement>();
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		Tilt();
		Look();
		MyInput();
	}

	private void Tilt()
	{
		tilt = Mathf.Lerp(tilt, desiredTilt + currentRotation.z, Time.deltaTime * tiltSmoothTime);
		
		if (PlayerMovement.Instance.isSliding)
		{
			desiredTilt = PlayerMovement.Instance.crouchTilt;
		}
		
		else
		{
			desiredTilt = 0f;
		}
	}

	private void MyInput()
	{
		horizontalMovement = 0f;
		verticalMovement = 0f;
		
		if (Input.GetKey(InputManager.Instance.rightKey))
		{
			horizontalMovement += 1f;
		}
		
		if (Input.GetKey(InputManager.Instance.leftKey))
		{
			horizontalMovement -= 1f;
		}
		
		if (Input.GetKey(InputManager.Instance.forwardKey))
		{
			verticalMovement += 1f;
		}
		
		if (Input.GetKey(InputManager.Instance.backwardKey))
		{
			verticalMovement -= 1f;
		}
		moveDirection = PlayerMovement.Instance.orientation.forward * verticalMovement + PlayerMovement.Instance.orientation.right * horizontalMovement;
		jumpInput = Input.GetKey(InputManager.Instance.jumpKey);
		sprintInput = Input.GetKey(InputManager.Instance.sprintKey);
		crouchInput = Input.GetKey(InputManager.Instance.crouchKey);
		reloadInput = Input.GetKey(InputManager.Instance.reloadKey);
	}

	private void Look()
	{
		mouseX = Input.GetAxisRaw("Mouse X");
		mouseY = Input.GetAxisRaw("Mouse Y");
		yRotation += mouseX * sensitivity * 0.01f;
		xRotation -= mouseY * sensitivity * 0.01f;
		xRotation = Mathf.Clamp(xRotation, 0f - cameraLockRotation, cameraLockRotation);
		cameraRot = new Vector3(xRotation + currentRotation.x, yRotation + currentRotation.y, tilt);
		PlayerMovement.Instance.playerCam.transform.rotation = Quaternion.Euler(cameraRot);
		PlayerMovement.Instance.orientation.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
	}
}
