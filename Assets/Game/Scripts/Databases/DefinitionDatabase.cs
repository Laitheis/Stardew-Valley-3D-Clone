using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data base", menuName = "Databases/ItemDatabase")]
public class DefinitionDatabase : ScriptableObject
{
    //TODO
    [SerializeField] public List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();
    [SerializeField] public List<CropModel> cropModels = new List<CropModel>();

    [ContextMenu("Load All Items From Resources")]
    public void LoadAllFromResources()
    {
        itemDefinitions.Clear();
        ItemDefinition[] loadedItems = Resources.LoadAll<ItemDefinition>("");

        itemDefinitions.AddRange(loadedItems);
        Debug.Log($"[ItemDatabase] Загружено {itemDefinitions.Count} предметов из Resources.");
    }

    public bool AddItem(ItemDefinition item)
    {
        if (item == null || itemDefinitions.Contains(item))
            return false;

        itemDefinitions.Add(item);
        return true;
    }

    public bool RemoveItem(ItemDefinition item)
    {
        if (item == null)
            return false;

        return itemDefinitions.Remove(item);
    }

    public ItemDefinition GetItemAt(int index)
    {
        if (index < 0 || index >= itemDefinitions.Count)
            return null;

        return itemDefinitions[index];
    }

    public ItemDefinition GetItemByName(string name)
    {
        return itemDefinitions.Find(item => item.Name == name);
    }

    public bool Contains(ItemDefinition item) => itemDefinitions.Contains(item);

    public IReadOnlyList<ItemDefinition> GetAllItems() => itemDefinitions;

    public int Count => itemDefinitions.Count;
}
