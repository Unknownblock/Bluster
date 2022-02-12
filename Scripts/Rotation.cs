using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public void Update()
    {
        transform.Rotate(Vector3.up * 500 * Time.deltaTime);
    }
}
