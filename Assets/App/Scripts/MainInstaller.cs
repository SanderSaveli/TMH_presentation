using Zenject;

public class MainInstaller : MonoInstaller
{
    public CameraController cameraController;
    public ModelsManager modelManager;
    
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<EventNewModelSelected>();
        Container.DeclareSignal<EventInputSelectModel>();
        Container.DeclareSignal<EventModelDeSelected>();

        Container.Bind<ICameraController>().FromInstance(cameraController).AsSingle();
        Container.Bind<IModelManager>().FromInstance(modelManager).AsSingle();
    }
}
