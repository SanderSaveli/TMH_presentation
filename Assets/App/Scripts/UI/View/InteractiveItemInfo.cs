using UnityEngine;
using System.Collections;
using Zenject;
using TMPro;

public class InteractiveItemInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name, _description;
    [SerializeField] private GameObject _descriptionPanel;
    [SerializeField, Min(0.01f)] private float _charDelay = 0.01f;

    private SignalBus _signalBus;
    private Coroutine _currentCoroutine;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    private void Start()
    {
        if (_descriptionPanel == null || _name == null || _description == null)
        {
            Debug.LogError($"{nameof(InteractiveItemInfo)}: Missing required component references on {gameObject.name}.");
            return;
        }

        DisablePanel(null);
        _signalBus.Subscribe<EventNewModelSelected>(UpdateDescription);
        _signalBus.Subscribe<EventModelDeSelected>(DisablePanel);
    }

    public void DisablePanel(EventModelDeSelected ctx)
    {
        if (_descriptionPanel != null && _descriptionPanel.activeSelf)
            _descriptionPanel.SetActive(false);
    }

    public void UpdateDescription(EventNewModelSelected ctx)
    {
        if (ctx?.ModelData == null)
        {
            Debug.LogWarning($"{nameof(InteractiveItemInfo)}: ModelData is null.");
            return;
        }

        if (_descriptionPanel != null)
            _descriptionPanel.SetActive(true);

        if (_name != null)
            _name.text = ctx.ModelData.Name;

        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(AnimateText(ctx.ModelData.Description));
    }

    private IEnumerator AnimateText(string fullText)
    {
        if (_description == null)
            yield break;

        _description.text = "";

        foreach (char c in fullText)
        {
            _description.text += c;
            yield return new WaitForSeconds(_charDelay);
        }
    }

    private void OnDestroy()
    {
        if (_signalBus != null)
        {
            _signalBus.Unsubscribe<EventNewModelSelected>(UpdateDescription);
            _signalBus.Unsubscribe<EventModelDeSelected>(DisablePanel);
        }
    }
}