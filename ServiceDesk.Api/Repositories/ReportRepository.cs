using Microsoft.Data.SqlClient;
using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Repositories;

public class ReportRepository
{
    private readonly string _connectionString;

    public ReportRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<int> GetTicketsCountByCategoryAsync(int categoryId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Пример "сырого" запроса через ADO.NET
        using var command = new SqlCommand(
            "SELECT COUNT(*) FROM Tickets WHERE CategoryId = @categoryId", connection);
        command.Parameters.AddWithValue("@categoryId", categoryId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
}