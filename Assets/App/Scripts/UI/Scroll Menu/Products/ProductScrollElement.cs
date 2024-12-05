using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ProductScrollElement : ScrollElement
{
    [SerializeField] private Button _button;
    private GameObject _productModelPrefab; 
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) => _signalBus = signalBus;

    public override void Initialize(int index, Sprite icon, string name) => base.Initialize(index, icon, name);

    public void Initialize(int index, Sprite icon, string name, GameObject productModelPrefab)
    {
        Initialize(index, icon, name);

        if (productModelPrefab == null)
        {
            Debug.LogError("Product model prefab is null when initializing ProductScrollElement.");
        }
        else
        {
            Debug.Log($"Initializing with model prefab: {productModelPrefab.name}");
        }

        _productModelPrefab = productModelPrefab;

        _button.GetComponent<Button>().onClick.AddListener(() =>
        {
            ProductsManager.Instance.ShowModel(_productModelPrefab);
        });
    }

    public override void Select()
    {
        base.Select();

        if (_productModelPrefab != null)
        {
            _signalBus.Fire(new EventInputSelectModel(_productModelPrefab));
        }
        else
        {
            Debug.LogWarning("Product model prefab is null.");
        }
    }
}