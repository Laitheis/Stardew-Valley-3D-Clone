public class OakTree : TreeBase
{
    public override void InitializeStats()
    {
        base.InitializeStats();

        StatContainer.GetStat(StatTypes.Durability).Value = 100;
    }
}

