using Zenject;

public class MainInstaller : MonoInstaller
{
    public CameraController CameraController;
    public ModelsManager ModelManager;
    
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<EventNewModelSelected>();
        Container.DeclareSignal<EventInputSelectModel>();
        Container.DeclareSignal<EventModelDeSelected>();

        Container.Bind<ICameraController>().FromInstance(CameraController).AsSingle();
        Container.Bind<IModelManager>().FromInstance(ModelManager).AsSingle();
    }
}
