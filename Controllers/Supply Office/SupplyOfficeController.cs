using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers.Supply_Office
{
    [Route("api/accept")]
    [ApiController]
    public class SupplyOfficeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SupplyOfficeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // IF STOCK IS COMPLETE
        [HttpPut("accept-stock")]
        public JsonResult AcceptStock(AcceptStock _acceptStock)
        {
            try
            {
                string query = @"UPDATE dbo.Saved SET is_accepted = 'true' WHERE PRNo = @_prNo AND is_inspected = 'true'";

                string updateCompleteStock = @"UPDATE dbo.PurchaseOrder SET is_complete = 'true' WHERE PONo = @_poNo";

                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_prNo", _acceptStock.PRNo);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);
                    }

                    using (SqlCommand cmd = new SqlCommand(updateCompleteStock, conn))
                    {
                        cmd.Parameters.AddWithValue("@_poNo", _acceptStock.PONo);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }
                }

                return new JsonResult("Stock Successfully Accepted!");

            } catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }


        // IF STOCK IS PARTIAL
        [HttpPut("accept-partial-stock")]
        public JsonResult AcceptPartialStock(AcceptStock _acceptStock)
        {
            try
            {
                string query = @"UPDATE dbo.Saved SET is_accepted = 'true' WHERE PRNo = @_prNo AND is_inspected = 'true'";

                string updatePartialStock = @"UPDATE dbo.PurchaseOrder SET is_partial = 'true', Partial_Quantity = @partialQuantity WHERE PONo = @_poNo";

                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_prNo", _acceptStock.PRNo);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);
                    }

                    using (SqlCommand cmd = new SqlCommand(updatePartialStock, conn))
                    {
                        cmd.Parameters.AddWithValue("@_poNo", _acceptStock.PONo);
                        cmd.Parameters.AddWithValue("@partialQuantity", _acceptStock.Partial_Quantity);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }
                }

                return new JsonResult("Stock Successfully Accepted!");

            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
