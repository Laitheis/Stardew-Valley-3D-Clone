using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Collections/Item")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] protected ItemType _type = ItemType.Material;
    [SerializeField] private bool _isRenameable;
    [SerializeField][Min(1)] private int _maxCountInStack;
    [SerializeField] private int _price;
    [SerializeField][TextArea] private string _description;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _prefab;

    public string Name => _name;
    public bool IsRenameable => _isRenameable;
    public int MaxCountInStack => _maxCountInStack;
    public int Price => _price;
    public string Description => _description;
    public GameObject Prefab => _prefab;
    public Sprite Sprite => _sprite;
    public virtual ItemType Type => _type;

    //

    public virtual void UseItem()
    {

    }
}

public enum ItemType { None, Tool, Material }

public enum Tool { None, Hoe, Water, Harvest, Axe, Pickaxe }