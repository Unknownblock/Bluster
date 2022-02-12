using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    public GameObject target;
    public float speed;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, target.transform.rotation, Time.deltaTime * speed);
    }
}
