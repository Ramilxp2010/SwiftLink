using Microsoft.Win32;
using Pet.SwiftLink.Application.Interfaces;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Domain.Model;
using Pet.SwiftLink.Desktop.Commands;
using Pet.SwiftLink.Desktop.Services;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace Pet.SwiftLink.Desktop.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private readonly IAppSettingsService _settingsService;
    private readonly IDialogService _dialogService;
    private string _dataDirectory;

    public SettingsViewModel(IAppSettingsService settingsService, IDialogService dialogService)
    {
        _settingsService = settingsService;
        _dialogService = dialogService;

        _dataDirectory = _settingsService.GetCurrent().DataDirectory;

        BrowseCommand = new RelayCommand(Browse);
        SaveCommand = new RelayCommand(async _ => await SaveAsync());
        ResetToDefaultCommand = new RelayCommand(_ => DataDirectory = Path.GetTempPath());
    }

    public string DataDirectory
    {
        get => _dataDirectory;
        set
        {
            if (SetProperty(ref _dataDirectory, value))
            {
                OnPropertyChanged(nameof(QuickLinksPath));
                OnPropertyChanged(nameof(StatisticsPath));
            }
        }
    }

    public string QuickLinksPath =>
        IAppSettingsStore.GetQuickLinksPath(new AppSettings { DataDirectory = _dataDirectory });

    public string StatisticsPath =>
        IAppSettingsStore.GetStatisticsPath(new AppSettings { DataDirectory = _dataDirectory });

    public ICommand BrowseCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ResetToDefaultCommand { get; }

    private void Browse(object parameter)
    {
        var dialog = new OpenFolderDialog();
        if (dialog.ShowDialog() != true)
            return;

        DataDirectory = dialog.FolderName;
    }

    private async Task SaveAsync()
    {
        try
        {
            await _settingsService.SaveDataDirectoryAsync(DataDirectory);

            MessageBox.Show(
                "Настройки сохранены. Перезапустите приложение, чтобы изменения вступили в силу.",
                "Настройки",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Не удалось сохранить настройки: {ex.Message}");
        }
    }
}
