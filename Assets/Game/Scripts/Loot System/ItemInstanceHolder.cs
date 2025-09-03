using UnityEngine;

public class ItemInstanceHolder: MonoBehaviour
{
    [SerializeField] private ItemInstance _item;

    public ItemInstance Item { get { return _item; } set { _item = value; } }

    public void Constructor(ItemInstance item)
    {
        _item = item;
    }
}