using UnityEngine;

public class ArrowSteering : MonoBehaviour
{
    public float steeringInput;
    public CustomButton leftArrow;
    public CustomButton rightArrow;

    private void FixedUpdate()
    {
        steeringInput = 0f;

        if (rightArrow.isPressed)
        {
            steeringInput += 1f;
        }

        if (leftArrow.isPressed)
        {
            steeringInput -= 1f;
        }
    }
}
