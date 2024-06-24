using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class CondemnedStocksController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public CondemnedStocksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("condemned-stocks")]
        public JsonResult GetCondemn()
        {
            string query = @"SELECT * FROM dbo.Saved Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
							INNER JOIN dbo.RIS AS RIS ON Stocks.PRNo = RIS.PRNo
							INNER JOIN dbo.PAR AS PAR ON Stocks.PRno = PAR.PRno
							INNER JOIN dbo.Condemned_Stocks AS CONDEMNED ON Stocks.StockNo = CONDEMNED.StockNo
							WHERE Stocks.is_issued = 'true' AND Stocks.is_returned = 'true' ORDER BY Stocks.StockNo ASC";
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

        [HttpPost("add-condemn-stock")]
        public JsonResult Post(CondemnedStocks _condemnedStocks)
        {
            string query = @"INSERT INTO dbo.Condemned_Stocks (
	                        StockNo,
                            PRNo,
	                        ConsumedQty,
	                        AvailableQty,
	                        Status
                        ) VALUES (@stockNo, @prNumber, @consumedQty, @availableQty, @status)";

            string updateSaved = @"UPDATE dbo.Saved SET is_returned = 'true' WHERE PRNo = @prNo AND StockNo = @stockNo";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@stockNo", _condemnedStocks.StockNo);
                    cmd.Parameters.AddWithValue("@prNumber", _condemnedStocks.PRNo);
                    cmd.Parameters.AddWithValue("@consumedQty", _condemnedStocks.ConsumedQty);
                    cmd.Parameters.AddWithValue("@availableQty", _condemnedStocks.AvailableQty);
                    cmd.Parameters.AddWithValue("@status", _condemnedStocks.Status);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                }

                using (SqlCommand cmd = new SqlCommand(updateSaved, conn))
                {
                    cmd.Parameters.AddWithValue("@stockNo", _condemnedStocks.StockNo);
                    cmd.Parameters.AddWithValue("@prNo", _condemnedStocks.PRNo);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();   
                }
            }

            return new JsonResult("Added Successfully");
        }

        [HttpDelete("condemn/{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"DELETE FROM dbo.Condemned_Stocks WHERE stockID = @stockID";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@stockID", id);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Data is successfully Deleted");
        }

        [HttpPost("issue-condemned-stock")]
        public JsonResult PostIssueCondemnedStock(IssueCondemnedStock_RIS _issueCondemnedStock_RIS)
        {
            try
            {
                string IssueQuery = @"INSERT INTO dbo.Condemn_RIS (PONo, PRNo, StockNo, Condemn_RequestedBy, Condemn_Amount,
                                    Condemn_RequestorDesignation, Condemn_RequestedQuantity, Condemn_RequestorOffice,
                                    Condemn_RequestorDepartment, Condemn_DateRequested,
                                    Condemn_Receiver, Condemn_ReceiverDesignation, Condemn_DateReceived, Condemn_IssuedBy,
                                    Condemn_IssuerDesignation, Condemn_Purpose)
                                    VALUES (@PONo, @PRNo, @StockNo, @Condemn_RequestedBy, @Condemn_Amount,
                                    @Condemn_RequestorDesignation, @Condemn_RequestedQuantity, @Condemn_RequestorOffice,
                                    @Condemn_RequestorDepartment, @Condemn_DateRequested,
                                    @Condemn_Receiver, @Condemn_ReceiverDesignation, @Condemn_DateReceived, @Condemn_IssuedBy,
                                    @Condemn_IssuerDesignation, @Condemn_Purpose)";

                string UpdateCondemnedStocks = "UPDATE dbo.Condemned_Stocks SET AvailableQty = @UpdatedAvailableQty, ConsumedQty += @UpdatedConsumedQty WHERE StockNo = @StockNo";

                DataTable table = new();
                string SqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(SqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(IssueQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PONo", _issueCondemnedStock_RIS.PONo);
                        cmd.Parameters.AddWithValue("@PRNo", _issueCondemnedStock_RIS.PRNo);
                        cmd.Parameters.AddWithValue("@StockNo", _issueCondemnedStock_RIS.StockNo);
                        cmd.Parameters.AddWithValue("@Condemn_RequestedBy", _issueCondemnedStock_RIS.Condemn_RequestedBy);
                        cmd.Parameters.AddWithValue("@Condemn_Amount", _issueCondemnedStock_RIS.Condemn_Amount);
                        cmd.Parameters.AddWithValue("@Condemn_RequestorDesignation", _issueCondemnedStock_RIS.Condemn_RequestorDesignation);
                        cmd.Parameters.AddWithValue("@Condemn_RequestedQuantity", _issueCondemnedStock_RIS.Condemn_RequestedQuantity);
                        cmd.Parameters.AddWithValue("@Condemn_RequestorOffice", _issueCondemnedStock_RIS.Condemn_RequestorOffice);
                        cmd.Parameters.AddWithValue("@Condemn_RequestorDepartment", _issueCondemnedStock_RIS.Condemn_RequestorDepartment);
                        cmd.Parameters.AddWithValue("@Condemn_DateRequested", _issueCondemnedStock_RIS.Condemn_DateRequested);
                        cmd.Parameters.AddWithValue("@Condemn_Receiver", _issueCondemnedStock_RIS.Condemn_Receiver);
                        cmd.Parameters.AddWithValue("@Condemn_ReceiverDesignation", _issueCondemnedStock_RIS.Condemn_ReceiverDesignation);
                        cmd.Parameters.AddWithValue("@Condemn_DateReceived", _issueCondemnedStock_RIS.Condemn_DateReceived);
                        cmd.Parameters.AddWithValue("@Condemn_IssuedBy", _issueCondemnedStock_RIS.Condemn_IssuedBy);
                        cmd.Parameters.AddWithValue("@Condemn_IssuerDesignation", _issueCondemnedStock_RIS.Condemn_IssuerDesignation);
                        cmd.Parameters.AddWithValue("@Condemn_Purpose", _issueCondemnedStock_RIS.Condemn_Purpose);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);
                    }

                    using (SqlCommand cmd = new SqlCommand(UpdateCondemnedStocks, conn))
                    {
                        cmd.Parameters.AddWithValue("@StockNo", _issueCondemnedStock_RIS.StockNo);
                        cmd.Parameters.AddWithValue("@UpdatedAvailableQty", _issueCondemnedStock_RIS.AvailableQty);
                        cmd.Parameters.AddWithValue("@UpdatedConsumedQty", _issueCondemnedStock_RIS.ConsumedQty);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }

                        return new JsonResult("Condemned stock successfully issued!");
                }
            } catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
