using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Data;

namespace TodoApp.Models {
    public class TaskItem : INotifyPropertyChanged {
        public int Id { get; set; }
        public string Title { get; set; }

        private bool _isCompleted;
        public bool IsCompleted {
            get => _isCompleted;
            set { 
                _isCompleted = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
