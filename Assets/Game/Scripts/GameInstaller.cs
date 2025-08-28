using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DropItemToWorld>().AsSingle().NonLazy();

        Container.Bind<LootTable>().FromScriptableObjectResource("Loot Table").AsSingle();
        Container.Bind<LootGenerator>().AsSingle();

        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ItemDropSignal>();
    }
}