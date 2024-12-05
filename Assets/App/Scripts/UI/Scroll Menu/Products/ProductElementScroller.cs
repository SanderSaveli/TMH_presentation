public class ProductElementScroller : ScrollSelector
{
    protected override void Awake()
    {
        _startSelectedIndex = 0;//PlayerPrefsManager.LoadProductIndex();
        
        base.Awake();
    }

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);

        //PlayerPrefsManager.SaveProductIndex(index);
    }
}