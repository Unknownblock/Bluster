using UnityEngine;

public class VehicleCollision : MonoBehaviour
{
    public float damagePerCrash;
    public int currentHealth;
    public bool isColliding;
    public LayerMask layerMask;

    private void OnCollisionEnter(Collision collision)
    {
        if ((layerMask.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            isColliding = true;
            
            currentHealth -= (int)(collision.impulse.magnitude * damagePerCrash);
            
            isColliding = false;
        }
    }
}
