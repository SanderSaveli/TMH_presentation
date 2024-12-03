using UnityEngine;
using TMPro;

public class EnterpriseElementScroller : ScrollSelector
{
    [SerializeField] private TextMeshProUGUI _enterpriseNameText;

    protected override void Start()
    {
        _startSelectedIndex = PlayerPrefsManager.LoadEnterpriseIndex();

        base.Start();
    }

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);

        _enterpriseNameText.text = ((EnterpriseScrollElement)_targetElement).EnterpriseName;

        PlayerPrefsManager.SaveEnterpriseIndex(index);
    }
}
