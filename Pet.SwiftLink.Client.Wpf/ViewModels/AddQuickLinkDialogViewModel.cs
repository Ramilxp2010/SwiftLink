using Microsoft.Win32;
using Pet.SwiftLink.Domain.Model;
using Pet.SwiftLink.Desktop.Commands;
using System.Windows.Input;

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

        public AddQuickLinkDialogViewModel()
        {
            BrowseCommand = new RelayCommand(Browse);
        }

        public void Browse(object parameter)
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

            Result = new QuickLink
            {
                Id = Guid.NewGuid(),
                Name = Name,
                Path = Path,
                Type = SelectedType
            };

        }

    }
}