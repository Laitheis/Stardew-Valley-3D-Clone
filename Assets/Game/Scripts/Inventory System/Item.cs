using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Mock/Item")]
public class Item : ScriptableObject
{
    [SerializeField] GameObject _prefab;
    [SerializeField] string _name;
    [SerializeField] Sprite _sprite;

    [TextArea] public string _description;

    public string Name { get => _name; set => _name = value; }
    public GameObject Prefab { get => _prefab; set => _prefab = value; }
    public Sprite Sprite { get => _sprite; set => _sprite = value; }
    public string Description { get => _description; set => _description = value; }

    public GameObject CreateInstanceInWorld()
    {
        return Instantiate(_prefab);
    }

}
