using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common; // Для DbCommand
using Fly_Me_to_the_Moon.Data; // Переконайтеся, що це правильний простір імен для вашого DbContext

namespace FlyMeToTheMoon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SpaceFlightContext _context;

        public CheckController(IConfiguration configuration, SpaceFlightContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("db-table-count")]
        public async Task<IActionResult> GetDbTableCount()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            var status = new Dictionary<string, object>
            {
                { "API Connection String", connectionString },
                { "Status", "UNKNOWN" },
                { "Table Count", 0 }
            };

            DbConnection connection = _context.Database.GetDbConnection();

            try
            {
                // 1. Відкриваємо з'єднання
                await connection.OpenAsync();

                // 2. Створюємо команду для підрахунку таблиць у загальнодоступній схемі
                using var command = connection.CreateCommand();

                // SQL-запит для PostgreSQL, який повертає кількість таблиць
                // (WHERE table_schema = 'public' виключає системні таблиці)
                command.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';";

                // 3. Виконуємо запит, щоб отримати скалярне значення (число)
                var result = await command.ExecuteScalarAsync();

                // 4. Обробляємо результат
                int tableCount = Convert.ToInt32(result);

                status["Status"] = "SUCCESS: Database connection established.";
                status["Table Count"] = tableCount;

                // Додаємо діагностичний вивід
                if (tableCount > 0)
                {
                    status["Message"] = $"Found {tableCount} user-defined tables (excluding system tables).";
                }
                else
                {
                    status["Message"] = "Connected successfully, but no user-defined tables were found (0 tables).";
                }

            }
            catch (Exception ex)
            {
                // 5. Обробка помилок підключення
                status["Status"] = "FAILURE: Fatal Error during connection or command execution.";
                status["Table Count"] = -1;
                status["Error Message"] = ex.Message;
                return StatusCode(500, status);
            }
            finally
            {
                // Завжди закриваємо з'єднання
                await connection.CloseAsync();
            }

            return Ok(status);
        }
    }
}