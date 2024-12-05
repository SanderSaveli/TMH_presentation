using UnityEngine;
using TMPro;

public class EnterpriseElementScroller : ScrollSelector
{
    [SerializeField] private TextMeshProUGUI _enterpriseNameText;

    protected override void Awake()
    {
        _startSelectedIndex = 0;//PlayerPrefsManager.LoadEnterpriseIndex();

        base.Awake();
    }

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);

        _enterpriseNameText.text = ((EnterpriseScrollElement)_targetElement).EnterpriseName;

        //PlayerPrefsManager.SaveEnterpriseIndex(index);
    }
}
