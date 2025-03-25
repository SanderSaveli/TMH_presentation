using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public delegate void OnInteractAction(InteractableItem item);
    public static event OnInteractAction OnInteract;
    public ModelData Data;
    public List<Outline> _outlines;

    private void Start()
    {
        foreach (var item in _outlines)
        {
            item.enabled = false;
        }
    }

    void OnMouseDown()
    {
        OnInteract?.Invoke(this);
    }

    private void OnMouseEnter()
    {
        foreach (var item in _outlines)
        {
            item.enabled = true;
        }
    }

    private void OnMouseExit()
    {
        foreach (var item in _outlines)
        {
            item.enabled = false;
        }
    }
}
