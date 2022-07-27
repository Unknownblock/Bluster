using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Assignable Variables")]
    public Color normalRpmColor;
    public Color highRpmColor;
    
    [Header("UI Variables")]
    public TextMeshProUGUI speedIndicator;
    public TextMeshProUGUI gearIndicator;
    
    public PointerIndicator tachometer;
    public SliderIndicator tachometerSlider;
    
    private void FixedUpdate()
    {
        UIManagement();
    }

    private void UIManagement()
    {
        if (Vehicle.Instance.engineRpm >= Vehicle.Instance.maxRpm)
        {
            tachometerSlider.fill.color = highRpmColor;
            speedIndicator.color = highRpmColor;
            gearIndicator.color = highRpmColor;
        }

        else
        {
            tachometerSlider.fill.color = normalRpmColor;
            speedIndicator.color = normalRpmColor;
            gearIndicator.color = normalRpmColor;
        }
        
        tachometer.value = (int)Vehicle.Instance.smoothEngineRpm;
        tachometerSlider.value = (int)Vehicle.Instance.smoothEngineRpm;
        speedIndicator.text = $"{Vehicle.Instance.vehicleSpeed:000}";

        if (Vehicle.Instance.gearMode is Vehicle.GearMode.Drive or Vehicle.GearMode.Reverse)
        {
            gearIndicator.text = Vehicle.Instance.currentGear.gearName;
        }
        
        else if (Vehicle.Instance.gearMode == Vehicle.GearMode.Neutral)
        {
            gearIndicator.text = "N";
        }
        
        else if (Vehicle.Instance.gearMode == Vehicle.GearMode.Park)
        {
            gearIndicator.text = "P";
        }
    }
}
