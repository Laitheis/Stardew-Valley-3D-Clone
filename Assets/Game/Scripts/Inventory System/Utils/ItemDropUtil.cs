using InventorySystem;
using UnityEngine;

public static class ItemDropUtil
{
    public static void Drop(Vector3 pos, Vector3 impulse, ItemInstance item)
    {
        if (item.ItemDefinition.Prefab == null)
        {
            Debug.LogWarning($"Префаб для предмета {item.ItemDefinition} не назначен!");
            return;
        }

        GameObject spawnedItem = GameObject.Instantiate(item.ItemDefinition.Prefab, pos, Quaternion.identity);

        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.AddForce(impulse, ForceMode.Impulse);
        }
    }
}