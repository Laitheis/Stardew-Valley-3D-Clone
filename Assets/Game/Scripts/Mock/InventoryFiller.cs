using InventorySystem;
using UnityEngine;

public class InventoryFiller : MonoBehaviour
{
    public ItemsCollection itemCollection1;

    private void Start()
    {
        itemCollection1.AddRange(5);
    }
}