using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public delegate void OnInteractAction(ObjectInfo item, Transform target);
    public static event OnInteractAction OnInteract;
    public ObjectInfo Data;

    public void OnMouseDown() => OnInteract?.Invoke(Data, transform);
}
