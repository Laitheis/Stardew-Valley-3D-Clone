using InventorySystem;
using UnityEngine;

public class InventoryFiller : MonoBehaviour
{
    public ItemsCollection itemCollection1;
    public ItemInstance itemInstance;

    public void Fill()
    {
        bool succes = itemCollection1.AddAt(itemInstance, 0, 2);
        itemCollection1.AddAt(itemInstance, 1, 2);
        itemCollection1.AddAt(itemInstance, 2, 1);
        if (!succes) Debug.Log("Not enough space in inventory!");

    }
}