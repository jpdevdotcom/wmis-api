using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace UEPWarehouse_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PurchaseOrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public JsonResult GetPurchaseOrder(int id)
        {
            string query = @"SELECT StockNo, Unit, Description, Quantity, UnitCost, Amount, Stock.PRNo, Stock.PRDate, PONo 
                            FROM dbo.Saved AS Stock 
                            INNER JOIN dbo.PurchaseOrder as PO ON Stock.PRNo = @PRNo";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PRNo", id);

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
