using UnityEngine;

public class ItemDropEvent
{
    public Vector3 Position;

    public ItemInstance Item;

    public ItemDropEvent(Vector3 position, ItemInstance item)
    {
        Position = position;
        Item = item;
    }
}

