using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Pet.SwiftLink.Contract.Model;
using Pet.SwiftLink.Desktop.Commands;
using Pet.SwiftLink.Desktop.Services;

namespace Pet.SwiftLink.Desktop.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private QuickLinkViewModel _selectedLink;
    private const string DataFilePath = "quicklinks.json";

    public ObservableCollection<QuickLinkViewModel> QuickLinks { get; } = new();
    
    public QuickLinkViewModel SelectedLink
    {
        get => _selectedLink;
        set => SetProperty(ref _selectedLink, value);
    }

    public ICommand AddQuickLinkCommand { get; }
    public ICommand OpenQuickLinkCommand { get; }
    public ICommand RemoveQuickLinkCommand { get; }
    public ICommand MinimizeToTrayCommand { get; }

    public MainViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;

        // Инициализация команд
        AddQuickLinkCommand = new RelayCommand(AddQuickLink);
        OpenQuickLinkCommand = new RelayCommand(OpenQuickLink, CanOpenQuickLink);
        RemoveQuickLinkCommand = new RelayCommand(RemoveQuickLink, CanRemoveQuickLink);
        MinimizeToTrayCommand = new RelayCommand(MinimizeToTray);

        LoadQuickLinks();
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
            foreach (var link in links) QuickLinks.Add(new QuickLinkViewModel(link));
        }
    }

    private void SaveQuickLinks()
    {
        string json = JsonConvert.SerializeObject(QuickLinks.Select(x=>x.Model).ToList());
        File.WriteAllText(DataFilePath, json);
    }

    private void AddQuickLink(object parameter)
    {
        var result = _dialogService.ShowAddQuickLinkDialog();
        if (result != null)
        {
            QuickLinks.Add(new QuickLinkViewModel(result));
        }
    }

    private void OpenQuickLink(object parameter)
    {
        if (parameter is QuickLinkViewModel link)
        {
            try
            {
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
    }

    private bool CanOpenQuickLink(object parameter) => parameter != null;

    private void RemoveQuickLink(object parameter)
    {
        if (parameter is QuickLinkViewModel link)
        {
            QuickLinks.Remove(link);
        }
    }

    private bool CanRemoveQuickLink(object parameter) => parameter != null;

    private void MinimizeToTray(object parameter)
    {
        Application.Current.MainWindow.Hide();
    }
}