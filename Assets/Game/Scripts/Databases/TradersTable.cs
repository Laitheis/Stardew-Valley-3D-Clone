using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Traders Table", menuName = "Databases/Traders Table")]
public class TradersTable : ScriptableObject
{
    [SerializeField] private List<TraderModel> _traders;

    public List<TraderModel> Traders => _traders;

    public TraderModel FindByName(string traderName)
    {
        return _traders.FirstOrDefault(item => item.Name == traderName);
    }
}

[System.Serializable]
public class TraderModel
{
    public string Name;
    public Sprite Icon;
    [TextArea] public string TraderWelcomeText;

    public List<Item> Items;

    [System.Serializable]
    public class Item
    {
        public ItemDefinition ItemDefinition;
        public int Price;
        public Season[] seasons;
    }

}