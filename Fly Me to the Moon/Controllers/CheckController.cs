using Fly_Me_to_the_Moon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

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

                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';";
                var result = await command.ExecuteScalarAsync();


                int tableCount = Convert.ToInt32(result);
                status["Status"] = "SUCCESS: Database connection established.";
                status["Table Count"] = tableCount;


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
                status["Status"] = "FAILURE: Fatal Error during connection or command execution.";
                status["Table Count"] = -1;
                status["Error Message"] = ex.Message;
                return StatusCode(500, status);
            }
            finally
            {
                await connection.CloseAsync();
            }

            return Ok(status);
        }
    }
}