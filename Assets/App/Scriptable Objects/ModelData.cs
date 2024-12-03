using UnityEngine;

public class ModelData : ScriptableObject
{
    [SerializeField] private string _name;
    [TextArea] [SerializeField] private string _description;

    public string Name => _name;
    public string Description => _description;
}
