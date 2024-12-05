using UnityEngine;
using Zenject;

public class EnterpriseViewManager : MonoBehaviour
{
    private SignalBus _signalBus;
    private ICameraController _cameraController;

    [SerializeField] private ObjectInfo _enterpriseObjectInfo;
    [SerializeField] private Transform _enterpriseTarget;
    [SerializeField] private Vector3 _initialRotation;

    [Inject]
    public void Construct(SignalBus signalBus, ICameraController cameraController, IModelManager modelManager) 
    {
        _signalBus = signalBus;
        _cameraController = cameraController;
    }

    private void OnEnable() 
    {
        InteractableItem.OnInteract += OnInteractableItemClick;
    }

    private void Start() 
    {
        ResetTarget();
    }

    private void OnInteractableItemClick(ObjectInfo objectInfo, Transform target)
    {
        _signalBus.Fire(new EventNewModelSelected(objectInfo));
        _cameraController.SetTarget(target);
    }

    public void ResetTarget() 
    {
        _signalBus.Fire(new EventNewModelSelected(_enterpriseObjectInfo));
        _cameraController.SetTarget(_enterpriseTarget, _initialRotation);
    }
    
    private void OnDisable() 
    {
        InteractableItem.OnInteract -= OnInteractableItemClick;
    }
}
