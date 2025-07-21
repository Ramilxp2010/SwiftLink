using Pet.SwiftLink.Domain.Interfaces;
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
    private readonly TrayIconViewModel _trayIconViewModel;
    
    private readonly IDialogService _dialogService;
    private readonly IStatisticTracker _statisticTracker;
    private readonly ISwiftLinkService _linkService;

    private readonly IContentDialogService _contentDialogService;

    private QuickLinkViewModel _selectedLink;

    public ObservableCollection<QuickLinkViewModel> QuickLinks { get; } = new();
    
    public QuickLinkViewModel SelectedLink
    {
        get => _selectedLink;
        set => SetProperty(ref _selectedLink, value);
    }

    private string? _dialogResultText = string.Empty;
    public string? DialogResultText
    {
        get => _dialogResultText;
        set => SetProperty(ref _dialogResultText, value);
    }

    public ICommand AddQuickLinkCommand { get; }
    public ICommand OpenQuickLinkCommand { get; }
    public ICommand RemoveQuickLinkCommand { get; }
    public ICommand MinimizeToTrayCommand { get; }

    public MainViewModel(IDialogService dialogService, IStatisticTracker statisticTracker, IContentDialogService contentDialogService, ISwiftLinkService linkService)
    {
        _trayIconViewModel = new TrayIconViewModel(ShowWindow, CloseApplication);

        _dialogService = dialogService;
        _statisticTracker = statisticTracker;
        _contentDialogService = contentDialogService;
        _linkService = linkService;

        OpenQuickLinkCommand = new RelayCommand(OpenQuickLink, CanOpenQuickLink);
        RemoveQuickLinkCommand = new RelayCommand(RemoveQuickLink, CanRemoveQuickLink);
        MinimizeToTrayCommand = new RelayCommand(_ => MinimizeToTray());
        AddQuickLinkCommand = new RelayCommand(async _ => await OnShowDialog());

        LoadQuickLinks();
    }

    private async Task OnShowDialog()
    {
        var addQuickLinkDialog = new AddQuickLinkDialog();

        ContentDialogResult result = await _contentDialogService.ShowSimpleDialogAsync(
            new SimpleContentDialogCreateOptions()
            {
                Title = "Добавить папку в быстрый доступ",
                Content = addQuickLinkDialog,
                PrimaryButtonText = "Добавить",
                CloseButtonText = "Отмена",
            }
        );

        if (result == ContentDialogResult.Primary)
        {
            var addQuickLinkVM = (AddQuickLinkDialogViewModel)addQuickLinkDialog.DataContext;
            if (addQuickLinkVM.Result != null)
            {
                QuickLinks.Add(new QuickLinkViewModel(addQuickLinkVM.Result));
                SaveQuickLink(addQuickLinkVM.Result);
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

        QuickLinks.Clear();
        var ordered = _statisticTracker.OrderByPopularity(links).Result;
        foreach (var link in ordered) QuickLinks.Add(new QuickLinkViewModel(link));
    }

    private void SaveQuickLinks()
    {
        var links = QuickLinks.Select(x => x.Model);
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
            _statisticTracker.TrackClickAsync(link.Model.Id);
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = link.Path,
                UseShellExecute = true
            });
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
        QuickLinks.Remove(link);
    }

    private bool CanRemoveQuickLink(object parameter) => parameter != null;

    private void ShowWindow()
    {
        WinApp.Current.MainWindow.Show();
        WinApp.Current.MainWindow.WindowState = WindowState.Normal;
    }

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