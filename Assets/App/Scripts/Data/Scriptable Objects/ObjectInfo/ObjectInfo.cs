using UnityEngine;

public class ObjectInfo : ScriptableObject
{
    public enum ObjectCategory
    {
        train,
        metro,
        locomotive,
        interactableObject,
        enterprise
    }
    
    [SerializeField] private ObjectCategory _category;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [TextArea] [SerializeField] private string _description;
    [SerializeField] private GameObject _modelPrefab;

    public ObjectCategory Category => _category;
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
    public GameObject ModelPrefab => _modelPrefab;
}
