using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotationAmount;
    
    public void Update()
    {
        transform.GetComponent<Rigidbody>().AddTorque(Vector3.up * rotationAmount);
    }
}
