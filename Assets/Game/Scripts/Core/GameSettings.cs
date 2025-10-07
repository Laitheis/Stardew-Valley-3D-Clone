using UnityEngine;
using UnityEngine.UI;
using Zenject;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
public class GameSettings : ScriptableObjectInstaller<GameSettings>
{
    public GameObject WorldItemPrefab;
    public GameObject SmokeExplosionEffect;
    public Image DraggedImagePrefab;
    public DefinitionDatabase ItemDatabase;
    public GameObject ItemSlotPrefab;
    public GameObject SoilPrefab;
    public GameObject SoilWetPrefab;
    public GameObject AvailableTilePrefab;
    public LootTable LootTable;
    public TradersTable TradersTable;
    public GameObject FloatingText;
    public Material OutlineGlowMat;
    public GameObject StarParticles;
    public GameObject WorldTooltip;
    public GameObject Notification;


    public override void InstallBindings()
    {
        Container.BindInstance(WorldItemPrefab).WithId("WorldItem");
        Container.BindInstance(SmokeExplosionEffect).WithId("SmokeEffect");
        Container.BindInstance(DraggedImagePrefab).WithId("DraggedImagePrefab");
        Container.BindInstance(ItemDatabase).NonLazy();
        Container.BindInstance(ItemSlotPrefab).WithId("ItemSlot");
        Container.BindInstance(SoilPrefab).WithId("Soil");
        Container.BindInstance(SoilWetPrefab).WithId("SoilWet");
        Container.BindInstance(AvailableTilePrefab).WithId("Available");
        Container.BindInstance(LootTable);
        Container.BindInstance(TradersTable);
        Container.BindInstance(FloatingText).WithId("FloatingText");
        Container.BindInstance(OutlineGlowMat).WithId("OutlineGlow");
        Container.BindInstance(StarParticles).WithId("StarParticles");
        Container.BindInstance(WorldTooltip).WithId("WorldTooltip");
        Container.BindInstance(Notification).WithId("Notif");

        //HACK
        InstallPlayerParam();
    }

    public void InstallPlayerParam()
    {
        //HACK
        Container.BindInstance(3).WithId("PickupDistance");
    }
}
