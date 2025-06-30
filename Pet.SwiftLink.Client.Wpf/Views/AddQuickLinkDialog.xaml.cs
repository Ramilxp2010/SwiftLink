using System.Windows;
using Pet.SwiftLink.Contract.Model;
using Pet.SwiftLink.Desktop.ViewModels;

namespace Pet.SwiftLink.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AddQuickLinkDialog.xaml
    /// </summary>
    public partial class AddQuickLinkDialog : Window
    {
        public AddQuickLinkDialog()
        {
            InitializeComponent();
        }
        
        public QuickLink? QuickLink => (DataContext as AddQuickLinkDialogViewModel)?.Result;
    }
}
