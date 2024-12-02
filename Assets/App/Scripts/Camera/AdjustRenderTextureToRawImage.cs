using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class AdjustRenderTextureToRawImage : MonoBehaviour
{
    [SerializeField] private Camera _renderCamera;
    private RawImage _rawImage;
    private RenderTexture _renderTexture;

    private int _currentWidth = 0, _currentHeight = 0;

    private void Start()
    {
        _rawImage = GetComponent<RawImage>();
        UpdateRenderTextureSize();
    }

    private void UpdateRenderTextureSize()
    {
        RectTransform rectTransform = _rawImage.rectTransform;
        int width = Mathf.CeilToInt(rectTransform.rect.width);
        int height = Mathf.CeilToInt(rectTransform.rect.height);

        if (width == _currentWidth && height == _currentHeight)
            return;


        _currentWidth = width;
        _currentHeight = height;

        if (_renderTexture != null)
        {
            _renderTexture.Release();
            Destroy(_renderTexture);
        }

        _renderTexture = new RenderTexture(_currentWidth, _currentHeight, 24);
        _renderTexture.name = "DynamicRenderTexture";

        _renderCamera.targetTexture = _renderTexture;
        _rawImage.texture = _renderTexture;
    }

    private void Update() => UpdateRenderTextureSize();

    private void OnDestroy()
    {
        if (_renderTexture != null)
        {
            _renderTexture.Release();
            Destroy(_renderTexture);
        }
    }
}