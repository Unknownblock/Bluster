using UnityEngine;

public class InputManager : MonoBehaviour
{
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode pickupWeaponKey = KeyCode.F;
    public KeyCode dropWeaponKey = KeyCode.G;
    public KeyCode lastWeaponKey = KeyCode.Q;
    public KeyCode inventoryKey = KeyCode.Tab;

    public static InputManager Instance { get; set; }

    private void Awake()
    {
        //Setting This To a Singleton
        Instance = this;
    }
}
