using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject inventory;
    public bool openedInventory;
    
    private void Update()
    {
        if (openedInventory)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (!openedInventory)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        if (Input.GetKeyDown(InputManager.Instance.inventoryKey) && !openedInventory)
        {
            openedInventory = true;
            inventory.SetActive(true);
        }
        
        else if (Input.GetKeyDown(InputManager.Instance.inventoryKey) && openedInventory)
        {
            openedInventory = false;
            inventory.SetActive(false);
        }
    }
}