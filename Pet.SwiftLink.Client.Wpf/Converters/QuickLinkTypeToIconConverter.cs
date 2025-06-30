using Pet.SwiftLink.Contract.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Pet.SwiftLink.Desktop.Converters
{
    public class QuickLinkTypeToIconConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is QuickLinkType type)
            {
                string iconPath = type switch
                {
                    QuickLinkType.Folder => "Images/folder.png",
                    QuickLinkType.File => "Images/file.png",
                    QuickLinkType.Application => "Images/app.png",
                    _ => "Images/default.png"
                };
                return new BitmapImage(new Uri(iconPath, UriKind.Relative));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
