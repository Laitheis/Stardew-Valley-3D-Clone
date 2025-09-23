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
    public TraderHandler TraderHandler;
    public CurrencyManager CurrencyManager;
    public UIDragController UIDragController;



    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DropItemToWorld>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LootGenerator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ItemDropUtil>().AsSingle().NonLazy();

        Container.BindInstance(MainCanvas);
        Container.BindInstance(TooltipView);
        Container.BindInstance(Player).WithId("Player");
        Container.BindInstance(PlayerInv).WithId("PlayerInv");
        Container.BindInstance(PlayerController);
        Container.BindInstance(SelSlotHandler);
        Container.BindInstance(HintVisualizer);
        Container.BindInstance(PlayerToolHandler);
        Container.BindInstance(DimmingScreen).WithId("Dimming");
        Container.BindInstance(TraderHandler);
        Container.BindInstance(CurrencyManager);
        Container.BindInstance(UIDragController);


        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ItemDropEvent>();
        Container.DeclareSignal<CurrencyEventArgs>();

    }
}
