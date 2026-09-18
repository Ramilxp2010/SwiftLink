using System.Runtime.InteropServices;

namespace Pet.SwiftLink.Desktop.Services.HotKeys;

internal static class NativeMethods
{
    internal const int WmHotkey = 0x0312;
    internal static readonly IntPtr HwndMessage = new(-3);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);
}
