using System.Windows.Forms;

namespace Pet.SwiftLink.Desktop.Models;

public class TrayIconModel
{
    public NotifyIcon NotifyIcon { get; private set; }

    public void Initialize()
    {
        NotifyIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("Resources/speed.ico"),
            Text = "SwiftLink",
            Visible = true
        };
    }

    public void Dispose()
    {
        NotifyIcon?.Dispose();
    }
}