using TMPro;
using UnityEngine;

public class ProductElementScroller : ScrollSelector
{
    [SerializeField] private TextMeshProUGUI _enterpriseNameText;
    protected override void Start()
    {
        _startSelectedIndex = PlayerPrefsManager.LoadProductIndex();
        
        base.Start();
    }

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);

        //_enterpriseNameText.text = ((EnterpriseScrollElement)_targetElement).EnterpriseName;

        PlayerPrefsManager.SaveProductIndex(index);
    }
}