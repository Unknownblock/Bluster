using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponContainer : MonoBehaviour
{
    [Header("Weapon Switching")]
    public int currentSelected;
    public int lastSelected;
    public int lateSelected;
    
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

    private void Start()
    {
        foreach (var everyWeapon in weapons)
        {
            everyWeapon.id = Random.Range(0, 1000);
        }
    }

    private void LateUpdate()
    {
        lateSelected = currentSelected;
    }

    public void FixedUpdate()
    {
        ChangeInput();
        PickingUpTheWeapon();
        
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
                    DropWeapon(everyWeapon, everyWeapon.name, everyWeapon.id);
                }
            }
            
            else if (everyWeapon.weaponState == Weapon.WeaponState.NotSelected || !everyWeapon.isAvailable)
            {
                everyWeapon.weaponPrefab.SetActive(false);
            }

        }
    }

    private void ChangeInput()
    {
        currentSelected += (int) Input.mouseScrollDelta.y;
        
        if (Input.GetKeyDown(InputManager.Instance.lastWeaponKey))
        {
            currentSelected = lastSelected;
        }

        for (var i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(weapons[i].changeInput))
            {
                currentSelected = i;
            }
        }
    }

    private void PickingUpTheWeapon()
    {
        if (Input.GetKeyDown(InputManager.Instance.pickupWeaponKey))
        {
            if (Physics.SphereCast(transform.position, pickUpRadius, transform.forward, out var hit, pickUpDistance, pickUpMask, QueryTriggerInteraction.UseGlobal))
            {
                hit.transform.gameObject.GetComponent<PickUpWeapon>().PickUp();
            }
        }
    }

    private void DropWeapon(Weapon weapon, string wantedName, int wantedId)
    {
        GameObject droppedWeapon = Instantiate(weapon.dropWeaponPrefab, weapon.weaponPrefab.transform.position, weapon.weaponPrefab.transform.rotation);
        
        droppedWeapon.GetComponent<Rigidbody>().AddForce(MoveCamera.Instance.transform.forward * throwForce);
        
        if (droppedWeapon.GetComponent<PickUpWeapon>() == null)
        {
            droppedWeapon.AddComponent<PickUpWeapon>();
        }
        
        droppedWeapon.GetComponent<PickUpWeapon>().weaponName = wantedName;
        droppedWeapon.GetComponent<PickUpWeapon>().weaponId = wantedId;
        
        weapon.isAvailable = false;

        currentSelected = lastSelected;
    }
    
    [Serializable]
    public class Weapon
    {
        public string name;

        public int id;
    
        public bool canDrop;
        public bool isAvailable;
    
        public KeyCode changeInput;
    
        public GameObject weaponPrefab;
        public GameObject dropWeaponPrefab;

        public enum WeaponState {Selected, NotSelected}
        public WeaponState weaponState;
    }
}
