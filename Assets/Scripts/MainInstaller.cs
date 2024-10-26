using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<EventNewModelSelected>();
        Container.DeclareSignal<EventInputSelectModel>();
    }
}
