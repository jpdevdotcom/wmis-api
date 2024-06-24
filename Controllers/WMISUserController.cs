using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers
{
    [Route("api/wmis-user")]
    [ApiController]
    public class WMISUserController : ControllerBase
    {
        private IConfiguration _configuration;

        public WMISUserController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpPost]
        public JsonResult PostUsers(WMIS_Users _wmis_users)
        {
            string query = @"INSERT INTO dbo.wmis_user (
	                        f_name,
	                        m_name,
	                        l_name,
	                        contactNumber,
	                        emailAddress,
	                        age,
	                        gender,
	                        position,
                            username,
                            password
	                        userType
                        ) VALUES (@f_name, @m_name, @l_name, @contactNumber, @emailAddress, @age, @gender, @position, @username, @password, @userType)";
            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@f_name", _wmis_users.Firstname);
                    cmd.Parameters.AddWithValue("@m_name", _wmis_users.Middlename);
                    cmd.Parameters.AddWithValue("@l_name", _wmis_users.Lastname);
                    cmd.Parameters.AddWithValue("@contactNumber", _wmis_users.ContactNumber);
                    cmd.Parameters.AddWithValue("@emailAddress", _wmis_users.EmailAddress);
                    cmd.Parameters.AddWithValue("@age", _wmis_users.Age);
                    cmd.Parameters.AddWithValue("@gender", _wmis_users.Gender);
                    cmd.Parameters.AddWithValue("@position", _wmis_users.Position);
                    cmd.Parameters.AddWithValue("@username", _wmis_users.username);
                    cmd.Parameters.AddWithValue("@password", _wmis_users.password);
                    cmd.Parameters.AddWithValue("@userType", _wmis_users.UserType);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("User Successfully Added!");
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            string query = @"SELECT * FROM dbo.wmis_user";
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

    }
}
