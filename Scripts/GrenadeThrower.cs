using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public bool godMode;
    public bool canThrow;
    public bool haveAmmo;
    public float currentGrenadeAmount;
    public float throwForce;
    public GameObject grenadePrefab;
    public Gun gun;

    private void Update()
    {
        ThrowInput();
    }

    private void ThrowInput()
    {
        if (currentGrenadeAmount > 0)
        {
            haveAmmo = true;
            transform.gameObject.SetActive(true);
        }

        if (currentGrenadeAmount <= 0)
        {
            haveAmmo = false;
            currentGrenadeAmount = 0;
            transform.gameObject.SetActive(false);
        }
        
        if (godMode)
        {
            currentGrenadeAmount = int.MaxValue;
        }

        if (Input.GetKeyUp(InputManager.Instance.shootKey))
        {
            Throw();
        }
    }

    private void Throw()
    {
        if (!canThrow || !haveAmmo)
            return;

        currentGrenadeAmount--;
        
        GameObject prefab = Instantiate(grenadePrefab, transform.position, gameObject.transform.rotation);
        
        prefab.GetComponent<Rigidbody>().AddForce(MoveCamera.Instance.transform.forward * throwForce);
        
        transform.gameObject.SetActive(false);
        
        transform.gameObject.SetActive(true);
        
        gun.Shoot();
    }
}