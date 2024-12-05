using UnityEngine;
using UnityEngine.EventSystems;

public class RawImageRaycaster : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Camera renderCamera;
    [SerializeField] private RectTransform rawImageRectTransform;

    private RenderTexture renderTexture;

    private void Start()
    {
        renderTexture = renderCamera.targetTexture;
        if (renderTexture == null)
            Debug.LogError("RenderTexture is not assigned to the camera!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ProcessClick(eventData.position);
    }

    private void ProcessClick(Vector2 screenPosition)
    {
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImageRectTransform,
            screenPosition,
            null,
            out localPoint))
            return;

        Rect rect = rawImageRectTransform.rect;
        float normalizedX = (localPoint.x - rect.xMin) / rect.width;
        float normalizedY = (localPoint.y - rect.yMin) / rect.height;

        if (normalizedX < 0 || normalizedX > 1 || normalizedY < 0 || normalizedY > 1)
            return;

        Vector3 viewportPoint = new Vector3(normalizedX, normalizedY, 0);

        Ray ray = renderCamera.ViewportPointToRay(viewportPoint);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Hit object: {hit.collider.name}");
            
            InteractableItem interactable = hit.collider.GetComponent<InteractableItem>();
            if (interactable != null)
            {
                Debug.Log($"Interacting with: {interactable.name}");
                interactable.OnMouseDown();
            }
        }
        else
        {
            Debug.Log("No object hit by raycast.");
        }
    }
}