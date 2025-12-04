using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TaskNote> TaskNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Models.Task>().ToTable("Tasks");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<TaskNote>().ToTable("TaskNotes");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<TaskNote>()
                .HasOne(n => n.Task)
                .WithMany(t => t.Notes)
                .HasForeignKey(n => n.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskNote>()
                .HasIndex(n => new { n.TaskId, n.CreatedAt });

            modelBuilder.Entity<Models.Task>()
                .HasIndex(t => t.DueDate);
        }
    }
}
