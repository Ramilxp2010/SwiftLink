using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Desktop.ViewModels;
using System.Windows.Controls;

namespace Pet.SwiftLink.Desktop.Views;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        DataContext = App.Services!.GetRequiredService<SettingsViewModel>();
    }
}
