using UnityEngine;

public class RayCast : MonoBehaviour
{
    public bool enableIndicator;
    public bool aimAssist;
    public float aimAssistAmount;
    public float aimAssistSpeed;
    public float radius;
    public float distance;
    public GameObject aimAssistThing;
    public GameObject indicator;
    public LayerMask aimAssistMask;
    public LayerMask indicatorMask;

    private void Update()
    {
        if (enableIndicator)
        {
            if (Physics.SphereCast(transform.position, radius, transform.forward, out var hit, distance, indicatorMask,
                    QueryTriggerInteraction.UseGlobal))
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

        if (aimAssist)
        {
            if (Physics.SphereCast(transform.position, aimAssistAmount, transform.forward, out var hit, distance, aimAssistMask, QueryTriggerInteraction.UseGlobal))
            {
                var targetRotation = Quaternion.LookRotation(PlayerMovement.Instance.transform.position - hit.transform.position);

                var rot = Quaternion.Slerp(Quaternion.LookRotation(PlayerInput.Instance.cameraRot), targetRotation, aimAssistSpeed * Time.deltaTime);

                PlayerInput.Instance.cameraRot = rot.eulerAngles;
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
