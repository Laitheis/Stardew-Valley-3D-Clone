public class Rock : DestructibleObjectBase
{
    public override void InitializeStats()
    {
        base.InitializeStats();

        _statContainer.GetStat(StatTypes.Durability).Value = 50;
    }

    protected override void InitializeLoot()
    {
        base.InitializeLoot();

        _pendingLoot = _lootGenerator.GenerateLoot("Stone", 0);
    }
}

