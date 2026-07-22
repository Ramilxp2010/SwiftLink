using System.Windows;
using System.Windows.Interop;
using Pet.SwiftLink.Desktop.Services.HotKeys;
using WinApp = System.Windows.Application;

namespace Pet.SwiftLink.Desktop.Services;

public sealed class MainWindowActivator : IMainWindowActivator
{
    public void Activate()
    {
        var window = WinApp.Current.MainWindow;
        if (window == null)
            return;

        if (!window.IsVisible)
            window.Show();

        if (window.WindowState == WindowState.Minimized)
            window.WindowState = WindowState.Normal;

        window.Activate();
        window.Focus();

        var handle = new WindowInteropHelper(window).Handle;
        if (handle != IntPtr.Zero)
            NativeMethods.SetForegroundWindow(handle);
    }
}
