using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GearShifter : MonoBehaviour, IPointerDownHandler
{
    [Header("Assignable Variables")]
    public Gear[] gears;
    public Transform gearPointer;

    public Vehicle vehicle;

    public CustomButton positiveGearButton;
    public CustomButton negativeGearButton;
    
    [Header("Settings")]
    public float autoDistance;

    [Header("Information")] 
    public Vector3 pointerPosition;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        pointerPosition = new Vector3(eventData.pointerCurrentRaycast.screenPosition.x, eventData.pointerCurrentRaycast.screenPosition.y, 0f);
    }

    private void FixedUpdate()
    {
        GearShifting();
        ManualGearShifting();
    }

    private void GearShifting()
    {
        foreach (var everyGear in gears)
        {
            var distance = Vector3.Distance(pointerPosition, everyGear.gearTransform.position);

            if (distance <= autoDistance)
            {
                gearPointer.transform.position = everyGear.gearTransform.position;

                if (int.TryParse(everyGear.name, out var result))
                {
                    vehicle.gearMode = Vehicle.GearMode.Drive;
                    vehicle.currentGearNum = result - 1;
                }

                else
                {
                    if (everyGear.name is "Park" or "P")
                    {
                        vehicle.transmissionMode = Vehicle.TransmissionMode.Automatic;
                        vehicle.gearMode = Vehicle.GearMode.Park;
                    }
                    
                    else if (everyGear.name is "Reverse" or "R")
                    {
                        vehicle.gearMode = Vehicle.GearMode.Reverse;
                    }
                    
                    else if (everyGear.name is "Neutral" or "N")
                    {
                        vehicle.transmissionMode = Vehicle.TransmissionMode.Automatic;
                        vehicle.gearMode = Vehicle.GearMode.Neutral;
                    }
                    
                    else if (everyGear.name is "Drive" or "D")
                    {
                        vehicle.transmissionMode = Vehicle.TransmissionMode.Automatic;
                        vehicle.gearMode = Vehicle.GearMode.Drive;
                    }
                    
                    else if (everyGear.name is "Manual" or "M")
                    {
                        vehicle.transmissionMode = Vehicle.TransmissionMode.Manual;
                    }
                }
            }
        }
    }

    private void ManualGearShifting()
    {
        if (positiveGearButton.isPressed)
        {
            if (vehicle.currentGearNum < vehicle.driveGears.Length - 1)
            {
                vehicle.currentGearNum++;
            }
        }

        if (negativeGearButton.isPressed)
        {
            if (vehicle.currentGearNum > 0)
            {
                vehicle.currentGearNum--;
            }
        }
    }

    [Serializable]
    public class Gear
    {
        public Transform gearTransform;
        public string name;
    }
}
