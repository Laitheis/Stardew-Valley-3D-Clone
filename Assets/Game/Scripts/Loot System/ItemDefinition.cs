using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Collections/Item")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private bool _isRenameable;
    [SerializeField] private int _maxCountInStack;
    [SerializeField] private bool _isStackable;
    [SerializeField] private int _price;
    [SerializeField] [TextArea] private string _description;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _prefab;

    public string Name => _name;
    public bool IsRenameable => _isRenameable;
    public int MaxCountInStack => _maxCountInStack;
    public bool IsStackable => _isStackable;
    public int Price => _price;
    public string Description => _description;
    public GameObject Prefab => _prefab;
    public Sprite Sprite => _sprite;

    //

    public virtual void UseItem()
    {

    }
}

