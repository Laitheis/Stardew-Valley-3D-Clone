using Inventory.UI;
using InventorySystem;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public Canvas MainCanvas;
    public ItemTooltipView TooltipView;
    public GameObject Player;
    public InventoryHandler PlayerInv;
    public SelectedSlotHandler SelSlotHandler;
    public HintVisualizer HintVisualizer;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DropItemToWorld>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LootGenerator>().AsSingle();

        Container.Bind<LootTable>().FromScriptableObjectResource("Loot Table").AsSingle();

        Container.BindInstance(MainCanvas).AsSingle();
        Container.BindInstance(TooltipView).AsSingle();
        Container.BindInstance(Player).WithId("Player");
        Container.BindInstance(PlayerInv).WithId("PlayerInv");
        Container.BindInstance(SelSlotHandler).AsSingle();
        Container.BindInstance(HintVisualizer).AsSingle();

        Container.Bind<ItemDropUtil>().AsSingle().NonLazy();

        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ItemDropEvent>();

    }
}
