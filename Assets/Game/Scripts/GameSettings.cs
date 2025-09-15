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
    public GameObject SoilPrefab;
    public GameObject AvailableTilePrefab;
    public GameObject UnavailableTilePrefab;

    public override void InstallBindings()
    {
        Container.BindInstance(WorldItemPrefab).WithId("WorldItem");
        Container.BindInstance(SmokeExplosionEffect).WithId("SmokeEffect");
        Container.BindInstance(DraggedImagePrefab).WithId("DraggedImagePrefab").AsSingle();
        Container.BindInstance(ItemDatabase).AsSingle().NonLazy();
        Container.BindInstance(ItemSlotPrefab).WithId("ItemSlot");
        Container.BindInstance(SoilPrefab).WithId("Soil");
        Container.BindInstance(AvailableTilePrefab).WithId("Available");
        Container.BindInstance(UnavailableTilePrefab).WithId("Unavailable");

        //HACK
        InstallPlayerParam();
    }

    public void InstallPlayerParam()
    {
        //HACK
        Container.BindInstance(3).WithId("PickupDistance");
    }
}
