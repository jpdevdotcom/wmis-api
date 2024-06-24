using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace UEPWarehouse_API.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class ReturnStockController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ReturnStockController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("return-stock")]
        public JsonResult GetReturnStock()
        {
            string query = @"SELECT * FROM dbo.Saved Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
							INNER JOIN dbo.RIS AS RIS ON Stocks.PRNo = RIS.PRNo
							INNER JOIN dbo.PAR AS PAR ON Stocks.PRNo = PAR.PRNo
							WHERE Stocks.is_issued = 'true' AND RIS.is_received = 'true' AND Stocks.is_returned = 'false'";

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
