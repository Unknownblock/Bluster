using UnityEngine;

public class Ball : MonoBehaviour
{
    public float force;
    
    private void Update()
    {
        PredictionManager.Instance.Predict(transform.gameObject, gameObject.transform.position, GetComponent<Rigidbody>().velocity);

        if (Input.GetKeyDown(InputManager.Instance.jumpKey))
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * force);
        }
    }
}
