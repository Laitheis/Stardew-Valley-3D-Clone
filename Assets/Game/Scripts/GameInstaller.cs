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
    public PlayerToolHandler PlayerToolHandler;
    public GameObject DimmingScreen;
    public PlayerController PlayerController;
    public TradingHandler TradingHandler;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DropItemToWorld>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LootGenerator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ItemDropUtil>().AsSingle().NonLazy();

        Container.BindInstance(MainCanvas).AsSingle();
        Container.BindInstance(TooltipView).AsSingle();
        Container.BindInstance(Player).WithId("Player");
        Container.BindInstance(PlayerInv).WithId("PlayerInv").AsSingle();
        Container.BindInstance(PlayerController).AsSingle();
        Container.BindInstance(SelSlotHandler).AsSingle();
        Container.BindInstance(HintVisualizer).AsSingle();
        Container.BindInstance(PlayerToolHandler).AsSingle();
        Container.BindInstance(DimmingScreen).WithId("Dimming");
        Container.BindInstance(TradingHandler);


        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ItemDropEvent>();

    }
}
