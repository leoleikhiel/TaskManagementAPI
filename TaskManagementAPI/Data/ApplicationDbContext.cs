using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TaskNote> TaskNotes { get; set; }
        public DbSet<GoogleCalendarToken> GoogleCalendarTokens { get; set; }
        public DbSet<TaskCalendarSync> TaskCalendarSyncs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<TaskItem>().ToTable("Tasks");
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

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.DueDate);

            modelBuilder.Entity<GoogleCalendarToken>()
                .HasIndex(t => t.UserId);

            modelBuilder.Entity<GoogleCalendarToken>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskCalendarSync>()
                .HasIndex(s => s.TaskId);

            modelBuilder.Entity<TaskCalendarSync>()
                .HasIndex(s => s.GoogleEventId);

            modelBuilder.Entity<TaskCalendarSync>()
                .HasOne(s => s.Task)
                .WithOne(t => t.CalendarSync)
                .HasForeignKey<TaskCalendarSync>(s => s.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskCalendarSync>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
