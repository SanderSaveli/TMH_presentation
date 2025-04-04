using TMPro;
using UnityEngine;
using Zenject;
using System.Collections;

public class DescriptionView : MonoBehaviour
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
        _signalBus.Subscribe<EventNewModelSelected>(UpdateDescription);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<EventNewModelSelected>(UpdateDescription);
    }

    public void UpdateDescription(EventNewModelSelected ctx)
    {
        StopAllCoroutines();

        StartCoroutine(AnimateText(ctx.modelData.Description));
    }

    private IEnumerator AnimateText(string fullText)
    {
        _text.text = "";
        foreach (char c in fullText)
        {
            _text.text += c;
            yield return new WaitForSeconds(0.01f); 

        }
    }
}
