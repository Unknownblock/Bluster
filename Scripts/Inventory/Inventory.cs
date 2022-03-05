using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IPointerDownHandler
{
    [Header("UI And Looks")]
    public Vector2 gap;
    public float slotSize;
    public int size;
    public int rows;
    public Vector2Int distance;
    public GameObject inventorySlot;
    
    [Header("Important Things")]
    public Item selectedItem;
    public List<Slot> inventorySlots;
    public List<Item> inventoryItems;
    
    [Header("Item Picking Up")] public LayerMask pickUpMask;
    public float pickUpDistance;
    public float pickUpRadius;

    private void Update()
    {
        ManagingTheSlots();
        SortingTheSlots();
        ItemPositionHandling();
    }

    private void SortingTheSlots()
    {
        var slots = inventorySlots.ToArray();

        float yPosition = gameObject.transform.position.y;
        
        for (int i = 0; i < slots.Length; i++)
        {
            float xPosition = transform.position.x  + gap.x * (i % rows);
            
            slots[i].name = "Slot " + (i + 1);

            if (i % rows == 0)
            {
                yPosition -= gap.y;
            }
            
            slots[i].transform.position = new Vector3(xPosition + distance.x, yPosition + distance.y, transform.position.z);
        }
    }

    private void ManagingTheSlots()
    {
        var slots = inventorySlots.ToArray();
        var items = inventoryItems.ToArray();

        foreach (var everySlot in slots)
        {
            everySlot.gameObject.transform.localScale = new Vector3(slotSize, slotSize, slotSize);
        }

        //Managing The Slots
        if (slots.Length < size)
        {
            for (var i = 0; i < size - slots.Length; i++)
            {
                GameObject slot = Instantiate(inventorySlot.gameObject, transform.position, Quaternion.identity,
                    gameObject.transform);

                slot.SetActive(true);

                inventorySlots.Add(slot.GetComponent<Slot>());
            }
        }

        if (slots.Length > size)
        {
            for (int i = 0; i < slots.Length - size; i++)
            {
                Destroy(slots[i].gameObject);
                inventorySlots.Remove(slots[i]);
            }
        }

        if (items.Length != slots.Length)
        {
            foreach (var everySlot in slots)
            {
                if (everySlot.currentSlotItem != null)
                {
                    inventoryItems.Add(everySlot.currentSlotItem);
                }

                if (everySlot.currentSlotItem == null)
                {
                    inventoryItems.Add(null);
                }
            }
        }
    }

    private void ItemPositionHandling()
    {
        var items = inventoryItems.ToArray();
        
        foreach (var everyItem in items)
        {
            if (everyItem != null)
            {
                if (everyItem.itemState == Item.ItemState.Static)
                {
                    everyItem.transform.localPosition = Vector3.zero;
                }

                if (everyItem.itemState == Item.ItemState.GettingGrabbed)
                {
                    everyItem.transform.position = Input.mousePosition;
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var slots = inventorySlots.ToArray();

        foreach (var everySlot in slots)
        {
            Slot slot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
            
            if (eventData.pointerPressRaycast.gameObject == everySlot.gameObject)
            {
                if (selectedItem == null && slot.currentSlotItem != null && slot != null)
                {
                    DragItem(slot);
                }
                
                else if (selectedItem != null && slot.currentSlotItem == null && slot != null)
                {
                    PutItem(slot);
                }
            }
            
            else if (selectedItem != null  && eventData.pointerPressRaycast.gameObject != everySlot.gameObject)
            {
                if (selectedItem != null && slot.currentSlotItem == null && slot != null)
                {
                    print("Drop Item");
                }
            }
        }
    }

    private void DragItem(Slot slot)
    {
        selectedItem = slot.currentSlotItem;
        slot.currentSlotItem.itemState = Item.ItemState.GettingGrabbed;
        slot.currentSlotItem = null;
        print("Getting Dragged");
    }
    
    private void PutItem(Slot slot)
    {
        slot.currentSlotItem = selectedItem;
        slot.currentSlotItem.itemState = Item.ItemState.Static;
        selectedItem = null;
        print("Static");
    }

    private void PickUpItem()
    {
        if (Input.GetKeyDown(InputManager.Instance.pickupWeaponKey))
        {
            if (Physics.SphereCast(transform.position, pickUpRadius, transform.forward, out var hit, pickUpDistance, pickUpMask, QueryTriggerInteraction.UseGlobal))
            {
                hit.transform.gameObject.GetComponent<PickUpWeapon>().PickUp();
            }
        }
    }

    private void PutItemInInventory(Item item)
    {
        
    }
}
