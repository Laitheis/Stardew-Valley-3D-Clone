using InventorySystem;
using UnityEngine;

public class InventoryFiller : MonoBehaviour
{
    public ItemsCollection itemCollection1;
    public ItemInstance itemInstance;

    public void Fill()
    {
        itemInstance.SetCount(2);

        bool succes = itemCollection1.AddAt(itemInstance, 0);
        itemInstance.SetCount(2);
        itemCollection1.AddAt(itemInstance, 1);
        itemInstance.SetCount(1);
        itemCollection1.AddAt(itemInstance, 2);
        if (!succes) Debug.Log("Not enough space in inventory!");

    }
}