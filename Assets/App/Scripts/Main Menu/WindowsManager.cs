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

    private bool _isInMainScene = false;

    private void Start()
    {
        InitializeCameraPriorities();
        InitializePanelPositions();
    }

    private void InitializeCameraPriorities()
    {
        SetCameraPriority(_startCamera, 10);
        SetCameraPriority(_mainCamera, 0);
    }

    private void InitializePanelPositions()
    {
        SetPanelPosition(_productsPanel, new Vector2(-Screen.width, 0));
        SetPanelPosition(_enterprisesPanel, new Vector2(Screen.width, 0));
    }

    public void OpenProductsWindow()
    {
        if (_isInMainScene) 
            return;

        _isInMainScene = true;
        SwitchToPanel(_menuPanel, _productsPanel, new Vector2(Screen.width, 0), Vector2.zero);
        SwitchCameraPriority(_mainCamera, _startCamera);
    }

    public void ReturnFromProducts()
    {
        if (!_isInMainScene) 
            return;

        _isInMainScene = false;
        SwitchToPanel(_productsPanel, _menuPanel, new Vector2(-Screen.width, 0), Vector2.zero);
        SwitchCameraPriority(_startCamera, _mainCamera);
    }

    public void OpenEnterprisesWindow() => SwitchToPanel(_menuPanel, _enterprisesPanel, new Vector2(-Screen.width, 0), Vector2.zero);

    public void ReturnFromEnterprises() => SwitchToPanel(_enterprisesPanel, _menuPanel, new Vector2(Screen.width, 0), Vector2.zero);

    public void LoadEnterpriseScene() => SceneManager.LoadScene(SceneNames.TestEnterpriseScene);

    // === Helper Methods ===

    private void SetCameraPriority(CinemachineVirtualCamera camera, int priority) => camera.Priority = priority;

    private void SwitchCameraPriority(CinemachineVirtualCamera activeCamera, CinemachineVirtualCamera inactiveCamera)
    {
        SetCameraPriority(activeCamera, 10);
        SetCameraPriority(inactiveCamera, 0);
    }

    private void SetPanelPosition(RectTransform panel, Vector2 position) => panel.anchoredPosition = position;

    private void SwitchToPanel(RectTransform fromPanel, RectTransform toPanel, Vector2 fromTarget, Vector2 toTarget)
    {
        fromPanel.DOKill();
        toPanel.DOKill();

        fromPanel.DOAnchorPos(fromTarget, _transitionDuration);
        toPanel.DOAnchorPos(toTarget, _transitionDuration);
    }
}
