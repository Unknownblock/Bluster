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
		if (Input.GetButtonDown("Interact"))
		{
			Time.timeScale = slowMotionAmount;
		}
		if (Input.GetButtonUp("Interact"))
		{
			Time.timeScale = startTime;
		}
	}
}
