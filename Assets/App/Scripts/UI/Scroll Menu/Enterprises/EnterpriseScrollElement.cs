using UnityEngine;

public class EnterpriseScrollElement : ScrollElement
{
    [SerializeField] private string _enterpriseName;
    public string EnterpriseName => _enterpriseName;
}
