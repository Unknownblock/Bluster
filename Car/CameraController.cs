﻿using UnityEngine;

public class CameraController : MonoBehaviour
{
	public GameObject playerCam;
	public Vector3 cameraRot;
	
	public float mouseX;
	public float mouseY;
	public float xRotation;
	public float yRotation;
	public float sensitivity = 50f;
	public float cameraLockRotation;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void FixedUpdate()
	{
		Look();
	}

	private void Look()
	{
		//Mouse Movement
		mouseX = Input.GetAxisRaw("Mouse X");
		mouseY = Input.GetAxisRaw("Mouse Y");

		//Mouse Movement With Sensitivity
		yRotation += mouseX * sensitivity * 0.01f;
		xRotation -= mouseY * sensitivity * 0.01f;

		//Limitations For Camera xRotation
		xRotation = Mathf.Clamp(xRotation, -cameraLockRotation, cameraLockRotation);

		//Camera Vector3 Rotation
		cameraRot = new Vector3(xRotation, yRotation, 0f);

		//Setting The Camera Rotation
 		playerCam.transform.rotation = Quaternion.Euler(cameraRot);
	}
}