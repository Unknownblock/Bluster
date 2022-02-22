using UnityEngine;

public class RayCast : MonoBehaviour
{
    public bool enableIndicator;
    public float radius;
    public float distance;
    public GameObject indicator;
    public LayerMask indicatorMask;

    private void Update()
    {
        if (enableIndicator)
        {
            if (Physics.SphereCast(transform.position, radius, transform.forward, out var hit, distance, indicatorMask, QueryTriggerInteraction.UseGlobal))
            {
                StartFade();
                indicator.transform.position = hit.transform.position;
                indicator.transform.LookAt(2f * transform.position -
                                           PlayerMovement.Instance.transform.position); //Look At Player
            }

            if (!Physics.SphereCast(transform.position, radius, transform.forward, out hit, distance, indicatorMask,
                    QueryTriggerInteraction.UseGlobal))
            {
                StopFade();
            }
        }
    }

    public void StartFade()
    {
        indicator.transform.localScale = Vector3.Lerp(indicator.transform.localScale, Vector3.one * 2, Time.deltaTime * 10);
    }
    
    public void StopFade()
    {
        indicator.transform.localScale = Vector3.Lerp(indicator.transform.localScale, Vector3.zero, Time.deltaTime * 10);
    }
}
