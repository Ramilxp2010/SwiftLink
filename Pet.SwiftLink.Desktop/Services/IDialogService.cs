using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Desktop.Services;

public interface IDialogService
{
    QuickLink ShowAddQuickLinkDialog();
    void ShowError(string message);
}