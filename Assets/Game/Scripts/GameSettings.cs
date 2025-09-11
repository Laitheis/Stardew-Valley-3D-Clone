using Inventory.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
public class GameSettings : ScriptableObjectInstaller<GameSettings>
{
    public GameObject WorldItemPrefab;
    public GameObject SmokeExplosionEffect;
    public Image DraggedImagePrefab;
    public ItemDatabase ItemDatabase;
    public GameObject ItemSlotPrefab;

    public override void InstallBindings()
    {
        Container.BindInstance(WorldItemPrefab).WithId("WorldItem");
        Container.BindInstance(SmokeExplosionEffect).WithId("SmokeEffect");
        Container.BindInstance(DraggedImagePrefab).WithId("DraggedImagePrefab").AsSingle();
        Container.BindInstance(ItemDatabase).AsSingle().NonLazy();
        Container.BindInstance(ItemSlotPrefab).WithId("ItemSlot");

        //HACK
        InstallPlayerParam();
    }

    public void InstallPlayerParam()
    {
        //HACK
        Container.BindInstance(3).WithId("PickupDistance");
    }
}
