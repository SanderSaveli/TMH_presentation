using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(ScrollRect))]

public class ScrollSelector : MonoBehaviour
{
    [SerializeField] private RectTransform _selectPosition;
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _smoothTime = 0.2f;
    [SerializeField] private float _scrollDeadZone = 0.1f;
    [SerializeField] private float _delayBetwenElelments = 0.25f;
    [SerializeField][Min(0)] protected int _startSelectedIndex = 0;

    private int _selectedIndex = -1;
    private ScrollRect _scrollRect;
    private List<ScrollElement> _elements;
    private Vector3 _targetPosition;

    private bool _isNeedToWatchTargetElement = false;

    public void OnEnable()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void OnDisable()
    {
        _scrollRect.onValueChanged.RemoveListener(OnScroll);
    }
    void Start()
    {
        _targetPosition = _content.position;
        TransitionEnd();
    }

    private void TransitionEnd()
    {
        FillAllElements();
        StartCoroutine(WaitAndSelect());
    }

    private IEnumerator WaitAndSelect()
    {
        yield return new WaitForSeconds(0.01f);
        SelectElement(_startSelectedIndex);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) || Input.mouseScrollDelta.y != 0)
        {
            _isNeedToWatchTargetElement = true;
        }
        else
        {
            _isNeedToWatchTargetElement = false;
            ScrollToElement();
        }
    }

    private void FillAllElements()
    {
        _elements = new List<ScrollElement>();
        for (int i = 0; i < _content.childCount; i++)
        {
            ScrollElement element = _content.GetChild(i).GetComponent<ScrollElement>();
            float delay = Mathf.Abs(_startSelectedIndex - i) * _delayBetwenElelments;
            element.Ini(i);
            _elements.Add(element);
            element.OnClicked += SelectElement;
        }
    }

    void OnScroll(Vector2 scrollPosition)
    {
        if (!_isNeedToWatchTargetElement)
        {
            return;
        }
        float minDistanceToCenter = float.MaxValue;
        int minElementIndex = 0;
        for (int i = 0; i < _elements.Count; i++)
        {
            RectTransform element = _elements[i].rectTransform;

            float distanceToCenter = Mathf.Abs((element.position - _selectPosition.position).magnitude);
            if (minDistanceToCenter > distanceToCenter)
            {
                minDistanceToCenter = distanceToCenter;
                minElementIndex = i;
            }
        }

        SelectElement(minElementIndex);
    }

    private void SelectElement(int index)
    {
        if (_selectedIndex == index)
        {
            return;
        }
        DeselectElement(_selectedIndex);
        ScrollElement targetElement = _elements[index];
        _targetPosition = _selectPosition.position + (_content.position - targetElement.rectTransform.position);
        targetElement.Select();
        _selectedIndex = index;
    }

    private void DeselectElement(int index)
    {
        if (index >= 0)
        {
            _elements[index].Deselect();
        }
    }

    public void ScrollToElement()
    {
        if (_selectedIndex < 0) return;
        _targetPosition = _selectPosition.position + (_content.position - _elements[_selectedIndex].rectTransform.position);
        if (Vector3.Magnitude(_content.position - _targetPosition) > _scrollDeadZone)
        {
            _content.position = Vector3.Lerp(_content.position, _targetPosition, _smoothTime * Time.deltaTime);
        }
    }
}
