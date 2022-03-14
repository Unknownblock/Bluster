using UnityEngine;

public class MoveCamera : MonoBehaviour
{
	[Header("Assignable")]
	public Transform cameraPosition;

	[Header("OffSet")]
	public Vector3 deSyncOffset;

	public Vector3 vaultOffset;

	public Vector3 bobOffset;

	[Header("Bob")]
	public Vector3 desiredBob;
	public float bobSpeed = 15f;
	public float gettingBackSpeed = 0.5f;
	public float minBob = -3;
	public float maxBob = 3;
	public float directionMultiplier = 0.15f;

	[Header("FOV")]
	public float fov;

	public static MoveCamera Instance { get; private set; }

	private void Start()
	{
		//Setting This To a Singleton
		Instance = this;
	}

	private void Update()
	{
		//Setting The Cameras FOV
		GetComponentInChildren<Camera>().fieldOfView = fov;
	}

	private void LateUpdate()
	{
		UpdateBob();
		
		//Setting Cameras Position To Wanted
		transform.position = cameraPosition.transform.position + bobOffset + deSyncOffset + vaultOffset;
		deSyncOffset = Vector3.Lerp(deSyncOffset, Vector3.zero, Time.deltaTime * 15f);
		vaultOffset = Vector3.Slerp(vaultOffset, Vector3.zero, Time.deltaTime * 7f);
	}
	
	public void BobOnce(Vector3 bobDirection, float bobMultiplier)
	{
		//Bobbing The Camera Once
		desiredBob = ClampVector(bobDirection * directionMultiplier, minBob, maxBob) * bobMultiplier;
	}

	private void UpdateBob()
	{
		//Getting The Camera Back To Its Original Position
		desiredBob = Vector3.Lerp(desiredBob, Vector3.zero, Time.deltaTime * bobSpeed * gettingBackSpeed);
		
		//Smoothing The Camera
		bobOffset = Vector3.Lerp(bobOffset, desiredBob, Time.deltaTime * bobSpeed);
	}

	private Vector3 ClampVector(Vector3 vec, float min, float max)
	{
		return new Vector3(Mathf.Clamp(vec.x, min, max), Mathf.Clamp(vec.y, min, max), Mathf.Clamp(vec.z, min, max));
	}
}
