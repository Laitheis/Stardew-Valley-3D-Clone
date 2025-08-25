using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameObject WorldItemPrefab;
    public override void InstallBindings()
    {
        // Регистрируем сервис
        Container.BindInterfacesAndSelfTo<LootService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<DropItem>().AsSingle().NonLazy();

        // Включаем сигналы
        SignalBusInstaller.Install(Container);

        // Регистрируем сигнал выпадения
        Container.DeclareSignal<ItemDropSignal>();

        Container.BindInstance(WorldItemPrefab);

        Container.Bind<LootTable>().FromScriptableObjectResource("Loot Table").AsSingle();
    }
}