using UnityEngine;
using UnityEngine.UI;

public class WorldTooltipable : MonoBehaviour
{
    public WorldObjectType type;
}

public enum WorldObjectType
{
    Item,
    Crop,
    Build,
}

