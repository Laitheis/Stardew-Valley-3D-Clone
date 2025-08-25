public class OakTree : TreeBase
{
    public override void InitializeStats()
    {
        base.InitializeStats();

        _statContainer.GetStat(StatTypes.Durability).Value = 100;
    }
}

