using System;
using UnityEngine;

public class WeaponContainer : MonoBehaviour
{
    [Header("Weapon Switching")]
    public int currentSelected;
    public int lateSelected;
    public int lastSelected;

    public float throwForce;
    
    public Weapon[] weapons;

    [Header("Weapon Picking Up")] public LayerMask pickUpMask;
    public float pickUpDistance;
    public float pickUpRadius;

    public static WeaponContainer Instance { get; private set; }

    private void Awake()
    {
        //Setting This To a Singleton
        Instance = this;
    }

    public void FixedUpdate()
    {
        ContainerInput();

        if (currentSelected > weapons.Length - 1)
        {
            currentSelected = 0;
        }

        if (currentSelected < 0)
        {
            currentSelected = weapons.Length - 1;
        }

        if (currentSelected != lateSelected)
        {
            lastSelected = lateSelected;
            lateSelected = currentSelected;
        }

        foreach (var everyWeapon in weapons)
        {
            if (weapons[currentSelected] != null && weapons[currentSelected].isAvailable)
                weapons[currentSelected].weaponState = Weapon.WeaponState.Selected;

            if (everyWeapon != weapons[currentSelected] && everyWeapon.weaponState == Weapon.WeaponState.Selected)
            {
                everyWeapon.weaponState = Weapon.WeaponState.NotSelected;
            }

            if (everyWeapon.weaponState == Weapon.WeaponState.Selected && everyWeapon.isAvailable)
            {
                everyWeapon.weaponPrefab.SetActive(true);

                if (Input.GetKeyDown(InputManager.Instance.dropWeaponKey) && everyWeapon.canDrop)
                {
                    DropWeapon(everyWeapon, everyWeapon.name);
                    everyWeapon.isAvailable = false;
                }
            }
            
            else if (everyWeapon.weaponState == Weapon.WeaponState.NotSelected || !everyWeapon.isAvailable)
            {
                everyWeapon.weaponPrefab.SetActive(false);
            }

        }
    }

    private void ContainerInput()
    {
        currentSelected += (int) Input.mouseScrollDelta.y;

        for (var i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(weapons[i].changeInput))
            {
                currentSelected = i;
            }
        }
        
        if (Input.GetKeyDown(InputManager.Instance.pickupWeaponKey))
        {
            if (Physics.SphereCast(transform.position, pickUpRadius, transform.forward, out var hit, pickUpDistance, pickUpMask, QueryTriggerInteraction.UseGlobal))
            {
                hit.transform.gameObject.GetComponent<PickUpWeapon>().PickUp();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentSelected = lastSelected;
        }
    }

    public void PickUp(string weaponName, GameObject droppedWeapon)
    {
        int amountOfSupportingWeapons = 1;
        
        foreach (var everyWeapon in weapons)
        {
            if (everyWeapon.name == weaponName && amountOfSupportingWeapons > 0)
            {
                switch (everyWeapon.isAvailable)
                {
                    case false:
                        amountOfSupportingWeapons -= 1;
                        everyWeapon.isAvailable = false;
                        Destroy(droppedWeapon);
                        everyWeapon.isAvailable = true;
                        break;
                    
                    case true:
                        amountOfSupportingWeapons -= 1;
                        DropWeapon(everyWeapon, everyWeapon.name);
                        everyWeapon.isAvailable = false;
                        Destroy(droppedWeapon);
                        everyWeapon.isAvailable = true;
                        break;
                }
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
    }
    
    [Serializable]
    public class Weapon
    {
        public string name;
        
        public bool canDrop;
        public bool isAvailable;
    
        public KeyCode changeInput;
    
        public GameObject weaponPrefab;
        public GameObject dropWeaponPrefab;

        public enum WeaponState {Selected, NotSelected}
        public WeaponState weaponState;
    }
}
