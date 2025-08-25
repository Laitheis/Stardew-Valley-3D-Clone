using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Collections/Item")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private bool _isRenameable;
    [SerializeField] private int _maxCountInStack;
    [SerializeField] private bool _isStackable;
    [SerializeField] private int _price;

    public string Name => _name;
    public bool IsRenameable => _isRenameable;
    public int MaxCountInStack => _maxCountInStack;
    public bool IsStackable => _isStackable;
    public int Price => _price;

    //

    public virtual void UseItem()
    {

    }
}

