using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers.Inspectorate
{
    [Route("api/inspect")]
    [ApiController]
    public class InspectorateController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InspectorateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPut("inspect-stock")]
        public JsonResult InspectStock(InspectStock _inspect)
        {
            try
            {
                string query = @"UPDATE dbo.Saved SET DateInspected = @dateInspected, is_inspected = 'true', RCC = @_rcc, InspectedBy = @_inspectedBy WHERE PRNo = @_prNo AND StockNo = @_stockno";
                string UPDATE_TOTAL_STATUS = @"UPDATE dbo.PurchaseOrder SET is_complete = @is_complete, is_partial = @is_partial, Partial_Quantity = @partialQuantity
                                                WHERE PRNo = @_prNo";

                string INSERT_LOG = @"INSERT INTO dbo.wmis_logs (user_fname, user_lname, user_position, user_photo, user_activity, log_time) 
                            VALUES (@user_fname, @user_lname, @user_position, @user_photo, @user_activity, @log_time)";

                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_stockno", _inspect.StockNo);
                        cmd.Parameters.AddWithValue("@_prNo", _inspect.PRNo);   
                        cmd.Parameters.AddWithValue("@_rcc", _inspect.RCC);
                        cmd.Parameters.AddWithValue("@_inspectedBy", _inspect.InspectedBy);
                        cmd.Parameters.AddWithValue("@dateInspected", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd2 = new SqlCommand(UPDATE_TOTAL_STATUS, conn))
                    {
                        cmd2.Parameters.AddWithValue("@_prNo", _inspect.PRNo);
                        cmd2.Parameters.AddWithValue("@is_complete", _inspect.is_complete);
                        cmd2.Parameters.AddWithValue("@is_partial", _inspect.is_partial);
                        cmd2.Parameters.AddWithValue("@partialQuantity", _inspect.Partial_Quantity);

                        cmd2.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(INSERT_LOG, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_lname", _inspect.user_lname);
                        cmd.Parameters.AddWithValue("@user_fname", _inspect.user_fname);
                        cmd.Parameters.AddWithValue("@user_position", _inspect.user_position);
                        cmd.Parameters.AddWithValue("@user_photo", _inspect.user_photo);
                        cmd.Parameters.AddWithValue("@user_activity", _inspect.user_activity);
                        cmd.Parameters.AddWithValue("@log_time", DateTime.Now);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }
                }
                return new JsonResult("Stock is Inspected!");
            } catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
