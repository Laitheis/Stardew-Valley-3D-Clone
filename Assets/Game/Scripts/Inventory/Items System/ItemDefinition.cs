using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Collections/Item")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private ItemType _type;
    [SerializeField] private bool _isRenameable;
    [SerializeField] [Min(1)] private int _maxCountInStack;
    [SerializeField] private int _price;
    [SerializeField] [TextArea] private string _description;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _prefab;

    public string Name => _name;
    public bool IsRenameable => _isRenameable;
    public int MaxCountInStack => _maxCountInStack;
    public int Price => _price;
    public string Description => _description;
    public GameObject Prefab => _prefab;
    public Sprite Sprite => _sprite;
    public ItemType Type => _type;

    //

    public virtual void UseItem()
    {

    }
}

public enum ItemType
{
    Tool,
    Material,
}
