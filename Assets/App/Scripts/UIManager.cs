using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform _menuRectTransform;
    [SerializeField] private RectTransform _trainsRectTransform;
    [SerializeField] private RectTransform _enterprisesRectTransform;
    [SerializeField] private RectTransform _trainsSelectRectTransform;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Color _solidColor;
    [SerializeField] private CinemachineVirtualCamera _cinemachineStartCamera;
    [SerializeField] private CinemachineVirtualCamera _cinemachineMainCamera;
    [SerializeField] private float _transitionDuration = 1f;

    [SerializeField] private Image _image;

    private bool _isInMainScene = false;

    void Start()
    {
        _mainCamera.clearFlags = CameraClearFlags.SolidColor;
        _mainCamera.backgroundColor = _solidColor;

        _cinemachineStartCamera.Priority = 10;
        _cinemachineMainCamera.Priority = 0;

        _trainsSelectRectTransform.anchoredPosition = new Vector2(-Screen.width, 0);
        _trainsRectTransform.anchoredPosition = new Vector2(-Screen.width, 0); // мб *2
        _enterprisesRectTransform.anchoredPosition = new Vector2(Screen.width, 0);

        _image.enabled = false;
    }

    public void OnStartButtonPressed()
    {
        if (_isInMainScene) return;

        _isInMainScene = true;

        // Завершаем текущие анимации интерфейса и цвета, чтобы избежать наложений
        _menuRectTransform.DOKill();
        _trainsSelectRectTransform.DOKill();
        _trainsRectTransform.DOKill();
        _mainCamera.DOKill();

        // Анимация перехода камеры на Skybox
        _mainCamera.DOColor(Color.black, _transitionDuration);
        _mainCamera.clearFlags = CameraClearFlags.Skybox;

        // Анимация интерфейса
        _menuRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), _transitionDuration);
        _trainsSelectRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), _transitionDuration);
        _trainsRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);

        // Переключение камеры
        _cinemachineStartCamera.Priority = 0;
        _cinemachineMainCamera.Priority = 10;
    }

    public void ReturnFromModelsToMain()
    {
        if (!_isInMainScene) return;

        _isInMainScene = false;

        // Завершаем текущие анимации интерфейса и цвета
        _menuRectTransform.DOKill();
        _trainsSelectRectTransform.DOKill();
        _trainsRectTransform.DOKill();
        _mainCamera.DOKill();

        // Анимация возврата камеры
        _menuRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
        _trainsSelectRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
        _trainsRectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), _transitionDuration);

        // Включаем solidColor только после завершения анимации
        _mainCamera.DOColor(_solidColor, _transitionDuration)
            .OnComplete(() => _mainCamera.clearFlags = CameraClearFlags.SolidColor);

        // Переключение камеры
        _cinemachineStartCamera.Priority = 10;
        _cinemachineMainCamera.Priority = 0;
    }

    public void ReturnFromModelsToSelect()
    {
        if (!_isInMainScene) return;

        _isInMainScene = false;

        // Завершаем текущие анимации интерфейса и цвета
        _menuRectTransform.DOKill();
        _trainsSelectRectTransform.DOKill();
        _trainsRectTransform.DOKill();
        _mainCamera.DOKill();

        // Анимация возврата камеры
        _menuRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), _transitionDuration);
        _trainsSelectRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
        _trainsRectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), _transitionDuration);

        // Включаем solidColor только после завершения анимации
        _mainCamera.DOColor(_solidColor, _transitionDuration)
            .OnComplete(() => _mainCamera.clearFlags = CameraClearFlags.SolidColor);

        // Переключение камеры
        _cinemachineStartCamera.Priority = 10;
        _cinemachineMainCamera.Priority = 0;
    }

    public void OpenEnterprisesWindow()
    {
        // Завершаем текущие анимации интерфейса
        _menuRectTransform.DOKill();
        _enterprisesRectTransform.DOKill();

        _menuRectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), _transitionDuration);
        _enterprisesRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
    }
    public void OpenTrainsSelectWindow()
    {
        // Завершаем текущие анимации интерфейса
        _menuRectTransform.DOKill();
        _trainsSelectRectTransform.DOKill();

        _menuRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), _transitionDuration);
        _trainsSelectRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
    }

    /*public void FromModelOpenTrainsSelectWindow()
    {
        // Завершаем текущие анимации интерфейса
        //_menuRectTransform.DOKill();
        _trainsRectTransform.DOKill();
        _trainsSelectRectTransform.DOKill();

        _trainsRectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), _transitionDuration);
        _trainsSelectRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
    }*/


    public void ReturnFromEnterprises()
    {
        // Завершаем текущие анимации интерфейса
        _menuRectTransform.DOKill();
        _enterprisesRectTransform.DOKill();

        _image.enabled = true;
        // белый фон во время анимации
        _menuRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration).OnComplete(() => { _image.enabled = false; });
        _enterprisesRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), _transitionDuration);
    }

    public void ReturnFromEnterprisesToSelect()
    {
        // Завершаем текущие анимации интерфейса
        _trainsSelectRectTransform.DOKill();
        _enterprisesRectTransform.DOKill();
        
        _trainsSelectRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
        _enterprisesRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), _transitionDuration);
    }

    public void ReturnFromTrainsSelect()
    {
        // Завершаем текущие анимации интерфейса
        _menuRectTransform.DOKill();
        _trainsSelectRectTransform.DOKill();

        _image.enabled = true;
        // белый фон во время анимации
        _menuRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration).OnComplete(() => { _image.enabled = false; });
        _trainsSelectRectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), _transitionDuration);
    }

    public void ReturnFromTrainsSelectToEnterprises()
    {
        // Завершаем текущие анимации интерфейса
        _enterprisesRectTransform.DOKill();
        _trainsSelectRectTransform.DOKill();

        _enterprisesRectTransform.DOAnchorPos(Vector2.zero, _transitionDuration);
        _trainsSelectRectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), _transitionDuration);
    }

    public void LoadEnterpriseScene()
    {
        SceneManager.LoadScene(SceneNames.TestEnterpriseScene);
    }
}
