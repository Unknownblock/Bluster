using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerDownHandler
{
    public Inventory inventory;
    public Item currentSlotItem;
    public int amountOfItems;
    public int maxAmountOfItems;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inventory.selectedItem == null && currentSlotItem == inventory.selectedItem)
        {

            currentSlotItem.itemState = Item.ItemState.GettingGrabbed;
            inventory.selectedItem = currentSlotItem;
            currentSlotItem = null;
            print("Getting Grabbed");
        }

        if (inventory.selectedItem != null)
        {
            currentSlotItem = inventory.selectedItem;
            inventory.selectedItem = null;
            currentSlotItem.itemState = Item.ItemState.Static;
            print("Static");
        }
    }
}