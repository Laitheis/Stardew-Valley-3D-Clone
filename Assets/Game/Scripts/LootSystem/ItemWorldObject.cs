using UnityEngine;

public class ItemWorldObject: MonoBehaviour
{
    [SerializeField] private ItemInstance _item;

    public ItemInstance Item => _item;

    public void Constructor(ItemInstance item)
    {
        _item = item;
    }
}