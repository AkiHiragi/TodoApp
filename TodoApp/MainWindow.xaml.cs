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
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private AppDbContext _db = new AppDbContext();

        public MainWindow() {
            InitializeComponent();
            using (var db = new AppDbContext()) {
                db.Database.EnsureCreated();
                LoadTasks();
            }
        }

        private void LoadTasks() {
            
            TasksListView.ItemsSource = _db.Tasks.ToList();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(NewTaskTextBox.Text)) {
                _db.Tasks.Add(new TaskItem { Title = NewTaskTextBox.Text });
                _db.SaveChanges();
                NewTaskTextBox.Clear();
                LoadTasks();
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e) {
            if(TasksListView.SelectedItem is TaskItem task) {
                _db.Tasks.Remove(task);
                _db.SaveChanges();
                LoadTasks();
            }
        }
    }
}