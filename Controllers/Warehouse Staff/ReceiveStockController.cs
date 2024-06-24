using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers.Warehouse_Staff
{
    [Route("api/stock-receive")]
    [ApiController]
    public class ReceiveStockController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ReceiveStockController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPut("receive-stock")]
        public JsonResult ReceiveStock(ReceiveStocks _receiveStocks)
        {
            string query = @"UPDATE dbo.Saved SET DRNo = @drno, RCC = @rcc,
                            is_available = 'true' WHERE PRNo = @prno";
            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@rcc", _receiveStocks.RCC);
                    cmd.Parameters.AddWithValue("@prno", _receiveStocks.PRNo);
                    cmd.Parameters.AddWithValue("@drno", _receiveStocks.DRNo);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Stock Successfully Received!");
        }
    }
}
