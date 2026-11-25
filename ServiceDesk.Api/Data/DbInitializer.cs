using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (context.Users.Any()) return; // БД уже заполнена

        var user1 = new User { LastName = "Иванов", FirstName = "Иван", Email = "ivan@test.ru", RoleId = 1 };
        var user2 = new User { LastName = "Петров", FirstName = "Петр", Email = "petr@test.ru", RoleId = 2 };
        var user3 = new User { LastName = "Сидоров", FirstName = "Сидор", Email = "admin@test.ru", RoleId = 3 };
        context.Users.AddRange(user1, user2, user3);

        var cat1 = new Category { Name = "Оборудование" };
        var cat2 = new Category { Name = "Программное обеспечение" };
        context.Categories.AddRange(cat1, cat2);

        context.SlaRules.AddRange(
            new SlaRule { PriorityId = 1, ResponseTimeMin = 480, ResolutionTimeMin = 2880 },
            new SlaRule { PriorityId = 2, ResponseTimeMin = 240, ResolutionTimeMin = 1440 },
            new SlaRule { PriorityId = 3, ResponseTimeMin = 60, ResolutionTimeMin = 480 },
            new SlaRule { PriorityId = 4, ResponseTimeMin = 15, ResolutionTimeMin = 120 }
        );

        await context.SaveChangesAsync();
    }
}