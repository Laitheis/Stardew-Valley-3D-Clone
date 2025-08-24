using Zenject;

public class ServicesInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Регистрируем сервис
        Container.BindInterfacesAndSelfTo<LootService>().AsSingle();

        // Включаем сигналы
        SignalBusInstaller.Install(Container);

        // Регистрируем сигнал выпадения
        Container.DeclareSignal<LootDropSignal>();
    }
}