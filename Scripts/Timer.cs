using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
	private TextMeshProUGUI timerLabel;

	public static float time;

	public static float bestTime;

	private void OnEnable()
	{
		time = 0f;
		timerLabel = base.gameObject.GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		time += Time.deltaTime;
		bestTime = time;
		float num = time / 60f;
		float num2 = time % 60f;
		float num3 = time * 100f % 10f;
		timerLabel.text = $"{num:00} : {num2:00} : {num3:00}";
	}
}
