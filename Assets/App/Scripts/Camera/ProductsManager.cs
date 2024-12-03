using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Zenject;

public class ProductsManager : MonoBehaviour, IModelManager
{
    [SerializeField] private List<ObjectInfoPair> _models;
    [SerializeField] private float _animationDuration = 0.5f;

    private SignalBus _signalBus;
    
    private int _currentModelIndex;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    private void Awake()
    {
        if (_models == null || _models.Count == 0)
            Debug.LogError("Products list is empty or null!");

        LoadCurrentProduct();
    }

    private void Start()
    {
        LoadCurrentProduct();
        ShowModel(_currentModelIndex);
    }

    public void ShowModel(int index)
    {
        if (index < 0 || index >= _models.Count)
        {
            Debug.LogError("Invalid model index!");
            return;
        }

        int previousModelIndex = _currentModelIndex;

        _currentModelIndex = index;

        GameObject currentModel = _models[previousModelIndex].Model;
        GameObject nextModel = _models[_currentModelIndex].Model;

        if (currentModel.activeSelf)
        {
            currentModel.transform.DOKill();
            currentModel.transform.DOScale(Vector3.zero, _animationDuration).OnComplete(() =>
            {
                currentModel.SetActive(false);
            });
        }

        nextModel.transform.DOKill();
        nextModel.transform.localScale = Vector3.zero;
        nextModel.SetActive(true);
        nextModel.transform.DOScale(Vector3.one, _animationDuration);

        _signalBus.Fire(new EventNewModelSelected(_models[_currentModelIndex].ObjectInfo));
    }

    public Transform GetCurrentModelTransform()
    {
        if (_models == null || _models.Count == 0 || _currentModelIndex >= _models.Count)
        {
            Debug.LogError("No valid model available!");
            return null;
        }

        return _models[_currentModelIndex].Model.transform;
    }

    public ObjectInfo GetCurrentObjectInfo()
    {
        if (_models == null || _models.Count == 0 || _currentModelIndex >= _models.Count)
        {
            Debug.LogError("No valid model data available!");
            return null;
        }

        return _models[_currentModelIndex].ObjectInfo;
    }

    public ObjectInfo GetObjectInfo(int index)
    {
        if (index < 0 || index >= _models.Count)
        {
            Debug.LogError("Invalid model index!");
            return null;
        }

        return _models[index].ObjectInfo;
    }

    private void LoadCurrentProduct() => _currentModelIndex = PlayerPrefsManager.LoadProductIndex() % 2; //Временно, пока поезда всего два, а кнопок 5
}

[System.Serializable]
public class ObjectInfoPair
{
    public GameObject Model;
    public ObjectInfo ObjectInfo;
}
