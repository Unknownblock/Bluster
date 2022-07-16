using System.Collections.Generic;
using UnityEngine;

public class IndicatorManagement : MonoBehaviour
{
    public Indicator indicator;
    public List<Indicator> indicators;
    public int[] nums;
    public int numberOfIndicators;
    public float distance;
    public float startRotation;

    private void Update()
    {
        for (var i = 0; i < numberOfIndicators - indicators.Count; i++)
        {
            indicators.Add(Instantiate(indicator, transform.position, gameObject.transform.rotation, transform));
        }

        for (var i = 0; i < indicators.Count; i++)
        {
            var value = nums[i];
            var indicatorRotation = new Vector3(0f, 0f, -distance * i + -startRotation);
            var textRotation = new Vector3(0f, 0f, distance * i + startRotation);
            
            indicators[i].transform.localRotation = Quaternion.Euler(indicatorRotation);
            indicators[i].transform.GetChild(0).localRotation = Quaternion.Euler(textRotation);
            indicators[i].textMeshProUGUI.text = value.ToString();
            indicators[i].transform.name = value.ToString();
        }
    }
}
