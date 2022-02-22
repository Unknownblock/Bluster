using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    public string weaponName;

    public int weaponId;

    public void PickUp()
    {
        foreach (var everyWeapon in WeaponContainer.Instance.weapons)
        {
            if (everyWeapon.id == weaponId)
            {
                everyWeapon.isAvailable = true;
                Destroy(gameObject);
            }
        }
    }
}
