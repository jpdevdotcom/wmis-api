using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers.Admin
{
    [Route("api/admin")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public UserRegistrationController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet("get-users")]
        public JsonResult GetUsers()
        {
            try
            {
                string query = @"Select * FROM dbo.wmis_user";
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

            } catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpPost("register-user")]
        public JsonResult RegisterUser(UserRegistration _userRegistration)
        {
            try
            {
                string query = @"INSERT INTO dbo.wmis_user(f_name, m_name, l_name, photoFilename, contactNumber,
                                emailAddress, age, gender, position, username, password, userType, createdAt)
                                VALUES (@fname, @mname, @lname, @photofilename, @contactnumber, @emailaddress,
                                @age, @gender, @position, @username, @password, @usertype, @createdAt)";
                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fname", _userRegistration.f_name);
                        cmd.Parameters.AddWithValue("@mname", _userRegistration.m_name);
                        cmd.Parameters.AddWithValue("@lname", _userRegistration.l_name);
                        cmd.Parameters.AddWithValue("@photofilename", _userRegistration.photoFilename);
                        cmd.Parameters.AddWithValue("@contactnumber", _userRegistration.contactNumber);
                        cmd.Parameters.AddWithValue("@emailaddress", _userRegistration.emailAddress);
                        cmd.Parameters.AddWithValue("@age", _userRegistration.age);
                        cmd.Parameters.AddWithValue("@gender", _userRegistration.gender);
                        cmd.Parameters.AddWithValue("@position", _userRegistration.position);
                        cmd.Parameters.AddWithValue("@username", _userRegistration.username);
                        cmd.Parameters.AddWithValue("@password", _userRegistration.password);
                        cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }
                }

                return new JsonResult("Your account has been successfully registered!");

            } catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        /*[HttpPost("save-photo")]
        public JsonResult SavePhoto()
        {
            try
            { 
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(fileName);
            } catch (Exception ex)
            {
                return new JsonResult("Anonymous.png");
            }
        }*/
    }
}
