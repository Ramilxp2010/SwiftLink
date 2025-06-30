using System.Configuration;
using System.Data;
using System.Windows;
using Pet.SwiftLink.Desktop.ViewModels;
using Pet.SwiftLink.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Contract.Interfaces;
using Pet.SwiftLink.Desktop.Services;
using Pet.SwiftLink.Infrastructure.Extensions;
using Pet.SwiftLink.Ranging.Extensions;

namespace Pet.SwiftLink.Desktop
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
        
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<MainViewModel>();

            services.AddRepository();
            services.AddRanging();
            
            services.AddSingleton<IStatisticTracker, WpfStatisticTracker>();
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            var repo = Services?.GetService<ILinkRankRepository>();
            if (repo is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }

}
