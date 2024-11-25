using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Zenject;
using DG.Tweening;

public class ModelViewer : MonoBehaviour
{
    [SerializeField] private List<ModelDataPair> _models;
    [SerializeField] private Transform _target;
    [SerializeField] private float _rotationSpeed = 100.0f, _zoomSpeed = 2.0f, _minDistance = 5.0f, _maxDistance = 20.0f;
    [SerializeField] private float _animationDuration = 0.5f;

    private int _currentModelIndex = 0, _previousModelIndex = 0;
    private Vector3 _currentEulerAngles;
    private float _distanceToTarget;

    private bool _isFirstModel = true;
    private bool _isRotating = false;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    private void OnEnable() => _signalBus.Subscribe<EventInputSelectModel>(ShowModel);

    private void Start()
    {
        _distanceToTarget = Vector3.Distance(transform.position, _target.position);
        _currentEulerAngles = transform.eulerAngles;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            _isRotating = true;
        
        if (Input.GetMouseButtonUp(0))
            _isRotating = false;

        if (_isRotating)
        {
            float horizontal = Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;

            _currentEulerAngles.x += vertical;
            _currentEulerAngles.y += horizontal;
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0.0f)
            {
                _distanceToTarget -= scroll * _zoomSpeed;
                _distanceToTarget = Mathf.Clamp(_distanceToTarget, _minDistance, _maxDistance);
            }
        }

        Quaternion rotation = Quaternion.Euler(_currentEulerAngles);
        transform.position = _target.position - rotation * Vector3.forward * _distanceToTarget;
        transform.LookAt(_target);
    }

    private void ShowModel(EventInputSelectModel ctx) => ShowModel(ctx.modelIndex);

    private void ShowModel(int index)
    {
        if (index < 0 || index >= _models.Count) 
            return;

        if (_target != null && !_isFirstModel)
        {
            GameObject previous = _models[_previousModelIndex].Model;

            previous.transform.DOKill();
            previous.SetActive(false);

            _previousModelIndex = _currentModelIndex;

            GameObject currentModel = _models[_currentModelIndex].Model;

            currentModel.transform.DOScale(Vector3.zero, _animationDuration).OnComplete(() =>
            {
                currentModel.SetActive(false); 
            });
        }

        _currentModelIndex = index;
        GameObject nextModel = _models[_currentModelIndex].Model;
        ModelData nextModelData = _models[_currentModelIndex].ModelData;
        _target = nextModel.transform;

        if (!_isFirstModel)
            nextModel.transform.localScale = Vector3.zero;

        nextModel.SetActive(true);

        _signalBus.Fire(new EventNewModelSelected(nextModelData));

        Vector3 targetScale = nextModel.transform.localScale == Vector3.zero ? Vector3.one : nextModel.transform.localScale;
        nextModel.transform.DOScale(targetScale, _animationDuration);

        _isFirstModel = false;
    }

    private void OnDisable() => _signalBus.Unsubscribe<EventInputSelectModel>(ShowModel);
}

[Serializable]
public class ModelDataPair
{
    public GameObject Model;
    public ModelData ModelData;
}