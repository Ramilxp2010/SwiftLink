using Microsoft.Extensions.DependencyInjection;

namespace Pet.SwiftLink.Desktop.ViewModels;


public class ViewModelLocator
{
    public MainViewModel MainViewModel => App.Services.GetRequiredService<MainViewModel>();
}