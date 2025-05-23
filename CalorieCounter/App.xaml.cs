using CalorieCounter.Models;
using CalorieCounter.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace CalorieCounter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public App()
        {
            var services = new ServiceCollection();
            services.AddDbContext<CalorieCounterContext>(options => options.UseSqlServer(("Data Source=HannahLeah;Initial Catalog=PersonsDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")));

            services.AddTransient<MainWindow>();
            services.AddTransient<MainViewModel>();

            ServiceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = ServiceProvider.GetService<MainWindow>();
            window.Show();
        }
    }

}
