using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Weapon
{
    public string name;

    public bool canDrop;
    public bool haveTheWeapon;
    
    public GameObject weaponPrefab;
    public GameObject dropWeaponPrefab;

    public enum WeaponState {Selected, NotSelected}

    public WeaponState weaponState;
}

public class WeaponContainer : MonoBehaviour
{
    public int currentSelected;

    public float throwForce;

    public Weapon[] weapons;

    public static WeaponContainer Instance { get; private set; }

    private void Awake()
    {
        //Setting This To a Singleton
        Instance = this;
    }
    
    public void FixedUpdate()
    {
        NumInput();

        currentSelected += (int) Input.mouseScrollDelta.y;

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
            if (weapons[currentSelected] != null && weapons[currentSelected].haveTheWeapon)
                weapons[currentSelected].weaponState = Weapon.WeaponState.Selected;

            if (everyWeapon != weapons[currentSelected] && everyWeapon.weaponState == Weapon.WeaponState.Selected)
            {
                everyWeapon.weaponState = Weapon.WeaponState.NotSelected;
            }

            if (everyWeapon.weaponState == Weapon.WeaponState.Selected && everyWeapon.haveTheWeapon)
            {
                everyWeapon.weaponPrefab.SetActive(true);

                if (Input.GetKeyDown(InputManager.Instance.dropWeaponKey) && everyWeapon.canDrop)
                {
                    DropWeapon(everyWeapon);
                }
            }
            
            else if (everyWeapon.weaponState == Weapon.WeaponState.NotSelected || !everyWeapon.haveTheWeapon)
            {
                everyWeapon.weaponPrefab.SetActive(false);
            }

        }
    }

    public void NumInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentSelected = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentSelected = 1;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentSelected = 2;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentSelected = 3;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentSelected = 4;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentSelected = 5;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            currentSelected = 6;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            currentSelected = 7;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            currentSelected = 8;
        }
    }

    public void DropWeapon(Weapon weapon)
    {
        GameObject droppedWeapon = Instantiate(weapon.dropWeaponPrefab, weapon.weaponPrefab.transform.position, weapon.weaponPrefab.transform.rotation);
        droppedWeapon.GetComponent<Rigidbody>().AddForce(MoveCamera.Instance.transform.forward * throwForce);
        weapon.haveTheWeapon = false;
    }
}
