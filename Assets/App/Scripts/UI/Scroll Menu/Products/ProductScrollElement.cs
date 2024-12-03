using UnityEngine;
using Zenject;

public class ProductScrollElement : ScrollElement
{
    [SerializeField] private int _modelIndex;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    public override void Select()
    {
        base.Select();

        _signalBus.Fire(new EventInputSelectModel(_modelIndex));
    }
}