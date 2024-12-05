using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;

[RequireComponent(typeof(ScrollRect))]
public class ScrollSelector : MonoBehaviour
{
    [SerializeField] protected RectTransform _selectPosition;
    [SerializeField] protected RectTransform _content;
    [SerializeField] protected float _smoothTime = 0.2f;
    [SerializeField] protected float _scrollDeadZone = 0.1f;
    [SerializeField] protected float _delayBetweenElements = 0.25f;
    [SerializeField][Min(0)] protected int _startSelectedIndex = 0;

    protected int _selectedIndex = -1;
    protected ScrollRect _scrollRect;
    protected List<ScrollElement> _elements = new List<ScrollElement>();
    protected Vector3 _targetPosition;

    protected bool _isWatchingScroll = false;

    protected ScrollElement _targetElement;

    protected void OnEnable()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.onValueChanged.AddListener(OnScroll);
    }

    protected virtual void Start() 
    {
        InitializeElements();
        StartCoroutine(WaitAndSelect());
    }

    protected void Update()
    {
        if (Input.GetMouseButton(0) || Input.mouseScrollDelta.y != 0)
            _isWatchingScroll = true;
        else
        {
            _isWatchingScroll = false;
            SmoothScrollToSelected();
        }
    }

    protected void InitializeElements()
    {
        _elements.Clear();

        for (int i = 0; i < _content.childCount; i++)
        {
            ScrollElement element = _content.GetChild(i).GetComponent<ScrollElement>();
            element.Initialize(i);

            float delay = Mathf.Abs(_startSelectedIndex - i) * _delayBetweenElements;
            _elements.Add(element);
            element.OnClicked += SelectElement;
        }
    }

    protected void OnScroll(Vector2 scrollPosition)
    {
        if (!_isWatchingScroll)
            return;

        float minDistanceToCenter = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < _elements.Count; i++)
        {
            float distanceToCenter = (_elements[i].RectTransform.position - _selectPosition.position).sqrMagnitude;

            if (distanceToCenter < minDistanceToCenter)
            {
                minDistanceToCenter = distanceToCenter;
                closestIndex = i;
            }
        }

        SelectElement(closestIndex);
    }

    protected virtual void SelectElement(int index)
    {
        if (_selectedIndex == index)
            return;

        DeselectElement(_selectedIndex);

        _targetElement = _elements[index];
        _targetPosition = CalculateTargetPosition(_targetElement.RectTransform.position);
        _targetElement.Select();
        _selectedIndex = index;
    }

    protected void DeselectElement(int index)
    {
        if (index >= 0 && index < _elements.Count)
            _elements[index].Deselect();
    }

    protected Vector3 CalculateTargetPosition(Vector3 elementPosition)
    {
        return _selectPosition.position + (_content.position - elementPosition);
    }

    protected void SmoothScrollToSelected()
    {
        if (_selectedIndex < 0)
            return;

        _targetPosition = CalculateTargetPosition(_elements[_selectedIndex].RectTransform.position);

        if ((_content.position - _targetPosition).sqrMagnitude > _scrollDeadZone * _scrollDeadZone)
            _content.position = Vector3.Lerp(_content.position, _targetPosition, _smoothTime * Time.deltaTime);
    }

    public void SelectRelativeElement(int offset) => SelectElement(Mathf.Clamp(_selectedIndex + offset, 0, _elements.Count - 1));

    protected IEnumerator WaitAndSelect()
    {
        yield return new WaitForSeconds(0.01f);
        SelectElement(_startSelectedIndex);
    }

    protected void OnDisable()
    {
        _scrollRect.onValueChanged.RemoveListener(OnScroll);

        foreach (var element in _elements)
            element.OnClicked -= SelectElement;
    }
}
