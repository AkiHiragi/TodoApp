using Microsoft.EntityFrameworkCore;
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

        private readonly AppDbContext _db;
        private bool _isInitialized = false;

        public MainWindow() {
            try {
                InitializeComponent();

                // Явная инициализация с проверкой
                _db = new AppDbContext();
                Console.WriteLine($"DbContext created: {_db != null}");
                Console.WriteLine($"Database path: {_db.Database.GetDbConnection().DataSource}");

                _db.Database.EnsureCreated();
                Console.WriteLine("Database ensured");

                LoadTasks();
                _isInitialized = true;
            }
            catch (Exception ex) {
                MessageBox.Show($"Ошибка инициализации: {ex.ToString()}");
                Close();
            }
        }

        protected override void OnClosed(EventArgs e) {
            _db?.Dispose();
            base.OnClosed(e);
        }

        private void LoadTasks() {
            try {
                Console.WriteLine($"LoadTasks: _db={_db != null}, Tasks={_db?.Tasks != null}");

                if (_db == null) throw new InvalidOperationException("DbContext is null");
                if (_db.Tasks == null) throw new InvalidOperationException("Tasks DbSet is null");

                var tasks = _db.Tasks.ToList();
                Console.WriteLine($"Loaded {tasks.Count} tasks");

                foreach (var task in tasks) {
                    task.PropertyChanged += (sender, e) => {
                        if (e.PropertyName == nameof(TaskItem.IsCompleted))
                            ScheduleSave();
                    };
                }

                TasksListView.ItemsSource = tasks;
                if (_isInitialized)
                    UpdateFilter();
            }
            catch (Exception ex) {
                MessageBox.Show($"Ошибка загрузки: {ex.ToString()}");
            }
        }

        private Timer? _saveTimer;

        private void ScheduleSave() {
            _saveTimer?.Dispose();
            _saveTimer = new Timer(_ => {
                Dispatcher.Invoke(() => {
                    try {
                        if (_db != null) {
                            _db.SaveChanges();
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Ошибка сохранения: {ex.Message}");
                    }
                });
            }, null, 500, Timeout.Infinite);
        }

        private void UpdateFilter() {
            if (!_isInitialized) return; // Защита от вызова до инициализации

            try {
                if (AllTasksFilter?.IsChecked == true)
                    TasksListView.ItemsSource = _db.Tasks.ToList();
                else if (ActiveTasksFilter?.IsChecked == true)
                    TasksListView.ItemsSource = _db.Tasks.Where(t => !t.IsCompleted).ToList();
                else
                    TasksListView.ItemsSource = _db.Tasks.Where(t => t.IsCompleted).ToList();
            }
            catch (Exception ex) {
                Console.WriteLine($"Ошибка фильтрации: {ex.Message}");
            }
        }

        private void AddTask_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(NewTaskTextBox.Text)) {
                _db.Tasks.Add(new TaskItem { Title = NewTaskTextBox.Text });
                _db.SaveChanges();
                NewTaskTextBox.Clear();
                LoadTasks();
                UpdateFilter();
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e) {
            if (TasksListView.SelectedItem is TaskItem task) {
                var result = MessageBox.Show(
                    $"Удалить задачу \"{task.Title}\"?",
                    "Подтверждение",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes) {
                    _db.Tasks.Remove(task);
                    _db.SaveChanges();
                    LoadTasks();
                    UpdateFilter();
                }
            }
        }

        private void AllTasksFilter_Checked(object sender, RoutedEventArgs e) {
            UpdateFilter();
        }

        private void ActiveTasksFilter_Checked(object sender, RoutedEventArgs e) {
            UpdateFilter();
        }

        private void CompletedTasksFilter_Checked(object sender, RoutedEventArgs e) {
            UpdateFilter();
        }
    }
}