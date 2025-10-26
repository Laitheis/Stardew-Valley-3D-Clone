using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Collections/Item")]
public class ItemDefinition : ScriptableObject
{
    public bool isTool;

    [SerializeField] private int _id;
    [SerializeField] private string _name;
    public ItemType type = ItemType.None;
    [SerializeField][Min(1)] private int _maxCountInStack;
    [SerializeField] private int _price;
    [SerializeField][TextArea] private string _description;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _prefab;
    [Header("For Tools")]
    [SerializeField] private int _damage;

    public string Name => _name;
    public int MaxCountInStack => _maxCountInStack;
    public int Price => _price;
    public string Description => _description;
    public GameObject Prefab => _prefab;
    public Sprite Sprite => _sprite;
    public int Damage { get => _damage; set => _damage = value; }
    public int Id { get => _id; set => _id = value; }
}

public enum ItemType { None, Trash, Hoe, WaterCan, Scythe, Axe, Pickaxe, Seed, Material, Crop, Fertilize }