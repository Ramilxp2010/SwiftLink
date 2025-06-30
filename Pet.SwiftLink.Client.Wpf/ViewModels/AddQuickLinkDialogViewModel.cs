using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Pet.SwiftLink.Contract.Model;
using Pet.SwiftLink.Desktop.Commands;

namespace Pet.SwiftLink.Desktop.ViewModels
{
    public class AddQuickLinkDialogViewModel : ObservableObject
    {
        private string? _name;
        private string? _path;
        private QuickLinkType _selectedType;

        public QuickLink? Result { get; private set; }
        
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string? Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        public QuickLinkType SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        public ICommand BrowseCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public AddQuickLinkDialogViewModel()
        {
            BrowseCommand = new RelayCommand(Browse);
            AddCommand = new RelayCommand(Add, CanAdd);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Browse(object parameter)
        {
            if (SelectedType == QuickLinkType.Folder)
            {
                var dialog = new OpenFolderDialog();
                if (dialog.ShowDialog() != true) return;
                Path = dialog.FolderName;
                Name = string.IsNullOrWhiteSpace(Name) ? System.IO.Path.GetFileName(Path) : Name;
            }
            else
            {
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() != true) return;
                Path = dialog.FileName;
                Name = string.IsNullOrWhiteSpace(Name) ? System.IO.Path.GetFileNameWithoutExtension(Path) : Name;
            }
        }

        private bool CanAdd(object parameter) => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Path);

        private void Add(object parameter)
        {
            Result = new QuickLink
            {
                Name = Name,
                Path = Path,
                Type = SelectedType
            };

            CloseDialog(true);
        }

        private void Cancel(object parameter) => CloseDialog(false);

        private void CloseDialog(bool dialogResult)
        {
            var window = Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }
    }
}