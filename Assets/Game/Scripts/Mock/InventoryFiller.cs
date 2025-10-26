using InventorySystem;
using UnityEngine;
using Zenject;

// Hack class
public class InventoryFiller : MonoBehaviour
{
    public ItemsCollection itemCollection1;
    public ItemInstance itemInstance;

    [Inject] private SignalBus _signalBus;
    [Inject(Id = "Player")] private GameObject _player;

    [ContextMenu("Fill")]
    public void Fill()
    {
        //itemInstance.SetCount(30);

        ItemDropUtil.Instance.AddWithDropToWorld(itemCollection1, Vector3.up * 100, itemInstance);

    }
}