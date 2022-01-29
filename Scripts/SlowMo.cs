using UnityEngine;

public class SlowMo : MonoBehaviour
{
	public float slowMotionAmount;

	private float startTime;

	private void Start()
	{
		startTime = Time.timeScale;
	}

	private void Update()
	{
		if (Input.GetKeyDown(InputManager.Instance.interactKey))
		{
			Time.timeScale = slowMotionAmount;
		}
		if (Input.GetKeyUp(InputManager.Instance.interactKey))
		{
			Time.timeScale = startTime;
		}
	}
}
