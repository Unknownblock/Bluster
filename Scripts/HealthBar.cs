using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Slider slider;

	public void SetHealth(float health)
	{
		//Setting The Sliders Value To The Health
		slider.value = health;
	}
}
