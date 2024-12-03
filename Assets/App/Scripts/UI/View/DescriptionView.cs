using UnityEngine;
using System.Collections;
using Zenject;
using TMPro;

public class DescriptionView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, Min(0.01f)] private float _charDelay = 0.01f;

    private SignalBus _signalBus;
    private Coroutine _currentCoroutine;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    private void OnEnable() => _signalBus.Subscribe<EventNewModelSelected>(UpdateDescription);

    public void UpdateDescription(EventNewModelSelected ctx)
    {
        if (_text == null)
        {
            Debug.LogWarning($"{nameof(DescriptionView)}: TMP_Text is not assigned in {gameObject.name}");
            return;
        }

        if (ctx?.ModelData?.Description == null)
        {
            Debug.LogWarning($"{nameof(DescriptionView)}: ModelData or Description is null.");
            return;
        }

        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(AnimateText(ctx.ModelData.Description));
    }

    private IEnumerator AnimateText(string fullText)
    {
        _text.text = "";

        foreach (char c in fullText)
        {
            _text.text += c;

            yield return new WaitForSeconds(_charDelay);
        }
    }

    private void OnDisable() => _signalBus.Unsubscribe<EventNewModelSelected>(UpdateDescription);
}