using Zenject;

public class ServicesInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Регистрируем сервис
        Container.BindInterfacesAndSelfTo<LootService>().AsSingle();
        Container.BindInterfacesAndSelfTo<DropItem>().AsSingle().NonLazy();

        // Включаем сигналы
        SignalBusInstaller.Install(Container);

        // Регистрируем сигнал выпадения
        Container.DeclareSignal<ItemDropSignal>();
    }
}