using InventorySystem;
using UnityEngine;
using Zenject;

public class BindingInstaller : MonoInstaller
{
    public Canvas MainCanvas;
    public TooltipView TooltipView;
    public GameObject Player;
    public InventoryHandler PlayerInv;
    public SelectedSlotController SelSlotHandler;
    public HintVisualizer HintVisualizer;
    public PlayerToolController PlayerToolHandler;
    public GameObject DimmingScreen;
    public PlayerController PlayerController;
    public TraderHandler TraderHandler;
    public CurrencyHandler CurrencyManager;
    public UIDragController UIDragController;
    public Camera UICamera;
    public TileContainer FarmTiles;
    public CropController CropManager;
    public DebrisGeneratorController DebrisGenerator;
    public GameObject StatusPanel;
    public FarmManager FarmManager;
    public InputHandler InputHandler;


    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DropItemToWorldController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LootGeneratorHandler>().AsSingle().NonLazy();
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
        Container.BindInstance(UICamera);
        Container.BindInstance(CropManager);
        Container.BindInstance(DebrisGenerator);
        Container.BindInstance(StatusPanel).WithId("StatusPanel");
        Container.BindInstance(FarmTiles).WithId("FarmTiles");
        Container.BindInstance(FarmManager);
        Container.BindInstance(InputHandler);


        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ItemDropEvent>();
        Container.DeclareSignal<CurrencyEventArgs>();

    }
}
