using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Data {
    public class AppDbContext : DbContext {
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "todo.db");
            Console.WriteLine($"Using database at: {dbPath}");
            options.UseSqlite($"Data Source={dbPath}");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.IsCompleted)
                .HasDefaultValue(false);
        }
    }
}