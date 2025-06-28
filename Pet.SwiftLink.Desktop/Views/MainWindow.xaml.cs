using Newtonsoft.Json;
using Pet.SwiftLink.Contract.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pet.SwiftLink.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<QuickLink> _quickLinks = new ObservableCollection<QuickLink>();
        private const string DataFilePath = "quicklinks.json";

        public MainWindow()
        {
            InitializeComponent();
            LinksList.ItemsSource = _quickLinks;
            LoadQuickLinks();
        }

        private void LoadQuickLinks()
        {
            if (File.Exists(DataFilePath))
            {
                string json = File.ReadAllText(DataFilePath);
                var links = JsonConvert.DeserializeObject<List<QuickLink>>(json);
                if (links == null)
                    return;

                _quickLinks.Clear();
                foreach (var link in links) _quickLinks.Add(link);
            }
        }

        private void SaveQuickLinks()
        {
            string json = JsonConvert.SerializeObject(_quickLinks.ToList());
            File.WriteAllText(DataFilePath, json);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddQuickLinkDialog();
            if (dialog.ShowDialog() == true && dialog?.QuickLink != null)
            {
                _quickLinks.Add(dialog.QuickLink);
                SaveQuickLinks();
            }
        }

        private void LinksList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LinksList.SelectedItem is QuickLink link && link != null && link.Path != null)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = link.Path,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}