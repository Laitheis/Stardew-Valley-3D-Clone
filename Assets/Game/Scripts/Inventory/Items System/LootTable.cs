using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Databases/Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField] private List<EntityLoot> _entityLoots;

    public List<EntityLoot> EntityLoots => _entityLoots;

    public EntityLoot FindByName(string lootName)
    {
        return _entityLoots.FirstOrDefault(item => item.LootName == lootName);
    }
}

[System.Serializable]
public class EntityLoot
{
    public string LootName;

    public List<LootByGrade> LootByGrades;

    public LootByGrade GetByGrade(int grade)
    {
        return LootByGrades.FirstOrDefault(item => item.Grade == grade);
    }
}

[System.Serializable]
public class LootByGrade
{
    public int Grade;

    public List<ItemDropChance> Loot;
}

[System.Serializable]
public class ItemDropChance
{
    [Range(0, 100)] public float Chance;

    public ItemDefinition Item;

    [Min(0)] public int MinCount = 1;
    [Min(0)] public int MaxCount = 1;
}