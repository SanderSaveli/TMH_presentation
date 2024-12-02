using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform _menuRectTransform;
    [SerializeField] private RectTransform _trainsRectTransform;
    [SerializeField] private RectTransform _enterprisesRectTransform;

    [SerializeField] private CinemachineVirtualCamera _cinemachineStartCamera;
    [SerializeField] private CinemachineVirtualCamera _cinemachineMainCamera;
    [SerializeField] private float _transitionDuration = 1f;

    private bool _isInMainScene = false;

    void Start()
    {
        _cinemachineStartCamera.Priority = 10;
        _cinemachineMainCamera.Priority = 0;

        _trainsRectTransform.anchoredPosition = new Vector2(-Screen.width, 0);
        _enterprisesRectTransform.anchoredPosition = new Vector2(Screen.width, 0);
    }

    public void OnStartButtonPressed()
    {
        if (_isInMainScene) return;

        _isInMainScene = true;

        // ��������� ������� �������� ���������� � �����, ����� �������� ���������
        _menuRectTransform.DOKill();
        _trainsRectTransform.DOKill();

        // �������� ����������
        _menuRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), _transitionDuration);
        _trainsRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);

        // ������������ ������
        _cinemachineStartCamera.Priority = 0;
        _cinemachineMainCamera.Priority = 10;
    }

    public void ReturnFromModels()
    {
        if (!_isInMainScene) return;

        _isInMainScene = false;

        // ��������� ������� �������� ���������� � �����
        _menuRectTransform.DOKill();
        _trainsRectTransform.DOKill();

        // �������� �������� ������
        _menuRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
        _trainsRectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), _transitionDuration);

        // ������������ ������
        _cinemachineStartCamera.Priority = 10;
        _cinemachineMainCamera.Priority = 0;
    }

    public void OpenEnterprisesVindow()
    {
        // ��������� ������� �������� ����������
        _menuRectTransform.DOKill();
        _enterprisesRectTransform.DOKill();

        _menuRectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), _transitionDuration);
        _enterprisesRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
    }

    public void ReturnFromEnterprises()
    {
        // ��������� ������� �������� ����������
        _menuRectTransform.DOKill();
        _enterprisesRectTransform.DOKill();

        _menuRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
        _enterprisesRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), _transitionDuration);
    }

    public void LoadEnterpriseScene()
    {
        SceneManager.LoadScene(SceneNames.TestEnterpriseScene);
    }
}
