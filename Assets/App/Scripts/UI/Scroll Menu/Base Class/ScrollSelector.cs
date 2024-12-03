using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class ScrollSelector : MonoBehaviour
{
    [SerializeField] protected RectTransform selectPosition;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected float smoothTime = 0.2f;
    [SerializeField] protected float scrollDeadZone = 0.1f;
    [SerializeField] protected float delayBetweenElements = 0.25f;
    [SerializeField][Min(0)] protected int startSelectedIndex = 0;

    protected int selectedIndex = -1;
    protected ScrollRect scrollRect;
    protected List<ScrollElement> elements = new();
    protected Vector3 targetPosition;

    protected bool isWatchingScroll = false;

    protected void OnEnable()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    protected void Awake()
    {
        InitializeElements();
        StartCoroutine(WaitAndSelect());
    }

    protected void Update()
    {
        if (Input.GetMouseButton(0) || Input.mouseScrollDelta.y != 0)
            isWatchingScroll = true;
        else
        {
            isWatchingScroll = false;
            SmoothScrollToSelected();
        }
    }

    protected void InitializeElements()
    {
        elements.Clear();

        for (int i = 0; i < content.childCount; i++)
        {
            ScrollElement element = content.GetChild(i).GetComponent<ScrollElement>();
            element.Initialize(i);

            float delay = Mathf.Abs(startSelectedIndex - i) * delayBetweenElements;
            elements.Add(element);
            element.OnClicked += SelectElement;
        }
    }

    protected void OnScroll(Vector2 scrollPosition)
    {
        if (!isWatchingScroll)
            return;

        float minDistanceToCenter = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            float distanceToCenter = (elements[i].RectTransform.position - selectPosition.position).sqrMagnitude;

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
        if (selectedIndex == index)
            return;

        DeselectElement(selectedIndex);

        ScrollElement targetElement = elements[index];
        targetPosition = CalculateTargetPosition(targetElement.RectTransform.position);
        targetElement.Select();
        selectedIndex = index;
    }

    protected void DeselectElement(int index)
    {
        if (index >= 0 && index < elements.Count)
            elements[index].Deselect();
    }

    protected Vector3 CalculateTargetPosition(Vector3 elementPosition)
    {
        return selectPosition.position + (content.position - elementPosition);
    }

    protected void SmoothScrollToSelected()
    {
        if (selectedIndex < 0)
            return;

        targetPosition = CalculateTargetPosition(elements[selectedIndex].RectTransform.position);

        if ((content.position - targetPosition).sqrMagnitude > scrollDeadZone * scrollDeadZone)
            content.position = Vector3.Lerp(content.position, targetPosition, smoothTime * Time.deltaTime);
    }

    protected IEnumerator WaitAndSelect()
    {
        yield return new WaitForSeconds(0.01f);
        SelectElement(startSelectedIndex);
    }

    protected void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScroll);

        foreach (var element in elements)
            element.OnClicked -= SelectElement;
    }
}
