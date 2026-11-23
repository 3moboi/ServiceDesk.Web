using Microsoft.EntityFrameworkCore;
using ServiceDesk.Api.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ServiceDesk.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Priority> Priorities { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<SlaRule> SlaRules { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TicketStatusHistory> TicketStatusHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка каскадных удалений (чтобы избежать ошибок при удалении)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Requester)
            .WithMany()
            .HasForeignKey(t => t.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Assignee)
            .WithMany()
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Заполнение справочников начальными данными (Seed Data)
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Заявитель" },
            new Role { Id = 2, Name = "Исполнитель" },
            new Role { Id = 3, Name = "Администратор" }
        );

        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Name = "Новая" },
            new Status { Id = 2, Name = "В работе" },
            new Status { Id = 3, Name = "Решена" },
            new Status { Id = 4, Name = "Закрыта" }
        );

        modelBuilder.Entity<Priority>().HasData(
            new Priority { Id = 1, Name = "Низкий", Weight = 1 },
            new Priority { Id = 2, Name = "Средний", Weight = 2 },
            new Priority { Id = 3, Name = "Высокий", Weight = 3 },
            new Priority { Id = 4, Name = "Критический", Weight = 4 }
        );

        modelBuilder.Entity<TicketStatusHistory>()
           .HasOne(h => h.ChangedByUser)
           .WithMany()
           .HasForeignKey(h => h.ChangedByUserId)
           .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление

        modelBuilder.Entity<TicketStatusHistory>()
            .HasOne(h => h.NewStatus)
            .WithMany()
            .HasForeignKey(h => h.NewStatusId)
            .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление

        modelBuilder.Entity<TicketStatusHistory>()
            .HasOne(h => h.OldStatus)
            .WithMany()
            .HasForeignKey(h => h.OldStatusId)
            .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление
    }
}