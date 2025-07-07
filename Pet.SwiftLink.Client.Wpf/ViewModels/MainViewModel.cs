using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Pet.SwiftLink.Contract.Model;
using Pet.SwiftLink.Desktop.Commands;
using Pet.SwiftLink.Desktop.Services;
using Pet.SwiftLink.Desktop.Views;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace Pet.SwiftLink.Desktop.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly TrayIconViewModel _trayIconViewModel;
    
    private readonly IDialogService _dialogService;
    private readonly IStatisticTracker _statisticTracker;

    private readonly IContentDialogService _contentDialogService;

    private QuickLinkViewModel _selectedLink;
    private const string DataFilePath = "quicklinks.json";

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

    public MainViewModel(IDialogService dialogService, IStatisticTracker statisticTracker, IContentDialogService contentDialogService)
    {
        _trayIconViewModel = new TrayIconViewModel(ShowWindow, CloseApplication);

        _dialogService = dialogService;
        _statisticTracker = statisticTracker;
        _contentDialogService = contentDialogService;

        // Инициализация команд
        OpenQuickLinkCommand = new RelayCommand(OpenQuickLink, CanOpenQuickLink);
        RemoveQuickLinkCommand = new RelayCommand(RemoveQuickLink, CanRemoveQuickLink);
        MinimizeToTrayCommand = new RelayCommand(_ => MinimizeToTray());
        AddQuickLinkCommand = new RelayCommand(async _ => await OnShowDialog());

        LoadQuickLinks();
    }

    private async Task OnShowDialog()
    {
        var addQuickLinkDialog = new AddQuickLinkDialog();

        ((AddQuickLinkDialogViewModel)addQuickLinkDialog.DataContext).Confirmed += (s, link) =>
        {
            if (link != null)
            {
                QuickLinks.Add(new QuickLinkViewModel(link));
            }
        };

        ContentDialogResult result = await _contentDialogService.ShowSimpleDialogAsync(
            new SimpleContentDialogCreateOptions()
            {
                Title = "Добавить папку в быстрый доступ",
                Content = addQuickLinkDialog,
                PrimaryButtonText = "Добавить",
                CloseButtonText = "Отмена",
            }
        );


        DialogResultText = result switch
        {
            ContentDialogResult.Primary => "Добавлено",
            _ => "Ничего не добавлено!",
        };
    }

    private void LoadQuickLinks()
    {
        if (File.Exists(DataFilePath))
        {
            string json = File.ReadAllText(DataFilePath);
            var links = JsonConvert.DeserializeObject<List<QuickLink>>(json);
            if (links == null)
                return;

            QuickLinks.Clear();
            var ordered = _statisticTracker.OrderByPopularity(links).Result;
            foreach (var link in ordered) QuickLinks.Add(new QuickLinkViewModel(link));
        }
    }

    private void SaveQuickLinks()
    {
        string json = JsonConvert.SerializeObject(QuickLinks.Select(x=>x.Model).ToList());
        File.WriteAllText(DataFilePath, json);
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
        Application.Current.MainWindow.Show();
        Application.Current.MainWindow.WindowState = WindowState.Normal;
    }

    private void MinimizeToTray()
    {
        Application.Current.MainWindow.Hide();
    }

    private void CloseApplication()
    {
        _trayIconViewModel.Dispose();
        Application.Current.Shutdown();
    }
}