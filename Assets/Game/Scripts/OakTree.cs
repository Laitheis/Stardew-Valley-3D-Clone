public class OakTree : TreeBase
{
    public override void InitializeStats()
    {
        base.InitializeStats();

        _statContainer.GetStat(StatTypes.Durability).Value = 100;
    }

    public override void InitializeLoot()
    {
        base.InitializeLoot();

        EntityLoot entityLoot = _loot.FindByName("Oak");
    }
}

