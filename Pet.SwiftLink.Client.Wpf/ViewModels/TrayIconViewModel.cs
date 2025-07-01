using System.Windows.Forms;
using System.Windows.Input;
using Pet.SwiftLink.Desktop.Commands;
using Pet.SwiftLink.Desktop.Models;

namespace Pet.SwiftLink.Desktop.ViewModels;

public class TrayIconViewModel : IDisposable
{
    private readonly TrayIconModel _model;
    private readonly Action _showMainWindow;
    private readonly Action _closeApplication;

    public ICommand ShowWindowCommand { get; }
    public ICommand ExitApplicationCommand { get; }

    public TrayIconViewModel(Action showMainWindow, Action closeApplication)
    {
        _model = new TrayIconModel();
        _showMainWindow = showMainWindow;
        _closeApplication = closeApplication;

        ShowWindowCommand = new RelayCommand(_ => _showMainWindow());
        ExitApplicationCommand = new RelayCommand(_ => _closeApplication());

        InitializeTrayIcon();
    }

    private void InitializeTrayIcon()
    {
        _model.Initialize();
            
        _model.NotifyIcon.DoubleClick += (s, e) => _showMainWindow();
            
        var menu = new ContextMenuStrip();
        menu.Items.Add("Открыть").Click += (s, e) => _showMainWindow();
        menu.Items.Add("Выход").Click += (s, e) => _closeApplication();
        _model.NotifyIcon.ContextMenuStrip = menu;
    }

    public void Dispose()
    {
        _model.Dispose();
    }
}