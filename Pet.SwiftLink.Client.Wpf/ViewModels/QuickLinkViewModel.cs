using System.Windows.Media;
using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Desktop.ViewModels;

public class QuickLinkViewModel : ObservableObject
{
    public QuickLink Model => _model;
    private readonly QuickLink _model;

    public string Name
    {
        get => _model.Name;
        set
        {
            _model.Name = value;
            OnPropertyChanged();
        }
    }

    public string Path => _model.Path;
    public QuickLinkType Type => _model.Type;

    public ImageSource Icon
    {
        get
        {
            // Здесь должна быть логика получения иконки
            return null;
        }
    }

    public QuickLinkViewModel(QuickLink model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }
}