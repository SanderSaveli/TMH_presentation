using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public Camera Camera;
    public TMP_Text tooltipText;
    public RectTransform canvasRectTransform;
    public RectTransform backgroundRectTransform;
    public CanvasGroup canvasGroup;
    private Vector2 offset = new Vector2(100, 50);
    private static Tooltip instance;

    private void Awake()
    {
        instance = this;
        HideTooltip();
    }

    private void Update()
    {
        HandleRaycast();

        if (canvasGroup.alpha > 0)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 newPosition = mousePosition + offset;

            if (newPosition.x + backgroundRectTransform.rect.width > Screen.width)
            {
                newPosition.x = mousePosition.x - backgroundRectTransform.rect.width - offset.x;
            }
            if (newPosition.y + backgroundRectTransform.rect.height > Screen.height)
            {
                newPosition.y = mousePosition.y - backgroundRectTransform.rect.height - offset.y;
            }
            transform.position = newPosition;
        }
    }

    private void HandleRaycast()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            InteractableItem item = hit.collider.GetComponent<InteractableItem>();
            if (item != null)
            {
                Show(item.Data.Name);
                return;
            }
        }
        Hide();
    }

    public void ShowTooltip(string itemName)
    {
        tooltipText.text = itemName;
        backgroundRectTransform.sizeDelta = new Vector2(tooltipText.preferredWidth + 10, tooltipText.preferredHeight + 10);
        canvasGroup.alpha = 1;
    }

    public void HideTooltip()
    {
        canvasGroup.alpha = 0;
    }

    public static void Show(string itemName)
    {
        if (instance != null)
        {
            instance.ShowTooltip(itemName);
        }
    }

    public static void Hide()
    {
        if (instance != null)
        {
            instance.HideTooltip();
        }
    }
}