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

    protected Button _button;
    protected Vector2 _originalSize;

    protected void Awake()
    {
        _button = GetComponent<Button>();
        RectTransform = GetComponent<RectTransform>();

        _button.interactable = false;

        _originalSize = RectTransform.sizeDelta;
    }

    public virtual void Initialize(int index)
    {
        Index = index;
    }

    public virtual void Deselect()
    {
        _button.interactable = false;

        if (RectTransform != null)
            RectTransform.sizeDelta = _originalSize;
    }

    public virtual void Select()
    {
        _button.interactable = true;

        if (RectTransform != null)
            RectTransform.sizeDelta = _originalSize + new Vector2(_sizeIncrease, _sizeIncrease);
    }

    public virtual void OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(Index);
}