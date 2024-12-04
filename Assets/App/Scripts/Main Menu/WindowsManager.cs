using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;

public class WindowsManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform _menuPanel;
    [SerializeField] private RectTransform _productsPanel;
    [SerializeField] private RectTransform _enterprisesPanel;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _startCamera;
    [SerializeField] private CinemachineVirtualCamera _mainCamera;

    [Header("Settings")]
    [SerializeField] private float _transitionDuration = 1f;

    private RectTransform _currentPanel;

    private void Start()
    {
        InitializeCameraPriorities();
        InitializePanelPositions();
        _currentPanel = _menuPanel; // Устанавливаем начальную панель
    }

    private void InitializeCameraPriorities()
    {
        SetCameraPriority(_startCamera, 10);
        SetCameraPriority(_mainCamera, 0);
    }

    private void InitializePanelPositions()
    {
        // Устанавливаем панели на начальные позиции
        SetPanelPosition(_menuPanel, Vector2.zero); // Центр
        SetPanelPosition(_productsPanel, new Vector2(-Screen.width, 0)); // Слева
        SetPanelPosition(_enterprisesPanel, new Vector2(Screen.width, 0)); // Справа
    }

    // === Методы открытия/закрытия окон ===

    public void OpenProductsWindow()
    {
        if (_currentPanel == _productsPanel) return;

        SwitchCameraPriority(_mainCamera, _startCamera);
        MovePanels(_productsPanel);
        _currentPanel = _productsPanel;
    }

    public void ReturnFromProducts()
    {
        if (_currentPanel == _menuPanel) return;

        SwitchCameraPriority(_startCamera, _mainCamera);
        MovePanels(_menuPanel);
        _currentPanel = _menuPanel;
    }

    public void OpenEnterprisesWindow()
    {
        if (_currentPanel == _enterprisesPanel) return;

        MovePanels(_enterprisesPanel);
        _currentPanel = _enterprisesPanel;
    }

    public void ReturnFromEnterprises()
    {
        if (_currentPanel == _menuPanel) return;

        MovePanels(_menuPanel);
        _currentPanel = _menuPanel;
    }

    public void LoadEnterpriseScene()
    {
        SceneManager.LoadScene(SceneNames.TestEnterpriseScene);
    }

    // === Вспомогательные методы ===

    private void MovePanels(RectTransform targetPanel)
    {
        // Рассчитываем смещение, чтобы выбранная панель была по центру
        float targetOffset = -targetPanel.anchoredPosition.x;

        // Останавливаем текущие анимации и начинаем новые
        _menuPanel.DOKill();
        _productsPanel.DOKill();
        _enterprisesPanel.DOKill();

        _menuPanel.DOAnchorPosX(_menuPanel.anchoredPosition.x + targetOffset, _transitionDuration);
        _productsPanel.DOAnchorPosX(_productsPanel.anchoredPosition.x + targetOffset, _transitionDuration);
        _enterprisesPanel.DOAnchorPosX(_enterprisesPanel.anchoredPosition.x + targetOffset, _transitionDuration);
    }

    private void SetCameraPriority(CinemachineVirtualCamera camera, int priority)
    {
        camera.Priority = priority;
    }

    private void SwitchCameraPriority(CinemachineVirtualCamera activeCamera, CinemachineVirtualCamera inactiveCamera)
    {
        SetCameraPriority(activeCamera, 10);
        SetCameraPriority(inactiveCamera, 0);
    }

    private void SetPanelPosition(RectTransform panel, Vector2 position)
    {
        panel.anchoredPosition = position;
    }
}