namespace Pet.SwiftLink.Desktop.ViewModels;

public class CategorySummaryViewModel : ObservableObject
{
    private string _name = string.Empty;
    private int _count;
    private string _backgroundKey = "CategoryMiscBrush";
    private string _foregroundKey = "CategoryMiscForeground";

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public int Count
    {
        get => _count;
        set
        {
            if (SetProperty(ref _count, value))
                OnPropertyChanged(nameof(CountLabel));
        }
    }

    public string BackgroundKey
    {
        get => _backgroundKey;
        set => SetProperty(ref _backgroundKey, value);
    }

    public string ForegroundKey
    {
        get => _foregroundKey;
        set => SetProperty(ref _foregroundKey, value);
    }

    public string CountLabel => Count.ToString();
}
