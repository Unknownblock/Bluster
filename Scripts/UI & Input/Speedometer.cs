using System;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public int value;

    public float fillAmount;

    public float maxValue;

    public Image fill;
    public Image background;

    public void Update()
    {
        fill.fillAmount = 1f / maxValue * (value * fillAmount);
        background.fillAmount = fillAmount;
    }
}
