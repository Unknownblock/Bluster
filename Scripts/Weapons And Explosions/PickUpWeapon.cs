using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    public string weaponName;

    public void PickUp()
    {
        WeaponContainer.Instance.PickUp(weaponName, gameObject);
    }
}
