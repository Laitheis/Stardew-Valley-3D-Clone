using UnityEngine;

public class ItemDropEvent
{
    public Vector3 Position;
    public ItemInstance Item;
    public bool IsDroppedFromPlayer;

    public ItemDropEvent(Vector3 position, ItemInstance item, bool isDroppedFromPlayer)
    {
        Position = position;
        Item = item;
        IsDroppedFromPlayer = isDroppedFromPlayer;
    }
}

