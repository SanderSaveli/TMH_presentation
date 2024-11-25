using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UITracker : MonoBehaviour
{
    public RectTransform startPointUI; // ����� �� UI, ������ ���������� �����
    public LineRenderer lineRenderer; // LineRenderer ��� ����������� �����
    public Camera mainCamera; // ������ ����� ��� �������������� ������� ��������� � ��������
    public Transform RayStart; // ����� ������ ���� (��� �����)

    private InteractableItem activeItem; // �������� ������������� ������
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            HandleClick();
        }

        if (activeItem != null)
        {
            UpdateLine();
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ���������, ����� �� Ray � ������������� ������
        if (Physics.Raycast(ray, out hit))
        {
            // ���������, ���� �� ��������� InteractableItem �� �������
            InteractableItem item = hit.collider.GetComponent<InteractableItem>();
            if (item != null)
            {
                // ���� ����� ������������� ������, ������ ��� ��������
                SetActiveItem(item);
            }
        }
    }

    void SetActiveItem(InteractableItem item)
    {
        if(activeItem == item)
        {
            activeItem = null;
            _signalBus.Fire(new EventModelDeSelected());
            return;
        }
        _signalBus.Fire(new EventNewModelSelected(item.Data));
        activeItem = item;
    }

    void UpdateLine()
    {
        Vector3 endPointWorld = activeItem.transform.position;

        // ������������� ������� ����� � ������� �����������
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, RayStart.transform.position);
        lineRenderer.SetPosition(1, endPointWorld);

        // ������ ����� ���������� � ����� (��������, �� ������ 0.1 �� 0.01)
        float distance = Vector3.Distance(RayStart.transform.position, endPointWorld);
        lineRenderer.startWidth = 0.5f; // ��������� ������ �����
        lineRenderer.endWidth = Mathf.Lerp(0.5f, 0.01f, distance / 10f); // �������� ������ ����� (�������� � ����������� ����������)
    }
}
