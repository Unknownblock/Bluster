using UnityEngine;

public class MoveCamera : MonoBehaviour
{
	[Header("Assignable")]
	public Transform cameraPosition;

	[Header("OffSet")]
	public Vector3 deSyncOffset;

	public Vector3 vaultOffset;

	private Vector3 bobOffset;

	[Header("Bob")]
	private Vector3 desiredBob;

	private float bobSpeed = 15f;

	[Header("FOV")]
	public float startFOV;

	public float fov;

	public static MoveCamera Instance { get; private set; }

	private void Start()
	{
		Instance = this;
		startFOV = GetComponentInChildren<Camera>().fieldOfView;
	}

	private void Update()
	{
		GetComponentInChildren<Camera>().fieldOfView = fov;
	}

	private void LateUpdate()
	{
		UpdateBob();
		base.transform.position = cameraPosition.transform.position + bobOffset + deSyncOffset + vaultOffset;
		deSyncOffset = Vector3.Lerp(deSyncOffset, Vector3.zero, Time.deltaTime * 15f);
		vaultOffset = Vector3.Slerp(vaultOffset, Vector3.zero, Time.deltaTime * 7f);
	}

	public void BobOnce(Vector3 bobDirection, float bobMultiplier)
	{
		desiredBob = ClampVector(bobDirection * 0.15f, -3f, 3f) * bobMultiplier;
	}

	private void UpdateBob()
	{
		desiredBob = Vector3.Lerp(desiredBob, Vector3.zero, Time.deltaTime * bobSpeed * 0.5f);
		bobOffset = Vector3.Lerp(bobOffset, desiredBob, Time.deltaTime * bobSpeed);
	}

	private Vector3 ClampVector(Vector3 vec, float min, float max)
	{
		return new Vector3(Mathf.Clamp(vec.x, min, max), Mathf.Clamp(vec.y, min, max), Mathf.Clamp(vec.z, min, max));
	}
}
