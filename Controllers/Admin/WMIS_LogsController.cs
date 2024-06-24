using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers.Admin
{
    [Route("api/wmis-logs")]
    [ApiController]
    public class WMIS_LogsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WMIS_LogsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("get-wmis-login-logs")]
        public JsonResult GetLoginWMISLogs()
        {
            string query = @"SELECT * FROM dbo.wmis_logs WHERE user_activity = 'logged in' ORDER BY log_time DESC";
            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpGet("get-wmis-event-logs")]
        public JsonResult GetEventWMISLogs()
        {
            string query = @"SELECT * FROM dbo.wmis_logs WHERE NOT user_activity = 'logged in'";
            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost("insert-wmis-log")]
        public JsonResult PostWMISLogs(WMIS_Logs _logs)
        {
            string query = @"INSERT INTO dbo.wmis_logs (user_fname, user_lname, user_position, user_photo, user_activity, log_time) 
                            VALUES (@user_fname, @user_lname, @user_position, @user_photo, @user_activity, @log_time)";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_lname", _logs.user_lname);
                    cmd.Parameters.AddWithValue("@user_fname", _logs.user_fname);
                    cmd.Parameters.AddWithValue("@user_position", _logs.user_position);
                    cmd.Parameters.AddWithValue("@user_photo", _logs.user_photo);
                    cmd.Parameters.AddWithValue("@user_activity", _logs.user_activity);
                    cmd.Parameters.AddWithValue("@log_time", DateTime.Now);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                }
            }

            return new JsonResult("Successfully Updated!");
        }

        [HttpDelete("delete-wmis-logs-loggedIn")]
        public JsonResult DeleteWMISLogs()
        {
            string query = @"DELETE dbo.wmis_logs WHERE user_activity = 'logged in'";
            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Logs Successfully Deleted!");
        }

        [HttpDelete("delete-wmis-logs-event")]
        public JsonResult DeleteEventWMISLogs()
        {
            string query = @"DELETE dbo.wmis_logs WHERE NOT user_activity = 'logged in'";
            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Logs Successfully Deleted!");
        }
    }
}
