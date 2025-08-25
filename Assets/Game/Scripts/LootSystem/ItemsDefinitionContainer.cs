using System.Collections.Generic;

public class ItemsDefinitionContainer
{
    private List<ItemDefinition> _items;

    public List<ItemDefinition> Items => _items;

    public ItemsDefinitionContainer()
    {
        _items = new List<ItemDefinition>();
    }

    public void AddItem(ItemDefinition item)
    {
        _items.Add(item);
    }
}