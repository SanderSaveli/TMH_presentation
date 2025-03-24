using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Zenject;

public class ModelsManager : MonoBehaviour, IModelManager
{
    [SerializeField] private List<ModelDataPair> _models;
    [SerializeField] private float _animationDuration = 0.5f;

    private SignalBus _signalBus;
    
    private int _currentModelIndex = 0;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    private void Awake()
    {
        if (_models == null || _models.Count == 0)
            Debug.LogError("Model list is empty or null!");
    }

    private void Start()
    {
        LoadCurrentModel();
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

        _signalBus.Fire(new EventNewModelSelected(_models[_currentModelIndex].ModelData));

        Debug.Log($"Model switched to index: {_currentModelIndex}, Name: {_models[_currentModelIndex].ModelData.Name}");

        SaveCurrentModel();
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

    public ModelData GetCurrentModelData()
    {
        if (_models == null || _models.Count == 0 || _currentModelIndex >= _models.Count)
        {
            Debug.LogError("No valid model data available!");
            return null;
        }

        return _models[_currentModelIndex].ModelData;
    }

    public ModelData GetModelData(int index)
    {
        if (index < 0 || index >= _models.Count)
        {
            Debug.LogError("Invalid model index!");
            return null;
        }

        return _models[index].ModelData;
    }

    private void SaveCurrentModel()
    {
        PlayerPrefs.SetInt("CurrentModelIndex", _currentModelIndex);
        PlayerPrefs.Save();
    }

    private void LoadCurrentModel() => _currentModelIndex = PlayerPrefs.GetInt("CurrentModelIndex", 0);
}

[System.Serializable]
public class ModelDataPair
{
    public GameObject Model;
    public ModelData ModelData;
}
