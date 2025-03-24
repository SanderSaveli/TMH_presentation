using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UITracker : MonoBehaviour
{
    public RectTransform startPointUI; // Точка на UI, откуда начинается линия
    public LineRenderer lineRenderer; // LineRenderer для отображения линии
    public Camera mainCamera; // Камера сцены для преобразования мировых координат в экранные
    public Transform RayStart; // Точка начала луча (или линии)

    private InteractableItem activeItem; // Активный интерактивный объект
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

        // Проверяем, попал ли Ray в интерактивный объект
        if (Physics.Raycast(ray, out hit))
        {
            // Проверяем, есть ли компонент InteractableItem на объекте
            InteractableItem item = hit.collider.GetComponent<InteractableItem>();
            if (item != null)
            {
                // Если нашли интерактивный объект, делаем его активным
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

        // Устанавливаем позиции линии в мировых координатах
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, RayStart.transform.position);
        lineRenderer.SetPosition(1, endPointWorld);

        // Делаем линию сужающейся к концу (например, от ширины 0.1 до 0.01)
        float distance = Vector3.Distance(RayStart.transform.position, endPointWorld);
        lineRenderer.startWidth = 0.5f; // Начальная ширина линии
        lineRenderer.endWidth = Mathf.Lerp(0.5f, 0.01f, distance / 10f); // Конечная ширина линии (сужается с увеличением расстояния)
    }
}
