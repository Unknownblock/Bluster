using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    public string weaponName;
    
    public float pickUpRadius;
    
    public LayerMask playerMask;
    
    public void Update()
    {
        Collider[] results = Physics.OverlapSphere(gameObject.transform.position, pickUpRadius, playerMask);

        if (results.Length > 0)
        {
            foreach (var everyWeapon in WeaponContainer.Instance.weapons)
            {
                if (everyWeapon.name == weaponName)
                {
                    everyWeapon.haveTheWeapon = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}
