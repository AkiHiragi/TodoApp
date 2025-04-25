using System.Configuration;
using System.Data;
using System.Windows;

namespace TodoApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            Console.WriteLine("Application started");
            Console.WriteLine($"Base directory: {AppDomain.CurrentDomain.BaseDirectory}");
        }
    }

}
