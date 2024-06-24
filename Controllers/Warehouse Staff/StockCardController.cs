using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers.Warehouse_Staff
{
    [Route("api/stock-card")]
    [ApiController]
    public class StockCardController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StockCardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Get Stock Card
        [HttpGet("get-stock-card")]
        public JsonResult GetStockCardData()
        {
            string query = @"SELECT DISTINCT *, (Stocks.Quantity - Stocks.TotalIssuedQuantity) AS TotalQuantity FROM dbo.Saved AS Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
                            WHERE Stocks.is_available = 'true'
							AND is_issued = 'false'
                            AND Stocks.Quantity != Stocks.TotalIssuedQuantity";

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

        // Add to Stock Card table
        [HttpPost("add-stock-card")]
        public JsonResult PostStockCard(StockCard _stockCard)
        {
            try
            {
                string ADD_STOCK_CARD_QUERY = @"INSERT INTO dbo.StockCard (FundCluster, PRNO, PONo, StockNo, Estimated_StockOutDate,
                                            Estimated_StockOutQuantity, DaysToConsume)
                                            VALUES (@fundCluster, @prNo, @poNo, @stockNo, @estimated_StockOutDate, @estimated_StockOutQuantity, @daysToConsume)";

                string UPDATE_SAVED_STOCK_CARD_ISSUE = @"UPDATE dbo.Saved SET issued_stockCard = 'true' WHERE StockNo = @stockNo AND PRNo = @prNo";

                DataTable table = new();
                string SqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(SqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(ADD_STOCK_CARD_QUERY, conn))
                    {
                        cmd.Parameters.AddWithValue("@fundCluster", _stockCard.FundCluster);
                        cmd.Parameters.AddWithValue("@prNo", _stockCard.PRNo);
                        cmd.Parameters.AddWithValue("@poNo", _stockCard.PONo);
                        cmd.Parameters.AddWithValue("@stockNo", _stockCard.StockNo);
                        cmd.Parameters.AddWithValue("@estimated_StockOutDate", _stockCard.Estimated_StockOutDate);
                        cmd.Parameters.AddWithValue("@estimated_StockOutQuantity", _stockCard.Estimated_StockOutQuantity);
                        cmd.Parameters.AddWithValue("@daysToConsume", _stockCard.DaysToConsume);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                    }

                    using (SqlCommand cmd = new SqlCommand(UPDATE_SAVED_STOCK_CARD_ISSUE, conn))
                    {
                        cmd.Parameters.AddWithValue("@prNo", _stockCard.PRNo);
                        cmd.Parameters.AddWithValue("@stockNo", _stockCard.StockNo);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }

                    return new JsonResult("Stock Card Successfully Issued!");
                }
            } catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
