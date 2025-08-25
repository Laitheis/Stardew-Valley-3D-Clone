using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data base", menuName = "Databases/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemDefinition> _itemDefinitions = new List<ItemDefinition>();

    [ContextMenu("Load All Items From Resources")]
    public void LoadAllFromResources()
    {
        _itemDefinitions.Clear();
        ItemDefinition[] loadedItems = Resources.LoadAll<ItemDefinition>("");

        _itemDefinitions.AddRange(loadedItems);
        Debug.Log($"[ItemDatabase] Загружено {_itemDefinitions.Count} предметов из Resources.");
    }

    public bool AddItem(ItemDefinition item)
    {
        if (item == null || _itemDefinitions.Contains(item))
            return false;

        _itemDefinitions.Add(item);
        return true;
    }

    public bool RemoveItem(ItemDefinition item)
    {
        if (item == null)
            return false;

        return _itemDefinitions.Remove(item);
    }

    public ItemDefinition GetItemAt(int index)
    {
        if (index < 0 || index >= _itemDefinitions.Count)
            return null;

        return _itemDefinitions[index];
    }

    public ItemDefinition GetItemByName(string name)
    {
        return _itemDefinitions.Find(item => item.Name == name);
    }

    public bool Contains(ItemDefinition item) => _itemDefinitions.Contains(item);

    public IReadOnlyList<ItemDefinition> GetAllItems() => _itemDefinitions;

    public int Count => _itemDefinitions.Count;
}
