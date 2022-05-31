using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace EZPostgresApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        NpgsqlConnection myConn;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            string connString = _configuration.GetConnectionString("DefaultConnection");
            myConn = new NpgsqlConnection(connString);
        }

        [HttpGet]
        public string Get()
        {
            string query = "SELECT * FROM users";

            DataTable table = new DataTable();
            NpgsqlDataReader reader;
            myConn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(query, myConn);
            reader = cmd.ExecuteReader();
            table.Load(reader);

            reader.Close();
            myConn.Close();

            return JsonConvert.SerializeObject(table);
        }
    }
}
