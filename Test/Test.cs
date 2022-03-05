using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float force;
    public Vector3 direction;

    public void Update()
    {
        gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 1000000f;
        gameObject.GetComponent<Rigidbody>().AddTorque(direction * force);
    }
}
