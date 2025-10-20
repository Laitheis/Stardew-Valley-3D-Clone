using UnityEngine;

public class Grass : DestructibleObjectBase
{
    public override void InitializeStats()
    {
        base.InitializeStats();

        _statContainer.GetStat(StatTypes.Durability).Value = 10;
    }

    protected override void InitializeLoot()
    {
        base.InitializeLoot();

        _pendingLoot = _lootGenerator.GenerateLoot("Grass", 0);
    }
}

