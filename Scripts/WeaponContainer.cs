using System;
using UnityEngine;

[Serializable]
public class Weapon
{
    public string name;

    public bool canSwitchTo;
    public bool canSwitchFrom;
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

    public int lastSelected;

    public int lateSelected;

    public float throwForce;
    
    public Weapon[] weapons;

    public static WeaponContainer Instance { get; private set; }

    private void Awake()
    {
        //Setting This To a Singleton
        Instance = this;
    }

    private void LateUpdate()
    {
        lateSelected = currentSelected;
    }

    public void FixedUpdate()
    {
        ChangeInput();
        
        if (currentSelected != lateSelected)
        {
            lastSelected = lateSelected;
        }

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
        if (Input.GetKeyDown(InputManager.Instance.lastWeaponInput))
        {
            currentSelected = lastSelected;
        }
        
        for (var i = 0; i < weapons.Length; i++)
        {
            if (!weapons[currentSelected].canSwitchFrom)
            {
                currentSelected = i;
            }
            
            else if (Input.GetKeyDown(weapons[i].changeInput))
            {
                currentSelected = i;
            }

            else
            {
                currentSelected += (int) Input.mouseScrollDelta.y;
            }
        }
    }

    private void DropWeapon(Weapon weapon, string wantedName)
    {
        GameObject droppedWeapon = Instantiate(weapon.dropWeaponPrefab, weapon.weaponPrefab.transform.position, weapon.weaponPrefab.transform.rotation);
        
        droppedWeapon.GetComponent<Rigidbody>().AddForce(MoveCamera.Instance.transform.forward * throwForce);
        
        if (droppedWeapon.GetComponent<PickUpWeapon>() == null)
        {
            droppedWeapon.AddComponent<PickUpWeapon>();
        }
        
        droppedWeapon.GetComponent<PickUpWeapon>().weaponName = wantedName;
        
        weapon.haveTheWeapon = false;

        currentSelected = lastSelected;
    }
}
