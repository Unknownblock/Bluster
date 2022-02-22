using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Vector2 gap;
    public float slotSize;
    public int size;
    public int rows;
    public Vector2Int distance;
    public GameObject inventorySlot;
    public Item selectedItem;
    public List<Slot> inventorySlots;
    public List<Item> inventoryItems;

    private void Update()
    {
        ManagingTheSlots();
        SortingTheSlots();
        DraggingItems();

        Mathf.Clamp(size, 0, 100);
        Mathf.Clamp(rows, 0, 100);
    }

    private void SortingTheSlots()
    {
        var slots = inventorySlots.ToArray();
        var items = inventoryItems.ToArray();
        
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

        foreach (var everyItem in items)
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

    private void ManagingTheSlots()
    {
        var slots = inventorySlots.ToArray();
        var items = inventoryItems.ToArray();

        foreach (var everySlot in slots)
        {
            everySlot.gameObject.transform.localScale = new Vector3(slotSize, slotSize, slotSize);
        }

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
            foreach (var everyItem in items)
            {
                inventoryItems.Remove(everyItem);
            }
            
            foreach (var everySlot in slots)
            {
                inventoryItems.Add(everySlot.currentSlotItem);
            }
        }
    }

    private void DraggingItems()
    {
        if (selectedItem != null)
        {
            selectedItem.transform.position = Input.mousePosition;
        }
    }
}
