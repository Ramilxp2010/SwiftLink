using System.Windows.Interop;
using System.Windows.Input;

namespace Pet.SwiftLink.Desktop.Services.HotKeys;

public sealed class HotKeyService : IHotKeyService
{
    private const int HotKeyId = 0x534C;

    private HwndSource? _messageWindow;
    private Action? _onPressed;
    private bool _isRegistered;

    public bool IsRegistered => _isRegistered;

    public bool TryRegister(HotKeyDefinition definition, Action onPressed)
    {
        if (_isRegistered)
            return true;

        _onPressed = onPressed;
        _messageWindow = CreateMessageWindow();
        _messageWindow.AddHook(WndProc);

        var modifiers = ToNativeModifiers(definition.Modifiers);
        var virtualKey = (uint)KeyInterop.VirtualKeyFromKey(definition.Key);

        if (!NativeMethods.RegisterHotKey(_messageWindow.Handle, HotKeyId, modifiers, virtualKey))
        {
            ReleaseResources();
            return false;
        }

        _isRegistered = true;
        return true;
    }

    public void Dispose()
    {
        if (_isRegistered && _messageWindow != null)
            NativeMethods.UnregisterHotKey(_messageWindow.Handle, HotKeyId);

        ReleaseResources();
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == NativeMethods.WmHotkey && wParam.ToInt32() == HotKeyId)
        {
            _onPressed?.Invoke();
            handled = true;
        }

        return IntPtr.Zero;
    }

    private void ReleaseResources()
    {
        if (_messageWindow != null)
        {
            _messageWindow.RemoveHook(WndProc);
            _messageWindow.Dispose();
            _messageWindow = null;
        }

        _onPressed = null;
        _isRegistered = false;
    }

    private static HwndSource CreateMessageWindow() =>
        new(new HwndSourceParameters("SwiftLinkHotKeyHost")
        {
            Width = 0,
            Height = 0,
            PositionX = 0,
            PositionY = 0,
            WindowStyle = 0,
            ParentWindow = NativeMethods.HwndMessage
        });

    private static uint ToNativeModifiers(HotKeyModifiers modifiers) => (uint)modifiers;
}
