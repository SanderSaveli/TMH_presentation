using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class InterractiveItemItemInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private GameObject _descriptionPanel;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Start()
    {
        DisablePanel(null);
        _signalBus.Subscribe<EventNewModelSelected>(UpdateDescriotion);
        _signalBus.Subscribe<EventModelDeSelected>(DisablePanel);
    }
    private void OnDestroy()
    {
        _signalBus.Unsubscribe<EventNewModelSelected>(UpdateDescriotion);
        _signalBus.Unsubscribe<EventModelDeSelected>(DisablePanel);
    }

    public void DisablePanel(EventModelDeSelected ctx)
    {
        _descriptionPanel.SetActive(false);
    }

    public void UpdateDescriotion(EventNewModelSelected ctx)
    {
        _descriptionPanel.SetActive(true);
        _name.text = ctx.modelData.Name;
        StartCoroutine(AnimateText(ctx.modelData.Description));
    }

    private IEnumerator AnimateText(string fullText)
    {
        _description.text = "";
        foreach (char c in fullText)
        {
            _description.text += c;
            yield return new WaitForSeconds(0.01f);

        }
    }
}
