using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public delegate void OnInteractAction(InteractableItem item);
    public static event OnInteractAction OnInteract;
    public ModelData Data;

    void OnMouseDown()
    {
        OnInteract?.Invoke(this);
    }
}
