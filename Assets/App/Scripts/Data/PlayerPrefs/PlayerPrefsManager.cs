using UnityEngine;

public static class PlayerPrefsManager
{
    private const string PRODUCTSSAVEDINDEXNAME = "Saved Product Index";
    private const string ENTERPRISESAVEDINDEXNAME = "Saved Enterprise Index";

    public static void SaveProductIndex(int index) => PlayerPrefs.SetInt(PRODUCTSSAVEDINDEXNAME, index);
    public static int LoadProductIndex() => PlayerPrefs.GetInt(PRODUCTSSAVEDINDEXNAME, 0);
    public static void SaveEnterpriseIndex(int index) => PlayerPrefs.SetInt(ENTERPRISESAVEDINDEXNAME, index);
    public static int LoadEnterpriseIndex() => PlayerPrefs.GetInt(ENTERPRISESAVEDINDEXNAME, 0);
}
