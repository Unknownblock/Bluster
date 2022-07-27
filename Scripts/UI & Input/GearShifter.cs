using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GearShifter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Assignable Variables")]
    public Gear[] gears;
    public Transform gearPointer;

    public Vehicle vehicle;

    public PointerEventData pointerEventData;

    [Header("Settings")]
    public bool isHovered;
    public float autoDistance;

    [Header("Information")] 
    public Vector3 pointerPosition;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        pointerEventData = eventData;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    private void FixedUpdate()
    {
        GearShifting();
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (pointerEventData != null)
            {
                pointerPosition = new Vector3(pointerEventData.pointerCurrentRaycast.screenPosition.x, pointerEventData.pointerCurrentRaycast.screenPosition.y, 0f);
            }
        }
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

                    InputManager.Instance.transmissionType = InputManager.TransmissionType.ManualTransmission;
                }

                else
                {
                    if (everyGear.name is "Park" or "P")
                    {
                        vehicle.transmissionMode = Vehicle.TransmissionMode.Automatic;
                        vehicle.gearMode = Vehicle.GearMode.Park;
                        InputManager.Instance.transmissionType = InputManager.TransmissionType.AutomaticTransmission;
                    }
                    
                    else if (everyGear.name is "Reverse" or "R")
                    {
                        vehicle.gearMode = Vehicle.GearMode.Reverse;
                        InputManager.Instance.transmissionType = InputManager.TransmissionType.AutomaticTransmission;
                    }
                    
                    else if (everyGear.name is "Neutral" or "N")
                    {
                        vehicle.transmissionMode = Vehicle.TransmissionMode.Automatic;
                        vehicle.gearMode = Vehicle.GearMode.Neutral;
                        InputManager.Instance.transmissionType = InputManager.TransmissionType.AutomaticTransmission;
                    }
                    
                    else if (everyGear.name is "Drive" or "D")
                    {
                        vehicle.transmissionMode = Vehicle.TransmissionMode.Automatic;
                        vehicle.gearMode = Vehicle.GearMode.Drive;
                        InputManager.Instance.transmissionType = InputManager.TransmissionType.AutomaticTransmission;
                    }
                    
                    else if (everyGear.name is "Manual" or "M")
                    {
                        InputManager.Instance.transmissionType = InputManager.TransmissionType.TiptronicTransmission;
                    }
                }
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
