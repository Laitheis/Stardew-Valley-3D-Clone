using UnityEngine;

public class ItemDropSignal
{
    public Vector3 Position;

    public ItemInstance Item;

    public ItemDropSignal(Vector3 position, ItemInstance item)
    {
        Position = position;
        Item = item;
    }
}

