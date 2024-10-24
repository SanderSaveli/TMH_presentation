using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _menuCanvasGroup;
    [SerializeField] private CanvasGroup _trainsCanvasGroup;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Color _solidColor;
    [SerializeField] private CinemachineVirtualCamera _cinemachineStartCamera; 
    [SerializeField] private CinemachineVirtualCamera _cinemachineMainCamera; 
    [SerializeField] private float _transitionDuration = 1f;

    private bool _isInMainScene = false;

    void Start()
    {
        _mainCamera.clearFlags = CameraClearFlags.SolidColor;
        _mainCamera.backgroundColor = _solidColor;

        _cinemachineStartCamera.Priority = 10;
        _cinemachineMainCamera.Priority = 0;

        _menuCanvasGroup.alpha = 1;
        _menuCanvasGroup.interactable = true;
        _menuCanvasGroup.blocksRaycasts = true;
    }

    public void OnStartButtonPressed()
    {
        if (_isInMainScene) return;

        _isInMainScene = true;

        _menuCanvasGroup.DOFade(0, _transitionDuration / 2).OnComplete(() =>
        {
            _menuCanvasGroup.interactable = false;
            _menuCanvasGroup.blocksRaycasts = false;
        });

        _trainsCanvasGroup.DOFade(1, _transitionDuration).OnComplete(() =>
        {
            _trainsCanvasGroup.interactable = true;
            _trainsCanvasGroup.blocksRaycasts = true;
        });

        _mainCamera.clearFlags = CameraClearFlags.Skybox;

        DOTween.To(() => _mainCamera.backgroundColor, x => _mainCamera.backgroundColor = x, Color.black, _transitionDuration);


        _cinemachineStartCamera.Priority = 0;
        _cinemachineMainCamera.Priority = 10;
    }

    public void OnBackButtonPressed()
    {
        if (!_isInMainScene) return;

        _isInMainScene = false;

        _menuCanvasGroup.DOFade(1, _transitionDuration).OnComplete(() =>
        {
            _menuCanvasGroup.interactable = true;
            _menuCanvasGroup.blocksRaycasts = true;
        });

        _trainsCanvasGroup.DOFade(0, _transitionDuration / 2).OnComplete(() =>
        {
            _trainsCanvasGroup.interactable = false;
            _trainsCanvasGroup.blocksRaycasts = false;
        });

        DOTween.To(() => _mainCamera.backgroundColor, x => _mainCamera.backgroundColor = x, _solidColor, _transitionDuration)
            .OnComplete(() => _mainCamera.clearFlags = CameraClearFlags.SolidColor);

        _cinemachineStartCamera.Priority = 10;
        _cinemachineMainCamera.Priority = 0;
    }
}
