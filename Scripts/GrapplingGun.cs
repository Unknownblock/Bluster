using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    public GameObject grapplingHook;
    public GameObject shootPoint;
    public GameObject currentGrapplingHook;
    public float shootForce;
    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;
    public bool didShoot;

    private void FixedUpdate()
    {
        if (currentGrapplingHook != null)
        {
            shootPoint.GetComponent<LineRenderer>().SetPosition(0, currentGrapplingHook.transform.position);
            shootPoint.GetComponent<LineRenderer>().SetPosition(1, shootPoint.gameObject.transform.position);

            if (currentGrapplingHook.GetComponent<SpringJoint>() != null)
            {
                //Adjust these values to fit your game.
                PlayerMovement.Instance.GetComponent<SpringJoint>().spring = spring;
                PlayerMovement.Instance.GetComponent<SpringJoint>().damper = damper;
                PlayerMovement.Instance.GetComponent<SpringJoint>().massScale = massScale;
            }
        }
        
        if (Input.GetKeyDown(InputManager.Instance.shootKey) && !didShoot)
        {
            ShootGrapple();
        }
        
        if (Input.GetKeyDown(InputManager.Instance.interactKey) && didShoot)
        {
            GetGrappleHookBack();
        }
    }

    private void ShootGrapple()
    {
        didShoot = true;

        GameObject spawnedGrapple = Instantiate(grapplingHook, shootPoint.transform.position, Quaternion.identity);
        
        spawnedGrapple.GetComponent<Rigidbody>().AddForce(shootPoint.transform.forward * shootForce);

        currentGrapplingHook = spawnedGrapple;
    }

    private void GetGrappleHookBack()
    {
        Destroy(PlayerMovement.Instance.GetComponent<SpringJoint>());
        Destroy(currentGrapplingHook);

        didShoot = false;
    }
}
