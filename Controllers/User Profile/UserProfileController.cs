using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers.User_Profile
{
    [Route("api/userProfile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserProfileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPut("main-profile-update")]
        public JsonResult MainProfileUpdate(UserProfileData _users)
        {
            try
            {
                string query = @"UPDATE dbo.wmis_user SET f_name = @fname, m_name = @mname, l_name = @lname,
                                photoFilename = @photofilename, position = @position WHERE userID = @userid";
                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userid", _users.userID);
                        cmd.Parameters.AddWithValue("@fname", _users.f_name);
                        cmd.Parameters.AddWithValue("@mname", _users.m_name);
                        cmd.Parameters.AddWithValue("@lname", _users.l_name);
                        cmd.Parameters.AddWithValue("@photofilename", _users.photoFilename);
                        cmd.Parameters.AddWithValue("position", _users.position);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }

                    return new JsonResult("Account Information has been successfully updated!");
                }

            } catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }


        [HttpPut("personal-information-update")]
        public JsonResult PersonalInformationUpdate(UserProfileData _users)
        {
            try
            {
                    string query = @"UPDATE dbo.wmis_user SET password = @password WHERE userID = @userid";
                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userid", _users.userID);
                        cmd.Parameters.AddWithValue("@password", _users.password);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }

                    return new JsonResult("Account Information has been successfully updated!");
                }

            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
