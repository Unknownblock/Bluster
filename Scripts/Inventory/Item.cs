using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemState
    {Static, GettingGrabbed}

    public ItemState itemState;

    public string itemName;

    private void Update()
    {
        gameObject.name = transform.parent.gameObject.name + "'s Item";
    }
}
