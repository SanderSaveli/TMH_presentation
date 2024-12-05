using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public abstract class ScrollElement : MonoBehaviour, IPointerClickHandler
{
    public event Action<int> OnClicked;

    [SerializeField] private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private Image _buttonImage;

    protected int Index { get; private set; }

    [SerializeField, Min(0)] protected float _sizeIncrease = 10f; 
    [SerializeField] protected Color _selectedColor = Color.white, _deselectedColor = Color.gray;

    protected Vector2 _originalSize;

    protected void Awake()
    {
        if (_buttonImage == null)
            Debug.LogWarning($"{nameof(ScrollElement)}: Image component is missing on {gameObject.name}.");
        else
            _buttonImage.color = _deselectedColor;

        _originalSize = RectTransform.sizeDelta;
    }

    public virtual void Initialize(int index, Sprite icon, string name)
    {
        Index = index;
        _iconImage.sprite = icon;
        _buttonText.text = name;
    }

    public virtual void Deselect()
    {
        if (RectTransform != null)
            RectTransform.sizeDelta = _originalSize;

        if (_buttonImage != null)
            _buttonImage.color = _deselectedColor;
    }

    public virtual void Select()
    {
        if (RectTransform != null)
            RectTransform.sizeDelta = _originalSize + new Vector2(_sizeIncrease, _sizeIncrease);

        if (_buttonImage != null)
            _buttonImage.color = _selectedColor;
    }

    public virtual void OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(Index);
}