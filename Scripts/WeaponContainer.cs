using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Weapon
{
    public string name;

    public bool canDrop;
    public bool haveTheWeapon;
    
    public KeyCode changeInput;
    
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
        ChangeInput();

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
                    DropWeapon(everyWeapon, everyWeapon.name);
                }
            }
            
            else if (everyWeapon.weaponState == Weapon.WeaponState.NotSelected || !everyWeapon.haveTheWeapon)
            {
                everyWeapon.weaponPrefab.SetActive(false);
            }

        }
    }

    private void ChangeInput()
    {
        currentSelected += (int) Input.mouseScrollDelta.y;

        for (var i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(weapons[i].changeInput))
            {
                currentSelected = i;
            }
        }
    }

    public void DropWeapon(Weapon weapon, string wantedName)
    {
        GameObject droppedWeapon = Instantiate(weapon.dropWeaponPrefab, weapon.weaponPrefab.transform.position, weapon.weaponPrefab.transform.rotation);
        
        droppedWeapon.GetComponent<Rigidbody>().AddForce(MoveCamera.Instance.transform.forward * throwForce);
        
        if (droppedWeapon.GetComponent<PickUpWeapon>() == null)
        {
            droppedWeapon.AddComponent<PickUpWeapon>();
        }
        
        droppedWeapon.GetComponent<PickUpWeapon>().weaponName = wantedName;
        
        weapon.haveTheWeapon = false;
    }
}
