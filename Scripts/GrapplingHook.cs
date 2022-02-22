using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public bool isLocked;
    
    private void FixedUpdate()
    {
        if (!isLocked)
        {
            transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
    
    private void OnCollisionEnter()
    {
        if (!isLocked)
        {
            isLocked = true;
            
            SpringJoint joint = PlayerMovement.Instance.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = transform.position;

            float distanceFromPoint = Vector3.Distance(PlayerMovement.Instance.transform.position, gameObject.transform.position);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnCollisionExit()
    {
        isLocked = false;
    }
}
