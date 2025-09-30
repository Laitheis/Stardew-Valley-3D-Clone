using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Collections/Item")]
public class ItemDefinition : ScriptableObject
{
    public bool isTool;

    [SerializeField] private string _name;
    public ItemType type = ItemType.None;
    [SerializeField][Min(1)] private int _maxCountInStack;
    [SerializeField] private int _price;
    [SerializeField][TextArea] private string _description;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _prefab;

    public string Name => _name;
    public int MaxCountInStack => _maxCountInStack;
    public int Price => _price;
    public string Description => _description;
    public GameObject Prefab => _prefab;
    public Sprite Sprite => _sprite;
}

public enum ItemType { None, Regular, Hoe, WaterCan, Scythe, Axe, Pickaxe, Seed, Material, Crop, Fertilize }