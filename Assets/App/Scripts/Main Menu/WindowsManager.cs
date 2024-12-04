using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class WindowsManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _menuPanelGroup;
    [SerializeField] private CanvasGroup _productsPanelGroup;
    [SerializeField] private CanvasGroup _enterprisesPanelGroup;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _startCamera;
    [SerializeField] private CinemachineVirtualCamera _mainCamera;

    [Header("Settings")]
    [SerializeField] private float _transitionDuration = 1f;
    [SerializeField] private float _fadeDuration = 0.5f;

    private CanvasGroup _currentPanelGroup;
    private RectTransform _menuPanelRect;
    private RectTransform _productsPanelRect;
    private RectTransform _enterprisesPanelRect;

    private void Start()
    {
        _menuPanelRect = _menuPanelGroup.GetComponent<RectTransform>();
        _productsPanelRect = _productsPanelGroup.GetComponent<RectTransform>();
        _enterprisesPanelRect = _enterprisesPanelGroup.GetComponent<RectTransform>();

        InitializePanels();
        _currentPanelGroup = _menuPanelGroup;
    }

    private void InitializePanels()
    {
        _menuPanelRect.anchoredPosition = Vector2.zero;
        _productsPanelRect.anchoredPosition = new Vector2(-Screen.width, 0);
        _enterprisesPanelRect.anchoredPosition = new Vector2(Screen.width, 0);

        _menuPanelGroup.alpha = 1f;
        _productsPanelGroup.alpha = 0f;
        _enterprisesPanelGroup.alpha = 0f;

        SetPanelInteractable(_menuPanelGroup, true);
        SetPanelInteractable(_productsPanelGroup, false);
        SetPanelInteractable(_enterprisesPanelGroup, false);
    }

    public void OpenProductsWindow()
    {
        SwitchCameraPriority(_mainCamera, _startCamera);
        TransitionPanels(_productsPanelGroup, _productsPanelRect);
    }

    public void OpenEnterprisesWindow()
    {
        TransitionPanels(_enterprisesPanelGroup, _enterprisesPanelRect);
    }

    public void ReturnToMenu()
    {
        SwitchCameraPriority(_startCamera, _mainCamera);
        TransitionPanels(_menuPanelGroup, _menuPanelRect);
    }

    private void TransitionPanels(CanvasGroup targetPanelGroup, RectTransform targetPanelRect)
    {
        float targetOffset = -targetPanelRect.anchoredPosition.x;

        _menuPanelRect.DOAnchorPosX(_menuPanelRect.anchoredPosition.x + targetOffset, _transitionDuration);
        _productsPanelRect.DOAnchorPosX(_productsPanelRect.anchoredPosition.x + targetOffset, _transitionDuration);
        _enterprisesPanelRect.DOAnchorPosX(_enterprisesPanelRect.anchoredPosition.x + targetOffset, _transitionDuration);

        _currentPanelGroup.DOFade(0f, _fadeDuration).OnComplete(() =>
        {
            SetPanelInteractable(_currentPanelGroup, false);
            _currentPanelGroup = targetPanelGroup;
        });

        targetPanelGroup.DOFade(1f, _fadeDuration).OnStart(() =>
        {
            SetPanelInteractable(targetPanelGroup, true);
        });
    }

    private void SwitchCameraPriority(CinemachineVirtualCamera activeCamera, CinemachineVirtualCamera inactiveCamera)
    {
        activeCamera.Priority = 10;
        inactiveCamera.Priority = 0;
    }

    private void SetPanelInteractable(CanvasGroup panelGroup, bool isInteractable)
    {
        panelGroup.interactable = isInteractable;
        panelGroup.blocksRaycasts = isInteractable;
    }
}