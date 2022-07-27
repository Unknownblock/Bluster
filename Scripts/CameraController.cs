using UnityEngine;

public class CameraController : MonoBehaviour
{
	public enum CameraType
	{
		Movable,
		Static
	}

	[Header("Settings")]
	public CameraType cameraType;
	public bool reverseCameraRotation;
	public float cameraSmoothness;
	public float sensitivity = 50f;
	public float cameraHeight;
	public float currentDistance;
	public float maximumDistance;
	
	public float minimumLock;
	public float maximumLock;

	[Header("Assignable Variables")]
	public Vector3 cameraRot;
	public Camera controlledCamera;

	[Header("Others")] 
	public float xRotation;
	public float yRotation;
	
	private void Update()
	{
		MainCameraSettings();
		
		if (cameraType == CameraType.Movable)
		{
			Look();
		}
	}

	private void MainCameraSettings()
	{
		controlledCamera.transform.localPosition = new Vector3(0f, 0f, -currentDistance);

		var addedHeight = new Vector3(0f, cameraHeight, 0f);
		
		transform.position = Vector3.Lerp(transform.position, Vehicle.Instance.transform.position + addedHeight, Time.smoothDeltaTime * cameraSmoothness);
	
		if (cameraType == CameraType.Static)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, Vehicle.Instance.transform.rotation, Time.smoothDeltaTime * cameraSmoothness);
		}
	}

	private void Look()
	{
		yRotation += InputManager.Instance.mouseX * sensitivity * 0.01f;
		xRotation -= InputManager.Instance.mouseY * sensitivity * 0.01f;

		xRotation = Mathf.Clamp(xRotation, minimumLock, maximumLock);

		cameraRot = new Vector3(xRotation, yRotation, 0f);
		
		transform.rotation = Quaternion.Euler(cameraRot);

		currentDistance = maximumDistance;
	}
}