using UnityEngine;

public class CarBody : MonoBehaviour
{
    public Vehicle vehicle;
    public bool isDetectingCollision;

    private void OnCollisionStay(Collision collisionInfo)
    {
        isDetectingCollision = true;
    }
}
