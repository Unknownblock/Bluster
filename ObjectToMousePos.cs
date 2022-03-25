using UnityEngine;

public class ObjectToMousePos : MonoBehaviour
{
    public LayerMask hitMask;
    public GameObject wantedObject;
    public Vector3 worldPosition;

    private void Update()
    {
        wantedObject.transform.position = worldPosition;

        Plane plane = new Plane(Vector3.down, 0);
        
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (plane.Raycast(ray, out var distance))
            {
                worldPosition = ray.GetPoint(distance);
            }
        }
    }
}
