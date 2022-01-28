using System;
using UnityEngine;

[Serializable]
public class Weapon
{
    public string name;

    public GameObject weaponMesh;
        
    public enum WeaponState {Selected, NotSelected}

    public WeaponState weaponState;
}

public class WeaponContainer : MonoBehaviour
{
    public int currentSelected;
    
    public Weapon[] weapons;

    public void Update()
    {
        currentSelected += (int)Input.mouseScrollDelta.y;

        if (currentSelected > weapons.Length - 1)
        {
            currentSelected = 0;
        }

        if (currentSelected < 0)
        {
            currentSelected = weapons.Length - 1;
        }
        
        foreach (var everyWeapon in weapons)
        {
            if (everyWeapon != weapons[currentSelected])
            {
                everyWeapon.weaponMesh.SetActive(false);
                everyWeapon.weaponState = Weapon.WeaponState.NotSelected;
            }

            if (everyWeapon == weapons[currentSelected])
            {
                everyWeapon.weaponState = Weapon.WeaponState.Selected;
                everyWeapon.weaponMesh.SetActive(true);
            }
        }
    }
}
