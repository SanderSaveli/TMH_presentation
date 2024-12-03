using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public abstract class ScrollElement : MonoBehaviour, IPointerClickHandler
{
    public event Action<int> OnClicked;

    public RectTransform RectTransform { get; private set; }

    protected int Index { get; private set; }
    
    [SerializeField, Min(0)] protected float _sizeIncrease = 10f; 
    [SerializeField] protected Color _selectedColor = Color.white;
    [SerializeField] protected Color _deselectedColor = Color.gray;

    protected Image _image;
    protected Vector2 _originalSize;

    protected virtual void Start()
    {
        _image = GetComponent<Image>();

        if (_image == null)
            Debug.LogWarning($"{nameof(ScrollElement)}: Image component is missing on {gameObject.name}.");
        else
            _image.color = _deselectedColor;

        _originalSize = RectTransform.sizeDelta;
    }

    public virtual void Initialize(int index)
    {
        RectTransform = GetComponent<RectTransform>();
        Index = index;
    }

    public virtual void Deselect()
    {
        if (RectTransform != null)
            RectTransform.sizeDelta = _originalSize;

        if (_image != null)
            _image.color = _deselectedColor;
    }

    public virtual void Select()
    {
        if (RectTransform != null)
            RectTransform.sizeDelta = _originalSize + new Vector2(_sizeIncrease, _sizeIncrease);

        if (_image != null)
            _image.color = _selectedColor;
    }

    public virtual void OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(Index);
}