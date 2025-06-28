using Microsoft.Win32;
using Pet.SwiftLink.Contract.Model;
using Pet.SwiftLink.Desktop.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Pet.SwiftLink.Desktop
{
    /// <summary>
    /// Interaction logic for AddQuickLinkDialog.xaml
    /// </summary>
    public partial class AddQuickLinkDialog : Window
    {
        public QuickLink? QuickLink { get; private set; }

        public AddQuickLinkDialog()
        {
            InitializeComponent();
        }
        //private void BrowseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
        //    if (type == "Folder")
        //    {
        //        string selectedPath = FolderPickerHelper.PickFolder(this);
        //        if (!string.IsNullOrEmpty(selectedPath))
        //            PathTextBox.Text = selectedPath;
        //    }
        //    else
        //    {
        //        var dialog = new Microsoft.Win32.OpenFileDialog();
        //        if (dialog.ShowDialog() == true)
        //            PathTextBox.Text = dialog.FileName;
        //    }
        //}
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Выберите папку"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                PathTextBox.Text = dialog.FileName;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TypeComboBox.SelectedItem is ComboBoxItem cmb && cmb.Tag != null)
            {
                QuickLink = new QuickLink
                {
                    Name = NameTextBox.Text,
                    Path = PathTextBox.Text,
                    Type = (QuickLinkType)Enum.Parse(typeof(QuickLinkType), cmb.Tag.ToString() ?? "")
                };

                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
