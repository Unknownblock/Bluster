using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public TextMeshProUGUI timerLabel;

	private static float _time;

	private void OnEnable()
	{
		_time = 0f;
	}

	private void Update()
	{
		_time += Time.deltaTime;
		float num = _time / 60f;
		float num2 = _time % 60f;
		float num3 = _time * 100f % 10f;
		timerLabel.text = $"{num:00} : {num2:00} : {num3:00}";
	}
}
