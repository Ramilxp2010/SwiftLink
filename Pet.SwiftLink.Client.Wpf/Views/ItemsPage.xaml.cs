using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Desktop.ViewModels;

namespace Pet.SwiftLink.Desktop.Views
{
    public partial class ItemPage : Page
    {
        public ItemPage()
        {
            InitializeComponent();
            DataContext = App.Services!.GetRequiredService<MainViewModel>();
        }

        public void FocusSearchBox()
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
        }

        private void QuickAccessCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2) return;
            if (DataContext is not MainViewModel vm) return;
            if (sender is FrameworkElement { Tag: QuickLinkViewModel link })
                vm.OpenQuickLinkCommand.Execute(link);
        }

        private void AddCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.AddQuickLinkCommand.Execute(null);
        }
    }
}
