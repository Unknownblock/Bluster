
using UnityEngine;
using UnityEngine.UI;

public class SliderIndicator : MonoBehaviour
{
    public int value;

    public float fillAmount;

    public float maxValue;

    public Image fill;

    public void Update()
    {
        fill.fillAmount = 1f / maxValue * (value * fillAmount);
    }
}