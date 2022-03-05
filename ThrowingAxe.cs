using UnityEngine;

public class ThrowingAxe : MonoBehaviour
{
    public Axe currentAxe;
    public GameObject throwPoint;
    public float throwForce;
    
    private void Update()
    {
        if (Input.GetKeyDown(InputManager.Instance.shootKey))
        {
            Throw();
        }
        
        if (Input.GetKeyDown(InputManager.Instance.interactKey))
        {
            GettingBack();
        }
    }

    private void Throw()
    {
        currentAxe = GetComponentInChildren<Axe>();
        currentAxe.transform.parent = null;

        currentAxe.axeState = Axe.AxeState.Thrown;
        currentAxe.rb.useGravity = true;
        currentAxe.rb.constraints = RigidbodyConstraints.None;
        currentAxe.rb.AddForce(throwPoint.transform.forward * throwForce);
    }

    private void GettingBack()
    {
        currentAxe.axeState = Axe.AxeState.GettingBack;
    }
    
    private void GetBack()
    {
        currentAxe.transform.parent = transform;

        currentAxe.axeState = Axe.AxeState.Static;
        currentAxe.gameObject.transform.localPosition = Vector3.zero;
        currentAxe.transform.localRotation = Quaternion.Euler(Vector3.zero);
        
        currentAxe.rb.useGravity = false;
        currentAxe.rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
