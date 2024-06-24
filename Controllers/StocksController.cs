using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StocksController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public StocksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        public JsonResult Get()
        {
                string query = @"SELECT DISTINCT * FROM dbo.Saved AS Stocks
                                INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                                INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
							    ORDER BY is_inspected DESC";

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

        [HttpGet("get-all-stock")]
        public JsonResult GetIssued_tobeIssuedStock()
        {
            string query = @"SELECT DISTINCT * FROM dbo.Saved AS Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
							INNER JOIN dbo.RIS AS RIS ON Stocks.PRNo = RIS.PRNo
							INNER JOIN dbo.PAR AS PAR ON Stocks.PRNo = RIS.PRNo";

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
        [HttpGet("get-stockedIn-items")]
        public JsonResult GetStockedInItems()
        {
            string query = @"SELECT PO.FundCluster, Description, Unit, SUM(Quantity - StockOutQuantity) AS Quantity FROM Saved 
							INNER JOIN PurchaseOrder AS PO ON PO.PRNo = Saved.PRNo
							WHERE is_inspected = 'true' AND StockInQuantity != 0 AND StockOutQuantity != StockInQuantity
                            GROUP BY Description, UNIT, PO.FundCluster";

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
        [HttpGet("get-stockedOut-items")]
        public JsonResult GetStockedOutItems()
        {
            string query = @"SELECT DISTINCT * FROM dbo.Saved AS Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
							INNER JOIN dbo.RIS AS RIS ON Stocks.PRNo = RIS.PRNo
							INNER JOIN dbo.PAR AS PAR ON Stocks.PRNo = RIS.PRNo
							WHERE Stocks.is_inspected = 'true' AND Stocks.is_available = 'false' AND Stocks.is_issued = 'true' AND Stocks.StockOutQuantity != 0";

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

        [HttpPost("stock-in-item")]
        public JsonResult StockInItem(StockInItem stockInItem)
        {
            string STOCK_IN_ITEM = @"UPDATE dbo.Saved SET is_available = 'true', StockInQuantity =+ @stockInQuantity WHERE PRNo = @prNo AND Description = @description";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(STOCK_IN_ITEM, conn))
                {
                    cmd.Parameters.AddWithValue("@stockInQuantity", stockInItem.StockInQuantity);
                    cmd.Parameters.AddWithValue("@prNo", stockInItem.PRNo);
                    cmd.Parameters.AddWithValue("@description", stockInItem.Description);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Item Stocked In!");
        }
        [HttpPost("stock-out-item")]
        public JsonResult StockOutItem(StockOutItem stockOutItem)
        {
            string STOCK_OUT_ITEM = @"UPDATE dbo.Saved SET is_available = 'false', StockOutQuantity = @stockOutQuantity WHERE PRNo = @prNo AND Description = @description";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(STOCK_OUT_ITEM, conn))
                {
                    cmd.Parameters.AddWithValue("@stockOutQuantity", stockOutItem.StockOutQuantity);
                    cmd.Parameters.AddWithValue("@prNo", stockOutItem.PRNo);
                    cmd.Parameters.AddWithValue("@description", stockOutItem.Description);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Item Stocked Out!");
        }

        // Issue Stocks
        [HttpPost("stock-out")]
        public JsonResult PostPAR(StockOut _par)
        {
            string INSERT_PAR = @"INSERT INTO dbo.PAR (PRNo, Entity_Name, PARDate_received, Accepted_by)    
                            VALUES (@prno, @entity_name, @pardate_received, @accepted_by)";

            string INSERT_RIS = @"INSERT INTO dbo.RIS (PRNo, Remarks, RIS_ReceivedBy, RIS_ReceiverDesignation, RIS_ReceivedDate, Issued_by,
                                Issuer_Designation, Issued_Date, is_received)
                                VALUES (@prno, @remarks, @ris_receivedBy, @ris_receiverDesignation, @ris_receivedDate, @issued_by,
                                @issuer_designation, @issued_date, 'true')";

            string UPDATE_STOCK = @"UPDATE dbo.Saved SET RCC = @rcc, is_issued = 'true', is_available = 'false', 
                                DRNo = @drno WHERE PRNo = @prNo AND is_inspected = 'true' AND is_issued = 'false' AND StockOutQuantity != 0";

            string INSERT_LOG = @"INSERT INTO dbo.wmis_logs (user_fname, user_lname, user_position, user_photo, user_activity, log_time) 
                            VALUES (@user_fname, @user_lname, @user_position, @user_photo, @user_activity, @log_time)";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(INSERT_RIS, conn))
                {
                    cmd.Parameters.AddWithValue("@prNo", _par.PRNo);
                    cmd.Parameters.AddWithValue("@remarks", _par.Remarks);
                    cmd.Parameters.AddWithValue("@ris_receivedBy", _par.RIS_ReceivedBy);
                    cmd.Parameters.AddWithValue("@ris_receiverDesignation", _par.RIS_ReceiverDesignation);
                    cmd.Parameters.AddWithValue("@ris_receivedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@issued_by", _par.Issued_by);
                    cmd.Parameters.AddWithValue("@issuer_designation", _par.Issuer_Designation);
                    cmd.Parameters.AddWithValue("@issued_date", DateTime.Now);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                }

                using (SqlCommand cmd = new SqlCommand(INSERT_PAR, conn))
                {
                    cmd.Parameters.AddWithValue("@prno", _par.PRNo);
                    cmd.Parameters.AddWithValue("@entity_name", _par.EntityName);
                    cmd.Parameters.AddWithValue("@pardate_received", DateTime.Now);
                    cmd.Parameters.AddWithValue("@accepted_by", _par.Accepted_by);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                }

                using (SqlCommand cmd = new SqlCommand(UPDATE_STOCK, conn))
                {
                    cmd.Parameters.AddWithValue("@stockNo", _par.StockNo);
                    cmd.Parameters.AddWithValue("@prno", _par.PRNo);
                    cmd.Parameters.AddWithValue("@rcc", _par.RCC);
                    cmd.Parameters.AddWithValue("@drno", _par.DRNo);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                }

                using (SqlCommand cmd = new SqlCommand(INSERT_LOG, conn))
                {
                    cmd.Parameters.AddWithValue("@user_lname", _par.user_lname);
                    cmd.Parameters.AddWithValue("@user_fname", _par.user_fname);
                    cmd.Parameters.AddWithValue("@user_position", _par.user_position);
                    cmd.Parameters.AddWithValue("@user_photo", _par.user_photo);
                    cmd.Parameters.AddWithValue("@user_activity", _par.user_activity);
                    cmd.Parameters.AddWithValue("@log_time", DateTime.Now);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Successfully Updated!");
        }
        [HttpPut("edit-stock-out")]
        public JsonResult EditStockOut(StockOutItem stockOut)
        {
            string UPDATE_STOCK = @"UPDATE dbo.Saved SET StockOutQuantity = @stockOutQuantity WHERE PRNo = @prNo AND Description = @description";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(UPDATE_STOCK, conn))
                {
                    cmd.Parameters.AddWithValue("@prno", stockOut.PRNo);
                    cmd.Parameters.AddWithValue("@description", stockOut.Description);
                    cmd.Parameters.AddWithValue("@stockOutQuantity", stockOut.StockOutQuantity);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);


                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Successfully Updated!");
        }
        [HttpPut("edit-issued-stock")]
        public JsonResult EditIssuedStock(StockOutItem stockOut)
        {
            string UPDATE_STOCK_SAVED = @"UPDATE dbo.Saved SET DRNo = @drno WHERE PRNo = @prNo AND Description = @description";
            string UPDATE_STOCK_RIS = @"UPDATE dbo.RIS SET RIS_ReceivedBy = @risReceivedBy, RIS_ReceiverDesignation = @ris_ReceiverDesignation WHERE PRNo = @prNo";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(UPDATE_STOCK_RIS, conn))
                {
                    cmd.Parameters.AddWithValue("@prno", stockOut.PRNo);
                    cmd.Parameters.AddWithValue("@risReceivedBy", stockOut.RIS_ReceivedBy);
                    cmd.Parameters.AddWithValue("@ris_ReceiverDesignation", stockOut.RIS_ReceiverDesignation);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                }

                using (SqlCommand cmd = new SqlCommand(UPDATE_STOCK_SAVED, conn))
                {
                    cmd.Parameters.AddWithValue("@prno", stockOut.PRNo);
                    cmd.Parameters.AddWithValue("@description", stockOut.Description);
                    cmd.Parameters.AddWithValue("@drno", stockOut.DRNo);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Successfully Updated!");
        }

        /*----------------------------------EDIT STOCKS--------------------------------------------*/

        [HttpPut("edit-stock")]
        public JsonResult InspectStock(EditStock _editStock)
        {
            try
            {
                string UPDATE_RIS = @"UPDATE dbo.RIS SET RIS_ReceivedBy = @RIS_ReceivedBy, RIS_ReceiverDesignation = @RIS_ReceiverDesignation WHERE PRNo = @_prNo AND StockNo = @_stockno";
                string UPDATE_PAR = @"UPDATE dbo.PAR SET Accepted_by = @Accepted_by WHERE PRNo = @_prNo AND StockNo = @_stockno";

                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(UPDATE_RIS, conn))
                    {
                        cmd.Parameters.AddWithValue("@_prNo", _editStock.PRNo);
                        cmd.Parameters.AddWithValue("@_stockno", _editStock.StockNo);
                        cmd.Parameters.AddWithValue("@RIS_ReceivedBy", _editStock.RIS_ReceivedBy);
                        cmd.Parameters.AddWithValue("@RIS_ReceiverDesignation", _editStock.RIS_ReceiverDesignation);

                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd2 = new SqlCommand(UPDATE_PAR, conn))
                    {
                        cmd2.Parameters.AddWithValue("@_prNo", _editStock.PRNo);
                        cmd2.Parameters.AddWithValue("@_stockno", _editStock.StockNo);
                        cmd2.Parameters.AddWithValue("@Accepted_by", _editStock.Accepted_by);

                        cmd2.ExecuteNonQuery();

                        conn.Close();
                    }
                }
                return new JsonResult("Stock is Updated!");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        /*----------------------------------EDIT STOCKS--------------------------------------------*/


        /*----------------------------------PENDING STOCKS--------------------------------------------*/

        [HttpGet("get-pending-stocks")]
        public JsonResult GetPendingStocks()
        {
            string query = @"SELECT Stocks.*, PO.*, PR.* FROM dbo.Saved AS Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
                            WHERE Stocks.is_inspected = 'true' AND Stocks.is_accepted = 'true'
                            AND Stocks.is_issued = 'false' AND Stocks.is_available = 'true'";

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
        /*----------------------------------</PENDING STOCKS>--------------------------------------------*/


        /*----------------------------------RECEIVE STOCKS--------------------------------------------*/

        [HttpGet("receive-stocks")]
        public JsonResult GetReceiveStocksData()
        {
            string query = @"SELECT STOCKS.*, PO.*, PR.* FROM dbo.Saved AS STOCKS
                            INNER JOIN dbo.PurchaseOrder AS PO ON STOCKS.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON STOCKS.PRNo = PR.PRNo
                            WHERE STOCKS.is_inspected = 'true' AND STOCKS.is_accepted = 'true'
                            AND STOCKS.is_issued = 'false' AND STOCKS.is_available = 'false'";

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
        /*----------------------------------</RECEIVE STOCKS>--------------------------------------------*/

        
        /*----------------------------------INSPECTORATE VIEW STOCKS--------------------------------------------*/

        [HttpGet("get-inspectorate")]
        public JsonResult GetStocks_SupplyOffice()
        {
            string query = @"SELECT *, PR.Date AS REQUESTED_DATE FROM dbo.Saved AS Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo";
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

        [HttpGet("refresh-inspectorate-sfgrid/{PO_NO}")]
        public JsonResult RefreshInspectorageGrid(string PO_NO)
        {
            try
            {
                string query = @"SELECT *, PR.Date AS REQUESTED_DATE FROM dbo.Saved AS Stocks
                                INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                                INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
                                WHERE is_inspected = 'false' AND PO.PONo = @PO_NO";
                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PO_NO", PO_NO);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }
                }

                return new JsonResult(table);


            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        /*----------------------------------</INSPECTORATE VIEW STOCKS>--------------------------------------------*/


        /*----------------------------------OIC VIEW STOCKS--------------------------------------------*/

        [HttpGet("get-supply-office")]
        public JsonResult GetSupplyOfficeData()
        {
            try
            {
                string query = @"SELECT Stocks.*, PO.*, PR.* FROM dbo.Saved AS Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
                            WHERE is_inspected = 'true' AND is_available = 'false' AND is_accepted = 'false'";
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
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
        /*----------------------------------</OIC VIEW STOCKS>--------------------------------------------*/


        /*----------------------------------END USER VIEW STOCKS------------------------------------------*/

        // END USER SELECTOR
        [HttpGet("end-user-view-selector")]
        public JsonResult EndUserViewSelector()
        {
            try
            {
                string query = @"SELECT DISTINCT PO.PONo, PR.PRNo FROM dbo.PurchaseOrder AS PO
                                INNER JOIN dbo.PurchaseRequest AS PR ON PR.PRNo = PO.PRNo";
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
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }


        // PR NUMBER TYPE
        [HttpGet("end-user-pr-view-stock/{PR_NUMBER}")] 
        public JsonResult EndUserPRViewStock(string PR_NUMBER)
        {
            try
            {
                string query = @"SELECT * ,
                                    CASE
	                                    WHEN is_issued = 'true' THEN 'Available'
	                                    WHEN is_issued = 'false' THEN 'On Process'
                                    END AS STATUS
                                    FROM dbo.Saved AS Stocks 
                                    INNER JOIN dbo.PurchaseRequest AS PR ON PR.PRNo = Stocks.PRNo
                                    INNER JOIN dbo.PurchaseOrder AS PO ON PO.PRNo = Stocks.PRNo
                                    WHERE PR.PRNo = @PR_NUMBER";
                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PR_NUMBER", PR_NUMBER);

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

        // PO NUMBER TYPE
        [HttpGet("end-user-po-view-stock/{PO_NUMBER}")]
        public JsonResult EndUserViewPOStock(string PO_NUMBER)
        {
            try
            {

                string query = @"SELECT * FROM dbo.Saved AS Stocks 
                                INNER JOIN dbo.PurchaseRequest AS PR ON PR.PRNo = Stocks.PRNo
                                INNER JOIN dbo.PurchaseOrder AS PO ON PO.PRNo = Stocks.PRNo
                                WHERE PO.PONo = @PO_NUMBER";
                DataTable table = new();
                string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
                SqlDataReader myReader;

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PO_NUMBER", PO_NUMBER);

                        myReader = cmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        conn.Close();
                    }
                }

                return new JsonResult(table);


            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }


        [HttpGet("get-badge-count")]
        public JsonResult GetBadgeCount()
        {
            try
            {

                string query = @"SELECT COUNT(*) AS TotalCount
                                    FROM (
                                        SELECT PO.PRNo
                                        FROM dbo.Saved AS Stocks
                                        INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                                        INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
                                        WHERE is_inspected = 'true' AND is_issued = 'false'                                        
                                        GROUP BY PO.PRNo
                                    ) AS UniquePRs;";
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
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        /*------------------------------------------------------------------------------------------------*/
    }
}
