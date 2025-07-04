using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Desktop.ViewModels;
using System.Windows.Controls;
using Wpf.Ui;

namespace Pet.SwiftLink.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ItemPage : Page
    {
        public ItemPage()
        {
            InitializeComponent();
            DataContext = App.Services.GetRequiredService<MainViewModel>();
        }

    }
}