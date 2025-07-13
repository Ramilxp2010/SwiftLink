using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Desktop.Services;
using Pet.SwiftLink.Desktop.ViewModels;
using Pet.SwiftLink.Desktop.Views;
using Pet.SwiftLink.Infrastructure.Extensions;
using Pet.SwiftLink.Application.Extensions;
using System.Windows;
using Wpf.Ui;


namespace Pet.SwiftLink.Desktop
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<Window1>();
            mainWindow.Show();
        }
        
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            services.AddSingleton<Window1>();
            services.AddSingleton<MainViewModel>();

            services.AddRepository();
            services.AddRanging();
            
            services.AddSingleton<IStatisticTracker, WpfStatisticTracker>();
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                DisposeService<ILinkRankRepository>();
                DisposeService<ISwiftLinkRepository>();
            }
            finally
            {
                base.OnExit(e);
            }
        }

        private void DisposeService<T>() where T : class 
        {
            if (Services?.GetService<T>() is IDisposable disposable) 
            {
                disposable.Dispose();
            }
        }
    }

}
