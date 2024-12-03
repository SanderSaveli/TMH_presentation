public class ProductElementScroller : ScrollSelector
{
    protected override void Start()
    {
        _startSelectedIndex = PlayerPrefsManager.LoadProductIndex();
        
        base.Start();
    }

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);

        PlayerPrefsManager.SaveProductIndex(index);
    }
}