using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Zenject;

[Serializable]
public class ModelDataPair
{
    public GameObject model;
    public ModelData modelData;
}
public class ModelViewer : MonoBehaviour
{
    public List<ModelDataPair> models; // ������ ���� �������
    public Transform target; // ������� ������� ������
    public float rotationSpeed = 100.0f; // �������� ��������
    public float zoomSpeed = 2.0f; // �������� ��������� ���������� (���)
    public float minDistance = 5.0f; // ����������� ���������� �� ������
    public float maxDistance = 20.0f; // ������������ ���������� �� ������

    private int currentModelIndex = 0; // ������ ������� ������
    private Vector3 currentEulerAngles; // ������� ���� ������
    private float distanceToTarget; // ������� ���������� �� ������

    public Button nextButton; // ������ "������"
    public Button prevButton; // ������ "�����"

    public float animationDuration = 0.5f; // ������������ �������� ��������� � ������������
    private bool isFirstModel = true; // ���� ��� ������������ ������� ������ ������
    private SignalBus _signalBus;
    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    void Start()
    {
        if (models.Count > 0)
        {
            ShowModel(0);
        }

        distanceToTarget = Vector3.Distance(transform.position, target.position);
        currentEulerAngles = transform.eulerAngles;

        nextButton.onClick.AddListener(NextModel);
        prevButton.onClick.AddListener(PreviousModel);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentEulerAngles.x += vertical;
            currentEulerAngles.y += horizontal;

            // ��������� ��������� ������ �� ������ �����
            Quaternion rotation = Quaternion.Euler(currentEulerAngles);
            transform.position = target.position - rotation * Vector3.forward * distanceToTarget;
            transform.LookAt(target);
        }

        // ���������� �����
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            distanceToTarget -= scroll * zoomSpeed;
            distanceToTarget = Mathf.Clamp(distanceToTarget, minDistance, maxDistance);

            // ��������� ��������� ������ ����� ��������� ����������
            Quaternion rotation = Quaternion.Euler(currentEulerAngles);
            transform.position = target.position - rotation * Vector3.forward * distanceToTarget;
        }
    }

    void ShowModel(int index)
    {
        if (index < 0 || index >= models.Count) return;

        if (target != null && !isFirstModel)
        {
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

        _signalBus.Fire(new NewModelSelected(nextModelData));

        Vector3 targetScale = nextModel.transform.localScale == Vector3.zero ? new Vector3(1, 1, 1) : nextModel.transform.localScale;
        nextModel.transform.DOScale(targetScale, animationDuration);

        isFirstModel = false;
    }

    void NextModel()
    {
        int nextIndex = (currentModelIndex + 1) % models.Count;
        ShowModel(nextIndex);
    }

    void PreviousModel()
    {
        int prevIndex = (currentModelIndex - 1 + models.Count) % models.Count;
        ShowModel(prevIndex);
    }
}
