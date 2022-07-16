using UnityEngine;

public class CameraController : MonoBehaviour
{
	public enum CameraType
	{
		Movable,
		Static
	}

	[Header("Settings")] public bool reverseCameraRotation;
	public CameraType cameraType;
	public float cameraSmoothness;
	public float sensitivity = 50f;
	public float maximumDistance;
	
	public float minimumLock;
	public float maximumLock;

	[Header("Assignable Variables")] 
	public Vehicle vehicle;
	public Vector3 cameraRot;
	public Camera controlledCamera;
	public GameObject targetObject;

	[Header("Others")] 
	public float mouseX;
	public float mouseY;
	public float xRotation;
	public float yRotation;
	public float currentDistance;
	
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
		transform.GetChild(0).localPosition = new Vector3(0f, 0f, -currentDistance);
		
		controlledCamera.transform.position = Vector3.Lerp(controlledCamera.transform.position, transform.GetChild(0).position, Time.deltaTime * cameraSmoothness);
		controlledCamera.transform.rotation = Quaternion.Lerp(controlledCamera.transform.rotation, transform.GetChild(0).rotation, Time.deltaTime * cameraSmoothness);

		transform.GetChild(0).LookAt(targetObject.transform);

		if (cameraType == CameraType.Static)
		{
			if (vehicle.gearMode == Vehicle.GearMode.Reverse)
			{
				if (reverseCameraRotation)
				{
					currentDistance = -maximumDistance;
				}
			}

			else
			{
				currentDistance = maximumDistance;
			}
		}
	}

	private void Look()
	{
		mouseX = Input.GetAxisRaw("Mouse X");
		mouseY = Input.GetAxisRaw("Mouse Y");

		yRotation += mouseX * sensitivity * 0.01f;
		xRotation -= mouseY * sensitivity * 0.01f;

		xRotation = Mathf.Clamp(xRotation, minimumLock, maximumLock);

		cameraRot = new Vector3(xRotation, yRotation, 0f);

		transform.rotation = Quaternion.Euler(cameraRot);
	}
}