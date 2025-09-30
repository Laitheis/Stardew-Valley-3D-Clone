public class OakTree : TreeBase
{
    public override void InitializeStats()
    {
        base.InitializeStats();

        _statContainer.GetStat(StatTypes.Durability).Value = 85;
    }

    public override void InitializeLoot()
    {
        _pendingLoot = _lootGenerator.GenerateLoot("Oak", 0);
    }
}

