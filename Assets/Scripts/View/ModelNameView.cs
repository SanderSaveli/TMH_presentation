using TMPro;
using UnityEngine;
using Zenject;
using System.Collections;


public class ModelNameView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }
    private void OnEnable()
    {
        _signalBus.Subscribe<EventNewModelSelected>(UpdateDescriotion);
    }
    private void OnDisable()
    {
        _signalBus.Unsubscribe<EventNewModelSelected>(UpdateDescriotion);
    }

    public void UpdateDescriotion(EventNewModelSelected ctx)
    {
        _text.text = ctx.modelData.Name;
    }
}
