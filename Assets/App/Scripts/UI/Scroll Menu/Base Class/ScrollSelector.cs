using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class ScrollSelector : MonoBehaviour
{
    [SerializeField] protected RectTransform _selectPosition;
    [SerializeField] protected float _smoothTime = 0.2f;
    [SerializeField] protected float _scrollDeadZone = 0.1f;
    [SerializeField] protected float _delayBetweenElements = 0.25f;
    [SerializeField][Min(0)] protected int _startSelectedIndex = 0;

    public RectTransform Content;

    protected int _selectedIndex = -1;
    protected ScrollRect _scrollRect;
    protected List<ScrollElement> _elements = new();
    protected Vector3 _targetPosition;

    protected bool _isWatchingScroll = false;

    protected ScrollElement _targetElement;

    protected virtual void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.onValueChanged.AddListener(OnScroll);
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

    public void UpdateElements() 
    {
        InitializeElements();
        StartCoroutine(WaitAndSelect());
    }

    protected void InitializeElements()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            var child = Content.GetChild(i);
            var element = child.GetComponent<ScrollElement>();
            element.OnClicked -= SelectElement;
        }
        
        _elements.Clear();

        for (int i = 0; i < Content.childCount; i++)
        {
            var child = Content.GetChild(i);
            var element = child.GetComponent<ScrollElement>();

            if (element == null)
            {
                Debug.LogError($"Child at index {i} does not have a ScrollElement component!");
                continue;
            }

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
        if (index < 0 || index >= _elements.Count)
        {
            Debug.LogError($"Invalid index: {index}");
            return;
        }

        var element = _elements[index];
        if (element == null)
        {
            Debug.LogError($"Element at index {index} is null!");
            return;
        }

        if (_selectedIndex == index)
            return;

        DeselectElement(_selectedIndex);

        _targetElement = element;
        _targetPosition = CalculateTargetPosition(_targetElement.RectTransform.position);
        _targetElement.Select(); // Здесь вызывается метод, где возникает ошибка
        _selectedIndex = index;
    }

    protected void DeselectElement(int index)
    {
        if (index >= 0 && index < _elements.Count)
            _elements[index].Deselect();
    }

    protected Vector3 CalculateTargetPosition(Vector3 elementPosition)
    {
        return _selectPosition.position + (Content.position - elementPosition);
    }

    protected void SmoothScrollToSelected()
    {
        if (_selectedIndex < 0)
            return;

        _targetPosition = CalculateTargetPosition(_elements[_selectedIndex].RectTransform.position);

        if ((Content.position - _targetPosition).sqrMagnitude > _scrollDeadZone * _scrollDeadZone)
            Content.position = Vector3.Lerp(Content.position, _targetPosition, _smoothTime * Time.deltaTime);
    }

    protected IEnumerator WaitAndSelect()
    {
        yield return new WaitForSeconds(0.01f);

        if (_startSelectedIndex < 0 || _startSelectedIndex >= _elements.Count)
        {
            Debug.LogError($"StartSelectedIndex {_startSelectedIndex} is out of range!");
            yield break;
        }

        SelectElement(_startSelectedIndex);
    }

    protected void OnDisable()
    {
        _scrollRect.onValueChanged.RemoveListener(OnScroll);

        foreach (var element in _elements)
            element.OnClicked -= SelectElement;
    }
}
