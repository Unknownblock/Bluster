using UnityEngine;

public class Axe : MonoBehaviour
{
    public enum AxeState{Thrown, Static, GettingBack, Stuck}

    public AxeState axeState;
    public LayerMask hitMask;

    public float throwTorqueAmount;

    public float getBackSpeed;
    
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (axeState == AxeState.Thrown)
        {
            rb.AddTorque(transform.right * (throwTorqueAmount * rb.velocity.magnitude));
        }
        
        if (axeState == AxeState.GettingBack)
        {
            rb.AddForce((PlayerMovement.Instance.transform.position - transform.position) * getBackSpeed);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        int layer = other.gameObject.layer;
        
        if ((hitMask.value & (1 << layer)) != 1 << layer)
        {
            return;
        }

        if (axeState != AxeState.GettingBack)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            axeState = AxeState.Stuck;
        }

        else
        {
            axeState = AxeState.GettingBack;
        }
    }
}
