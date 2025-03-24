using Zenject;
using UnityEngine;
using UnityEngine.UI;

public class TrainScrollElement : ScrollElement
{
    [SerializeField] private int _modelIndex;
    [SerializeField] private float _sizeIncrease; 
    [SerializeField] private Color _selectedColor = Color.white;
    [SerializeField] private Color _deselectedColor = Color.white;

    private SignalBus _signalBus;
    private Image _image;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Awake()
    {
        _image = GetComponent<Image>();

        if (_image != null)
        {
            _image.color = _deselectedColor;
        }
    }

    public override void Deselect()
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta -= new Vector2(_sizeIncrease, _sizeIncrease);
        }

        if (_image != null)
        {
            _image.color = _deselectedColor;
        }
    }

    public override void Select()
    {

        if (rectTransform != null)
        {
            rectTransform.sizeDelta += new Vector2(_sizeIncrease, _sizeIncrease);
        }

        if (_image != null)
        {
            _image.color = _selectedColor;
        }

        _signalBus.Fire(new EventInputSelectModel(_modelIndex));
    }
}
