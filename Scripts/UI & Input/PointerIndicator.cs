using UnityEngine;

public class PointerIndicator : MonoBehaviour
{
    [Header("Settings")] 
    public float value;
    public float maxValue;
    public float rotationAmount;
    public float startRotation;

    [Header("Assignable Variables")] 
    public GameObject pointer;

    private void Update()
    {
        var rotationPerValue = rotationAmount / maxValue;
        var rotation = new Vector3(0f, 0f, rotationPerValue * value + startRotation);
        pointer.transform.rotation = Quaternion.Euler(rotation);
    }
}
