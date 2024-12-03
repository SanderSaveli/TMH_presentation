using UnityEngine;
using Zenject;
using TMPro;

public class ModelNameView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    private void Start()
    {
        if (_text == null)
            Debug.LogError($"{nameof(ModelNameView)}: Missing TMP_Text reference on {gameObject.name}.");
    }

    private void OnEnable()
    {
        if (_signalBus != null)
            _signalBus.Subscribe<EventNewModelSelected>(UpdateDescription);
        else
            Debug.LogError($"{nameof(ModelNameView)}: SignalBus is not injected.");
    }

    public void UpdateDescription(EventNewModelSelected ctx)
    {
        if (_text == null)
        {
            Debug.LogWarning($"{nameof(ModelNameView)}: TMP_Text is not assigned.");
            return;
        }

        if (ctx?.ModelData == null)
        {
            Debug.LogWarning($"{nameof(ModelNameView)}: ModelData is null.");
            _text.text = "No Model Selected";
            return;
        }

        _text.text = ctx.ModelData.Name;
    }

    private void OnDisable()
    {
        if (_signalBus != null)
            _signalBus.Unsubscribe<EventNewModelSelected>(UpdateDescription);
    }
}