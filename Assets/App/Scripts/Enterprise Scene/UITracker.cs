using UnityEngine;
using Zenject;

public class UITracker : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform rayStart;

    private InteractableItem _activeItem;
    private SignalBus _signalBus;
    private ILineManager _lineManager;

    [Inject]
    public void Construct(SignalBus signalBus, ILineManager lineManager)
    {
        _signalBus = signalBus;
        _lineManager = lineManager;
    }

    private void Update()
    {
        HandleInput();

        if (_activeItem != null)
            _lineManager.UpdateLine(rayStart.position, _activeItem.transform.position);
        else
            _lineManager.ClearLine();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                InteractableItem item = hit.collider.GetComponent<InteractableItem>();
                if (item != null)
                    SetActiveItem(item);
            }
        }
    }

    private void SetActiveItem(InteractableItem item)
    {
        if (_activeItem == item)
        {
            _activeItem = null;
            _signalBus.Fire(new EventModelDeSelected());
            return;
        }

        _activeItem = item;
        _signalBus.Fire(new EventNewModelSelected(item.Data));
    }
}
