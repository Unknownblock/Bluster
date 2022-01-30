using UnityEngine;

public class RayCast : MonoBehaviour
{
    public float radius;
    public float distance;
    public GameObject indicator;
    public Material mat;
    public LayerMask mask;

    private void Update()
    {
        if (Physics.SphereCast(transform.position, radius, transform.forward, out var hit, distance, mask, QueryTriggerInteraction.UseGlobal))
        {
            indicator.SetActive(true);
            indicator.transform.position = hit.transform.position;
            indicator.transform.LookAt(2f * transform.position - PlayerMovement.Instance.transform.position); //Look At Player
        }

        if (!Physics.SphereCast(transform.position, radius, transform.forward, out hit, distance, mask,
                QueryTriggerInteraction.UseGlobal))
        {
            indicator.SetActive(false);
        }
    }
}
