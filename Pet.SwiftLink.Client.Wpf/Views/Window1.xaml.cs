using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Pet.SwiftLink.Desktop.ViewModels;
using Wpf.Ui;

namespace Pet.SwiftLink.Desktop.Views
{
    public partial class Window1 : Window
    {
        private readonly MainViewModel _viewModel;

        public Window1(IContentDialogService contentDialogService, MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            contentDialogService.SetDialogHost(RootContentDialog);

            _viewModel.RequestNavigateHome += NavigateHome;
            _viewModel.RequestNavigateSettings += NavigateSettings;
            _viewModel.RequestFocusSearch += FocusSearchBox;

            Loaded += (_, _) => NavigateHome();
        }

        private void NavigateHome()
        {
            if (ContentFrame.Content is not ItemPage)
                ContentFrame.Navigate(new ItemPage());
        }

        private void NavigateSettings()
        {
            ContentFrame.Navigate(new SettingsPage());
        }

        private void FocusSearchBox()
        {
            if (ContentFrame.Content is ItemPage page)
                page.FocusSearchBox();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.K && Keyboard.Modifiers == ModifierKeys.Control)
            {
                FocusSearchBox();
                e.Handled = true;
            }
        }

        private void FavoriteItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2) return;
            if (sender is FrameworkElement { Tag: QuickLinkViewModel link })
                _viewModel.OpenQuickLinkCommand.Execute(link);
        }

        private void CategoryChip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement { Tag: string category })
                _viewModel.SetCategoryFilterCommand.Execute(category);
        }
    }
}
