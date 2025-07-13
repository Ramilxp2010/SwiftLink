using System.Windows;
using Pet.SwiftLink.Domain.Model;
using Pet.SwiftLink.Desktop.Views;

namespace Pet.SwiftLink.Desktop.Services;

public class DialogService : IDialogService
{
    public QuickLink ShowAddQuickLinkDialog()
    {
        var dialog = new AddQuickLinkDialog();
        //return dialog.ShowDialog() == false ? dialog.QuickLink : null;
        return null;
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}