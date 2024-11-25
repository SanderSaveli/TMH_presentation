using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Zenject;
using UnityEngine.EventSystems;

[Serializable]
public class ModelDataPair
{
    public GameObject model;
    public ModelData modelData;
}
public class ModelViewer : MonoBehaviour
{
    public List<ModelDataPair> models; // Список всех моделей
    public Transform target; // Текущая целевая модель
    public float rotationSpeed = 100.0f; // Скорость вращения
    public float zoomSpeed = 2.0f; // Скорость изменения расстояния (зум)
    public float minDistance = 5.0f; // Минимальное расстояние до модели
    public float maxDistance = 20.0f; // Максимальное расстояние до модели

    private int currentModelIndex = 0; // Индекс текущей модели
    private int previousModelIndex = 0; // Индекс текущей модели
    private Vector3 currentEulerAngles; // Текущий угол камеры
    private float distanceToTarget; // Текущее расстояние до модели

    public float animationDuration = 0.5f; // Длительность анимации появления и исчезновения
    private bool isFirstModel = true; // Флаг для отслеживания первого показа модели
    private bool isRotating;

    private SignalBus _signalBus;
    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    void Start()
    {
        distanceToTarget = Vector3.Distance(transform.position, target.position);
        currentEulerAngles = transform.eulerAngles;
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<EventInputSelectModel>(ShowModel);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<EventInputSelectModel>(ShowModel);
    }

    void Update()
    {
        // Начинаем вращение, если нажата левая кнопка мыши и курсор не над UI
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isRotating = true;
        }
        // Прекращаем вращение при отпускании кнопки мыши
        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        // Выполняем вращение, если активен флаг isRotating
        if (isRotating)
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentEulerAngles.x += vertical;
            currentEulerAngles.y += horizontal;

            Quaternion rotation = Quaternion.Euler(currentEulerAngles);
            transform.position = target.position - rotation * Vector3.forward * distanceToTarget;
            transform.LookAt(target);
        }

        // Зум работает только при отсутствии взаимодействия с UI
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                distanceToTarget -= scroll * zoomSpeed;
                distanceToTarget = Mathf.Clamp(distanceToTarget, minDistance, maxDistance);

                Quaternion rotation = Quaternion.Euler(currentEulerAngles);
                transform.position = target.position - rotation * Vector3.forward * distanceToTarget;
            }
        }
    }

    private void ShowModel(EventInputSelectModel ctx)
    {
        ShowModel(ctx.modelIndex);
    }
    void ShowModel(int index)
    {
        if (index < 0 || index >= models.Count) return;

        if (target != null && !isFirstModel)
        {
            GameObject previous = models[previousModelIndex].model;
            previous.transform.DOKill();
            previous.SetActive(false);
            previousModelIndex = currentModelIndex;

            GameObject currentModel = models[currentModelIndex].model;
            currentModel.transform.DOScale(Vector3.zero, animationDuration).OnComplete(() =>
            {
                currentModel.SetActive(false); 
            });
        }

        currentModelIndex = index;
        GameObject nextModel = models[currentModelIndex].model;
        ModelData nextModelData = models[currentModelIndex].modelData;
        target = nextModel.transform;

        if (!isFirstModel)
        {
            nextModel.transform.localScale = Vector3.zero;
        }

        nextModel.SetActive(true);

        _signalBus.Fire(new EventNewModelSelected(nextModelData));

        Vector3 targetScale = nextModel.transform.localScale == Vector3.zero ? new Vector3(1, 1, 1) : nextModel.transform.localScale;
        nextModel.transform.DOScale(targetScale, animationDuration);

        isFirstModel = false;
    }
}
