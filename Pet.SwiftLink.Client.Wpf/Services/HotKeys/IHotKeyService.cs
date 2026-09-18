namespace Pet.SwiftLink.Desktop.Services.HotKeys;

public interface IHotKeyService : IDisposable
{
    bool IsRegistered { get; }

    bool TryRegister(HotKeyDefinition definition, Action onPressed);
}
