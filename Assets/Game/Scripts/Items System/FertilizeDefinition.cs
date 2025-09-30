using UnityEngine;

[CreateAssetMenu(fileName = "New Fertilize", menuName = "Collections/ItemFertilize")]
public class FertilizeDefinition : ItemDefinition
{
    public new FertilizeType type;
}

public enum FertilizeType
{
    Quality,
    QualityPro,
    Speed,
    SpeedPro,
    Retaining,
    Tree,
}

