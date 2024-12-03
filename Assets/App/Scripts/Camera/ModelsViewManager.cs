using UnityEngine;
using Zenject;

public class ProductsViewManager : MonoBehaviour
{
    private SignalBus _signalBus;
    private ICameraController _cameraController;
    private IModelManager _modelManager;

    [Inject]
    public void Construct(SignalBus signalBus, ICameraController cameraController, IModelManager modelManager)
    {
        _signalBus = signalBus;
        _cameraController = cameraController;
        _modelManager = modelManager;
    }

    private void OnEnable() => _signalBus.Subscribe<EventInputSelectModel>(OnModelSelected);

    private void Start() => _cameraController.SetTarget(_modelManager.GetCurrentModelTransform());

    private void OnModelSelected(EventInputSelectModel ctx)
    {
        _modelManager.ShowModel(ctx.ModelIndex);
        _cameraController.SetTarget(_modelManager.GetCurrentModelTransform());
    }

    private void OnDisable() => _signalBus.Unsubscribe<EventInputSelectModel>(OnModelSelected);
}