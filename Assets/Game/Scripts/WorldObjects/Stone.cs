using UnityEngine;

public class Stone : DestructibleObjectBase
{
    public override void InitializeStats()
    {
        base.InitializeStats();

        _statContainer.GetStat(StatTypes.Durability).Value = 30;
    }

    protected override void InitializeLoot()
    {
        base.InitializeLoot();

        _pendingLoot = _lootGenerator.GenerateLoot("Stone", 0);
    }
}

