using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using Zenject;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ProductsManager : MonoBehaviour, IModelManager
{
    public static ProductsManager Instance { get; private set; }

    [SerializeField] private List<ObjectInfo> _models;
    [SerializeField] private Transform _viewport; // Viewport внутри Scroll View
    [SerializeField] private ScrollRect _scrollView;
    [SerializeField] private ScrollSelector _scrollSelector;
    [SerializeField] private Transform _categoryButtonsParent; // Grid Layout Group для кнопок категорий
    [SerializeField] private GameObject _contentPrefab; // Префаб Content
    [SerializeField] private GameObject _buttonPrefab; // Префаб кнопки продуктов
    [SerializeField] private GameObject _categoryButtonPrefab; // Префаб кнопки категории
    [SerializeField] private Transform _spawnPoint; // Точка для появления модели
    [SerializeField] private float _animationDuration = 0.5f;

    private SignalBus _signalBus;

    private Dictionary<ObjectInfo.ObjectCategory, Transform> _categoryContents = new Dictionary<ObjectInfo.ObjectCategory, Transform>(); // Сопоставление категории и Content
    private Dictionary<GameObject, GameObject> _spawnedModels = new Dictionary<GameObject, GameObject>(); // Сохраненные модели
    private GameObject _currentModel; // Текущая активная модель
    private ObjectInfo _currentModelInfo; // Храним информацию о текущей модели
    private KeyValuePair<ObjectInfo.ObjectCategory, Transform> _firstCategory;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        if (_models == null || _models.Count == 0)
            Debug.LogError("Products list is empty or null!");

        GenerateCategoriesAndButtons();
        GenerateCategoryButtons();
        SelectCategory(_firstCategory.Key);
    }

    private void GenerateCategoriesAndButtons()
    {
        // Сортируем _models по категориям и по имени внутри каждой категории
        var groupedModels = _models
            .GroupBy(model => model.Category)
            .ToDictionary(group => group.Key, group => group.OrderBy(model => model.Name).ToList());

        foreach (var category in groupedModels)
        {
            // Создаем объект Content для категории
            var content = Instantiate(_contentPrefab, _viewport);
            content.name = $"{category.Key}Content";
            content.gameObject.SetActive(false);
            _categoryContents[category.Key] = content.transform;

            // Создаем кнопки для каждой модели внутри категории
            var modelsInCategory = category.Value;
            for (int i = 0; i < modelsInCategory.Count; i++)
            {
                var modelInfo = modelsInCategory[i];
                var button = Instantiate(_buttonPrefab, content.transform);
                var productScrollElement = button.GetComponent<ProductScrollElement>();

                productScrollElement.Initialize(
                    i,
                    modelInfo.Icon,
                    modelInfo.Name,
                    modelInfo.ModelPrefab
                );
            }
        }

        // Активируем первую категорию по умолчанию
        if (_categoryContents.Count > 0)
        {
            _firstCategory = _categoryContents.First();
            _firstCategory.Value.gameObject.SetActive(true);
        }
    }

    private void GenerateCategoryButtons()
    {
        foreach (var category in _categoryContents.Keys)
        {
            // Создаем кнопку категории
            var button = Instantiate(_categoryButtonPrefab, _categoryButtonsParent);
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = category.ToString(); // Название категории

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectCategory(category);
            });
        }
    }

    private void SelectCategory(ObjectInfo.ObjectCategory category)
    {
        GameObject contentGameObject = null;

        foreach (var content in _categoryContents)
        {
            contentGameObject = content.Value.gameObject;
            contentGameObject.SetActive(content.Key == category);
        }

        var scrollSelector = _categoryContents[category].GetComponentInParent<ScrollSelector>();
        if (scrollSelector != null)
        {
            scrollSelector.Content = contentGameObject.GetComponent<RectTransform>();
            _scrollView.content = contentGameObject.GetComponent<RectTransform>();
            scrollSelector.UpdateElements();
        }
    }

    public void ShowModel(GameObject modelPrefab)
    {
        if (_spawnedModels.ContainsKey(modelPrefab))
        {
            if (_currentModel != null)
                HideModel();

            _currentModel = _spawnedModels[modelPrefab];
        }
        else
        {
            if (_currentModel != null)
                HideModel();

            _currentModel = Instantiate(modelPrefab, _spawnPoint.position, Quaternion.identity);
            _spawnedModels[modelPrefab] = _currentModel;
        }

        _currentModelInfo = _models.FirstOrDefault(model => model.ModelPrefab == modelPrefab);

        PlayAppearAnimation(_currentModel);

        CameraController.Instance.SetTarget(modelPrefab.transform);

        if (_currentModelInfo != null)
        {
            _signalBus.Fire(new EventNewModelSelected(_currentModelInfo));
        }
    }

    private void HideModel()
    {
        if (_currentModel == null)
            return;

        PlayDisappearAnimation(_currentModel);
    }

    private void PlayAppearAnimation(GameObject model)
    {
        model.transform.DOKill();
        model.transform.localScale = Vector3.zero;
        model.SetActive(true);
        model.transform.DOScale(Vector3.one, _animationDuration);
    }

    private void PlayDisappearAnimation(GameObject model)
    {
        if (model.activeSelf)
        {
            model.transform.DOKill();
            model.transform.DOScale(Vector3.zero, _animationDuration).OnComplete(() =>
            {
                model.SetActive(false);
            });
        }
    }

    public Transform GetCurrentModelTransform()
    {
        return _currentModel.transform;
    }

    public ObjectInfo GetCurrentObjectInfo()
    {
        return _currentModelInfo;
    }

    public ObjectInfo GetObjectInfo(int index)
    {
        if (index < 0 || index >= _models.Count)
        {
            Debug.LogError("Invalid model index!");
            return null;
        }

        return _models[index];
    }
}
