using UnityEngine;

public class Slot : MonoBehaviour
{
    public Inventory inventory;
    public Item currentSlotItem;
    public int amountOfItems;
    public int maxAmountOfItems;

    private void Update()
    {
        if (currentSlotItem != null)
        {
            currentSlotItem.transform.SetParent(gameObject.transform);
        }

        Mathf.Clamp(amountOfItems, 1, maxAmountOfItems);
    }
}