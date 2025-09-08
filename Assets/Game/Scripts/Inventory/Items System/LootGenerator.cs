using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LootGenerator
{
    private LootTable _lootTable;

    [Inject]
    public LootGenerator(LootTable lootTable)
    {
        _lootTable = lootTable;
    }

    public List<ItemInstance> GenerateLoot(string lootName, int grade)
    {
        EntityLoot entityLoot = _lootTable.FindByName(lootName);
        LootByGrade lootByGrade = entityLoot.GetByGrade(grade);

        List<ItemInstance> itemInstances = new List<ItemInstance>();

        foreach (ItemDropChance itemDropChance in lootByGrade.Loot)
        {
            ItemInstance _item = GenerateItem(itemDropChance);
            itemInstances.Add(_item);
        }

        itemInstances.RemoveAll(item => item == null);

        return itemInstances;
    }

    public ItemInstance GenerateItem(ItemDropChance itemDropChance)
    {
        float roll = UnityEngine.Random.Range(0f, 100f);
        if (roll <= itemDropChance.Chance)
        {
            ItemInstance droppedItem = new ItemInstance(itemDropChance.Item);

            int roll2 = UnityEngine.Random.Range(itemDropChance.MinCount, itemDropChance.MaxCount + 1);
            if (roll2 == 0) return null;

            droppedItem.SetCount(roll2);

            Debug.Log($"Generated item: {itemDropChance.Item.name} in quantity {roll2} (chance {itemDropChance.Chance}%)");
            
            return droppedItem;
        }

        return null;
    }
}