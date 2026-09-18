using Pet.SwiftLink.Domain.Model;
using Pet.SwiftLink.Desktop.Commands;
using Pet.SwiftLink.Desktop.Services;
using Pet.SwiftLink.Desktop.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using Pet.SwiftLink.Application.Interfaces;
using WinApp = System.Windows.Application;

namespace Pet.SwiftLink.Desktop.ViewModels;

public class MainViewModel : ObservableObject
{
    private const int QuickAccessLimit = 8;

    private readonly TrayIconViewModel _trayIconViewModel;
    private readonly IDialogService _dialogService;
    private readonly IStatisticTracker _statisticTracker;
    private readonly ISwiftLinkService _linkService;
    private readonly IMainWindowActivator _mainWindowActivator;
    private readonly IContentDialogService _contentDialogService;

    private QuickLinkViewModel? _selectedLink;
    private string _searchText = string.Empty;
    private NavigationFilter _activeFilter = NavigationFilter.Home;
    private string? _selectedCategory;
    private bool _isSettingsOpen;
    private string _contentTitle = "Главная";
    private string? _dialogResultText = string.Empty;

    public ObservableCollection<QuickLinkViewModel> AllLinks { get; } = new();
    public ObservableCollection<QuickLinkViewModel> QuickAccessLinks { get; } = new();
    public ObservableCollection<QuickLinkViewModel> RecentLinks { get; } = new();
    public ObservableCollection<QuickLinkViewModel> FavoriteLinks { get; } = new();
    public ObservableCollection<QuickLinkViewModel> FilteredLinks { get; } = new();
    public ObservableCollection<CategorySummaryViewModel> CategorySummaries { get; } = new();

    /// <summary>Legacy alias used by older bindings.</summary>
    public ObservableCollection<QuickLinkViewModel> QuickLinks => AllLinks;

    public QuickLinkViewModel? SelectedLink
    {
        get => _selectedLink;
        set => SetProperty(ref _selectedLink, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                RefreshDerivedCollections();
        }
    }

    public NavigationFilter ActiveFilter
    {
        get => _activeFilter;
        private set
        {
            if (SetProperty(ref _activeFilter, value))
            {
                OnPropertyChanged(nameof(IsHomeView));
                OnPropertyChanged(nameof(IsFilteredView));
                OnPropertyChanged(nameof(IsHomeNavActive));
                OnPropertyChanged(nameof(IsFavoritesNavActive));
                OnPropertyChanged(nameof(IsRecentNavActive));
                OnPropertyChanged(nameof(IsAllNavActive));
                OnPropertyChanged(nameof(IsTrashNavActive));
                RefreshDerivedCollections();
            }
        }
    }

    public string? SelectedCategory
    {
        get => _selectedCategory;
        private set
        {
            if (SetProperty(ref _selectedCategory, value))
                OnPropertyChanged(nameof(IsCategoryNavActive));
        }
    }

    public bool IsSettingsOpen
    {
        get => _isSettingsOpen;
        private set
        {
            if (SetProperty(ref _isSettingsOpen, value))
            {
                OnPropertyChanged(nameof(IsHomeChromeVisible));
                OnPropertyChanged(nameof(IsSettingsNavActive));
                OnPropertyChanged(nameof(IsHomeView));
                OnPropertyChanged(nameof(IsFilteredView));
                OnPropertyChanged(nameof(IsHomeNavActive));
                OnPropertyChanged(nameof(IsFavoritesNavActive));
                OnPropertyChanged(nameof(IsRecentNavActive));
                OnPropertyChanged(nameof(IsAllNavActive));
                OnPropertyChanged(nameof(IsTrashNavActive));
                OnPropertyChanged(nameof(IsCategoryNavActive));
            }
        }
    }

    public bool IsHomeChromeVisible => !IsSettingsOpen;
    public bool IsHomeView => !IsSettingsOpen && ActiveFilter == NavigationFilter.Home;
    public bool IsFilteredView => !IsSettingsOpen && ActiveFilter != NavigationFilter.Home;

    public bool IsHomeNavActive => !IsSettingsOpen && ActiveFilter == NavigationFilter.Home;
    public bool IsFavoritesNavActive => !IsSettingsOpen && ActiveFilter == NavigationFilter.Favorites;
    public bool IsRecentNavActive => !IsSettingsOpen && ActiveFilter == NavigationFilter.Recent;
    public bool IsAllNavActive => !IsSettingsOpen && ActiveFilter == NavigationFilter.All;
    public bool IsTrashNavActive => !IsSettingsOpen && ActiveFilter == NavigationFilter.Trash;
    public bool IsSettingsNavActive => IsSettingsOpen;
    public bool IsCategoryNavActive => !IsSettingsOpen && ActiveFilter == NavigationFilter.Category;

    public string ContentTitle
    {
        get => _contentTitle;
        private set => SetProperty(ref _contentTitle, value);
    }

    public string? DialogResultText
    {
        get => _dialogResultText;
        set => SetProperty(ref _dialogResultText, value);
    }

    public IReadOnlyList<string> Categories => QuickLinkCategories.All;

    public ICommand AddQuickLinkCommand { get; }
    public ICommand OpenQuickLinkCommand { get; }
    public ICommand RemoveQuickLinkCommand { get; }
    public ICommand MinimizeToTrayCommand { get; }
    public ICommand TogglePinCommand { get; }
    public ICommand SetFilterCommand { get; }
    public ICommand SetCategoryFilterCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand FocusSearchCommand { get; }

    public event Action? RequestFocusSearch;
    public event Action? RequestNavigateHome;
    public event Action? RequestNavigateSettings;

    public MainViewModel(
        IDialogService dialogService,
        IStatisticTracker statisticTracker,
        IContentDialogService contentDialogService,
        ISwiftLinkService linkService,
        IMainWindowActivator mainWindowActivator)
    {
        _trayIconViewModel = new TrayIconViewModel(ShowWindow, CloseApplication);

        _dialogService = dialogService;
        _statisticTracker = statisticTracker;
        _contentDialogService = contentDialogService;
        _linkService = linkService;
        _mainWindowActivator = mainWindowActivator;

        OpenQuickLinkCommand = new RelayCommand(OpenQuickLink, CanOpenQuickLink);
        RemoveQuickLinkCommand = new RelayCommand(RemoveQuickLink, CanRemoveQuickLink);
        MinimizeToTrayCommand = new RelayCommand(_ => MinimizeToTray());
        AddQuickLinkCommand = new RelayCommand(async _ => await OnShowDialog());
        TogglePinCommand = new RelayCommand(TogglePin, CanTogglePin);
        SetFilterCommand = new RelayCommand(SetFilter);
        SetCategoryFilterCommand = new RelayCommand(SetCategoryFilter);
        OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
        FocusSearchCommand = new RelayCommand(_ => RequestFocusSearch?.Invoke());

        LoadQuickLinks();
        SetFilter(NavigationFilter.Home);
    }

    private async Task OnShowDialog()
    {
        var addQuickLinkDialog = new AddQuickLinkDialog();

        ContentDialogResult result = await _contentDialogService.ShowSimpleDialogAsync(
            new SimpleContentDialogCreateOptions()
            {
                Title = "Добавить в быстрый доступ",
                Content = addQuickLinkDialog,
                PrimaryButtonText = "Добавить",
                CloseButtonText = "Отмена",
            }
        );

        if (result == ContentDialogResult.Primary)
        {
            var addQuickLinkVM = (AddQuickLinkDialogViewModel)addQuickLinkDialog.DataContext;
            var link = addQuickLinkVM.BuildResult();
            if (link != null)
            {
                AllLinks.Add(new QuickLinkViewModel(link));
                SaveQuickLink(link);
                RefreshDerivedCollections();
            }
        }

        DialogResultText = result switch
        {
            ContentDialogResult.Primary => "Добавлено",
            _ => "Ничего не добавлено!",
        };
    }

    private void LoadQuickLinks()
    {
        var links = _linkService.GetQuickLinks();
        if (links == null)
            return;

        AllLinks.Clear();
        var ordered = _statisticTracker.OrderByPopularity(links).Result;
        var ranks = _statisticTracker.GetRanksAsync().Result;

        foreach (var link in ordered)
        {
            var vm = new QuickLinkViewModel(link);
            if (ranks.TryGetValue(link.Id, out var rank) && rank.LastClicked > DateTime.MinValue)
                vm.LastOpened = rank.LastClicked;
            AllLinks.Add(vm);
        }

        RefreshDerivedCollections();
    }

    private void RefreshDerivedCollections()
    {
        var query = AllLinks.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            query = query.Where(x =>
                (x.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.Path?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.Category?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var matched = query.ToList();

        FavoriteLinks.Clear();
        foreach (var link in matched.Where(x => x.IsPinned).OrderBy(x => x.Name))
            FavoriteLinks.Add(link);

        RecentLinks.Clear();
        foreach (var link in matched
                     .OrderByDescending(x => x.LastOpened ?? DateTime.MinValue)
                     .ThenBy(x => x.Name)
                     .Take(30))
            RecentLinks.Add(link);

        QuickAccessLinks.Clear();
        foreach (var link in matched.Take(QuickAccessLimit))
            QuickAccessLinks.Add(link);

        FilteredLinks.Clear();
        IEnumerable<QuickLinkViewModel> filtered = ActiveFilter switch
        {
            NavigationFilter.Favorites => matched.Where(x => x.IsPinned),
            NavigationFilter.Recent => matched
                .OrderByDescending(x => x.LastOpened ?? DateTime.MinValue)
                .ThenBy(x => x.Name),
            NavigationFilter.All => matched.OrderBy(x => x.Name),
            NavigationFilter.Category when !string.IsNullOrWhiteSpace(SelectedCategory) =>
                matched.Where(x => x.Category == SelectedCategory).OrderBy(x => x.Name),
            NavigationFilter.Trash => Enumerable.Empty<QuickLinkViewModel>(),
            _ => matched
        };

        foreach (var link in filtered)
            FilteredLinks.Add(link);

        RebuildCategorySummaries();
        UpdateContentTitle();
    }

    private void RebuildCategorySummaries()
    {
        CategorySummaries.Clear();
        foreach (var category in QuickLinkCategories.All)
        {
            var (bg, fg) = GetCategoryBrushKeys(category);
            CategorySummaries.Add(new CategorySummaryViewModel
            {
                Name = category,
                Count = AllLinks.Count(x => x.Category == category),
                BackgroundKey = bg,
                ForegroundKey = fg
            });
        }
    }

    private static (string Background, string Foreground) GetCategoryBrushKeys(string category) =>
        category switch
        {
            QuickLinkCategories.Work => ("CategoryWorkBrush", "CategoryWorkForeground"),
            QuickLinkCategories.Documents => ("CategoryDocumentsBrush", "CategoryDocumentsForeground"),
            QuickLinkCategories.Projects => ("CategoryProjectsBrush", "CategoryProjectsForeground"),
            _ => ("CategoryMiscBrush", "CategoryMiscForeground")
        };

    private void UpdateContentTitle()
    {
        ContentTitle = ActiveFilter switch
        {
            NavigationFilter.Home => "Главная",
            NavigationFilter.Favorites => "Избранное",
            NavigationFilter.Recent => "Недавние",
            NavigationFilter.All => "Все элементы",
            NavigationFilter.Category => SelectedCategory ?? "Категория",
            NavigationFilter.Trash => "Корзина",
            _ => "Главная"
        };
    }

    private void SetFilter(object? parameter)
    {
        NavigationFilter filter;
        if (parameter is NavigationFilter navFilter)
            filter = navFilter;
        else if (parameter is string s && Enum.TryParse(s, true, out NavigationFilter parsed))
            filter = parsed;
        else
            return;

        IsSettingsOpen = false;
        SelectedCategory = null;
        ActiveFilter = filter;
        RequestNavigateHome?.Invoke();
        OnPropertyChanged(nameof(IsCategoryNavActive));
    }

    private void SetCategoryFilter(object? parameter)
    {
        if (parameter is not string category)
            return;

        IsSettingsOpen = false;
        SelectedCategory = QuickLinkCategories.Normalize(category);
        ActiveFilter = NavigationFilter.Category;
        RequestNavigateHome?.Invoke();
        OnPropertyChanged(nameof(IsCategoryNavActive));
        RefreshDerivedCollections();
    }

    private void OpenSettings()
    {
        IsSettingsOpen = true;
        RequestNavigateSettings?.Invoke();
    }

    public bool IsCategorySelected(string category) =>
        ActiveFilter == NavigationFilter.Category &&
        string.Equals(SelectedCategory, category, StringComparison.Ordinal);

    private void SaveQuickLinks()
    {
        var links = AllLinks.Select(x => x.Model);
        _linkService.RecordAsync(links);
    }

    private void SaveQuickLink(QuickLink link)
    {
        _linkService.RecordAsync(link);
    }

    private void OpenQuickLink(object parameter)
    {
        if (parameter is not QuickLinkViewModel link) return;
        try
        {
            _ = _statisticTracker.TrackClickAsync(link.Model.Id);
            link.LastOpened = DateTime.UtcNow;
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = link.Path,
                UseShellExecute = true
            });
            RefreshDerivedCollections();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Не удалось открыть: {ex.Message}");
        }
    }

    private bool CanOpenQuickLink(object parameter) => parameter != null;

    private void RemoveQuickLink(object parameter)
    {
        if (parameter is not QuickLinkViewModel link) return;
        AllLinks.Remove(link);
        _linkService.DeleteAsync(link.Model.Id);
        RefreshDerivedCollections();
    }

    private bool CanRemoveQuickLink(object parameter) => parameter != null;

    private void TogglePin(object parameter)
    {
        if (parameter is not QuickLinkViewModel link) return;
        link.IsPinned = !link.IsPinned;
        SaveQuickLink(link.Model);
        RefreshDerivedCollections();
    }

    private bool CanTogglePin(object parameter) => parameter is QuickLinkViewModel;

    private void ShowWindow() => _mainWindowActivator.Activate();

    private void MinimizeToTray()
    {
        WinApp.Current.MainWindow.Hide();
    }

    private void CloseApplication()
    {
        SaveQuickLinks();
        _trayIconViewModel.Dispose();
        WinApp.Current.Shutdown();
    }
}
