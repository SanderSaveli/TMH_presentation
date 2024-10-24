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
        _signalBus.Subscribe<NewModelSelected>(UpdateDescriotion);
    }
    private void OnDisable()
    {
        _signalBus.Unsubscribe<NewModelSelected>(UpdateDescriotion);
    }

    public void UpdateDescriotion(NewModelSelected ctx)
    {
        _text.text = ctx.modelData.Name;
    }
}
