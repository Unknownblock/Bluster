using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;

    public float portalPosition;

    private void OnCollisionEnter(Collision other)
    {
        
        Vector3 additionalPosition = linkedPortal.transform.forward * linkedPortal.portalPosition;
        other.transform.position = linkedPortal.gameObject.transform.position + additionalPosition;
    }
}
