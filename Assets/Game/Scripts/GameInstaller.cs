using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameObject WorldItemPrefab;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DropItemToWorld>().AsSingle().NonLazy();

        Container.BindInstance(WorldItemPrefab);

        Container.Bind<LootTable>().FromScriptableObjectResource("Loot Table").AsSingle();
        Container.Bind<LootGenerator>().AsSingle();

        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ItemDropSignal>();

    }
}