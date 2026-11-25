using Microsoft.Data.SqlClient;
using ServiceDesk.Api.DTOs;

namespace ServiceDesk.Api.Repositories;

public interface IReportRepository
{
    Task<ReportDto> GenerateReportAsync(DateTime from, DateTime to);
}

public class ReportRepository : IReportRepository
{
    private readonly string _connectionString;

    public ReportRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Строка подключения не найдена");
    }

    public async Task<ReportDto> GenerateReportAsync(DateTime from, DateTime to)
    {
        var report = new ReportDto();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // 1. Отчёт по категориям
        using (var command = new SqlCommand(@"
            SELECT c.Name AS CategoryName, COUNT(t.Id) AS Count
            FROM Tickets t
            JOIN Categories c ON t.CategoryId = c.Id
            WHERE t.CreatedAt >= @From AND t.CreatedAt <= @To
            GROUP BY c.Name
            ORDER BY Count DESC", connection))
        {
            command.Parameters.AddWithValue("@From", from);
            command.Parameters.AddWithValue("@To", to);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                report.ByCategory.Add(new CategoryReportItem
                {
                    CategoryName = reader.GetString(0),
                    Count = reader.GetInt32(1)
                });
            }
        }

        // 2. Отчёт по исполнителям
        using (var command = new SqlCommand(@"
            SELECT u.LastName + ' ' + u.FirstName AS AssigneeName, COUNT(t.Id) AS Count
            FROM Tickets t
            JOIN Users u ON t.AssigneeId = u.Id
            WHERE t.CreatedAt >= @From AND t.CreatedAt <= @To
            GROUP BY u.LastName, u.FirstName
            ORDER BY Count DESC", connection))
        {
            command.Parameters.AddWithValue("@From", from);
            command.Parameters.AddWithValue("@To", to);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                report.ByAssignee.Add(new AssigneeReportItem
                {
                    AssigneeName = reader.GetString(0),
                    Count = reader.GetInt32(1)
                });
            }
        }

        // 3. Общая статистика (SLA)
        using (var command = new SqlCommand(@"
            SELECT 
                COUNT(*) AS TotalClosed,
                SUM(CASE WHEN ClosedAt <= DueDate THEN 1 ELSE 0 END) AS OnTime
            FROM Tickets
            WHERE StatusId = 4 -- Закрыта
              AND ClosedAt >= @From AND ClosedAt <= @To", connection))
        {
            command.Parameters.AddWithValue("@From", from);
            command.Parameters.AddWithValue("@To", to);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                report.TotalClosed = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                var onTime = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);

                if (report.TotalClosed > 0)
                {
                    report.OnTimePercentage = Math.Round((double)onTime / report.TotalClosed * 100, 1);
                }
            }
        }

        return report;
    }
}