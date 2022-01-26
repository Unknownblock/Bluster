using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    public GameObject target;
    
    private void Update()
    {
        transform.position = target.transform.position;
    }
}
