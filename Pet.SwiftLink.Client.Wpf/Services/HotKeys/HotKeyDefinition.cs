using System.Windows.Input;

namespace Pet.SwiftLink.Desktop.Services.HotKeys;

public readonly record struct HotKeyDefinition(HotKeyModifiers Modifiers, Key Key)
{
    public static HotKeyDefinition Default { get; } =
        new(HotKeyModifiers.Control | HotKeyModifiers.Shift, Key.S);
}
