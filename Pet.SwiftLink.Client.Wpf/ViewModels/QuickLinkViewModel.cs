using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Desktop.ViewModels;

public class QuickLinkViewModel : ObservableObject
{
    public QuickLink Model => _model;
    private readonly QuickLink _model;

    private DateTime? _lastOpened;

    public string Name
    {
        get => _model.Name ?? string.Empty;
        set
        {
            _model.Name = value;
            OnPropertyChanged();
        }
    }

    public string Path => _model.Path ?? string.Empty;
    public QuickLinkType Type => _model.Type;

    public string Category
    {
        get => QuickLinkCategories.Normalize(_model.Category);
        set
        {
            _model.Category = QuickLinkCategories.Normalize(value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(TypeAndCategory));
            OnPropertyChanged(nameof(MetaLine));
        }
    }

    public bool IsPinned
    {
        get => _model.IsPinned;
        set
        {
            if (_model.IsPinned == value) return;
            _model.IsPinned = value;
            OnPropertyChanged();
        }
    }

    public DateTime? LastOpened
    {
        get => _lastOpened;
        set
        {
            if (SetProperty(ref _lastOpened, value))
            {
                OnPropertyChanged(nameof(RelativeTime));
                OnPropertyChanged(nameof(MetaLine));
            }
        }
    }

    public string TypeDisplay => Type switch
    {
        QuickLinkType.Folder => "Папка",
        QuickLinkType.File => "Файл",
        QuickLinkType.Application => "Приложение",
        _ => Type.ToString()
    };

    public string TypeAndCategory => $"{TypeDisplay} • {Category}";

    public string MetaLine
    {
        get
        {
            var relative = RelativeTime;
            return string.IsNullOrEmpty(relative)
                ? TypeAndCategory
                : $"{TypeAndCategory} · {relative}";
        }
    }

    public string RelativeTime
    {
        get
        {
            if (LastOpened is null || LastOpened == DateTime.MinValue)
                return string.Empty;

            var local = LastOpened.Value.Kind == DateTimeKind.Utc
                ? LastOpened.Value.ToLocalTime()
                : LastOpened.Value;

            var delta = DateTime.Now - local;
            if (delta.TotalMinutes < 1) return "только что";
            if (delta.TotalMinutes < 60) return $"{(int)delta.TotalMinutes} мин назад";
            if (delta.TotalHours < 24) return $"{(int)delta.TotalHours} ч назад";
            if (delta.TotalDays < 7) return $"{(int)delta.TotalDays} дн назад";
            return local.ToString("dd.MM.yyyy");
        }
    }

    public QuickLinkViewModel(QuickLink model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _model.Category = QuickLinkCategories.Normalize(_model.Category);
    }
}
