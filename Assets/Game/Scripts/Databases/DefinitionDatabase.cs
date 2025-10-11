using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data base", menuName = "Databases/ItemDatabase")]
public class DefinitionDatabase : ScriptableObject
{
    [SerializeField] public List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();
    [SerializeField] public List<CropModel> cropModels = new List<CropModel>();
    [SerializeField] public List<DebrisModel> debrisModels = new List<DebrisModel>();

    [ContextMenu("Load All Items From Resources")]
    public void LoadAllFromResources()
    {
        itemDefinitions.Clear();
        ItemDefinition[] loadedItems = Resources.LoadAll<ItemDefinition>("");

        itemDefinitions.AddRange(loadedItems);
        Debug.Log($"[ItemDatabase] Загружено {itemDefinitions.Count} предметов из Resources.");
    }
}
