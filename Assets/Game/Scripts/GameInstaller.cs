using Inventory.UI;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public Canvas MainCanvas;
    public ItemTooltipView TooltipView;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DropItemToWorld>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LootGenerator>().AsSingle();

        Container.Bind<LootTable>().FromScriptableObjectResource("Loot Table").AsSingle();

        Container.BindInstance(MainCanvas).AsSingle();
        Container.BindInstance(TooltipView).AsSingle();

        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ItemDropSignal>();

    }
}
