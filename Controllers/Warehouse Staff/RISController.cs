using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers
{
    [Route("api/ris")]
    [ApiController]
    public class RISController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public RISController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Display Stocks
        [HttpGet("display-ris-data")]
        public JsonResult GetStocks_RIS()
        {
            string query = @"SELECT *, (Stocks.Quantity - Stocks.TotalIssuedQuantity) AS TotalQuantity FROM dbo.Saved Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo
							WHERE is_inspected='true' AND is_accepted='true'
							AND is_issued='false' AND is_available = 'true'
							AND Stocks.Quantity != Stocks.TotalIssuedQuantity AND issued_stockCard = 'true'
							 ORDER BY Stocks.SaveId";
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


        //Update Stock Status
        [HttpPost("stock-out-ris")]
        public JsonResult PostRIS(RIS _ris)
        {
            string query = "INSERT INTO dbo.RIS (StockNo, PRNo, Remarks, RIS_ReceivedBy, RIS_ReceiverDesignation, RIS_ReceivedDate, Issued_by, Issuer_Designation, Issued_Quantity, Issued_Date) VALUES (@stockNo, @prNumber, @remarks, @RIS_ReceivedBy, @RIS_ReceiverDesignation, @RIS_ReceivedDate, @Issued_by, @Issuer_Designation, @Issued_Quantity, @Issued_Date); SELECT SCOPE_IDENTITY();";

            string tblSaved_query = @"UPDATE dbo.Saved SET is_issued = @is_issued, TotalIssuedQuantity += @totalIssuedQuantity WHERE PRNo = @prNo AND StockNo = @stockNo";

            string UPDATE_STOCK_CARD = @"UPDATE dbo.StockCard SET ris_id = @ris_id WHERE StockNo = @stockNo AND PONo = @poNo AND ris_id IS NULL";

            string recordLog_query = @"INSERT INTO dbo.wmis_logs (user_fname, user_lname, user_position, user_photo, user_activity, log_time) 
                            VALUES (@user_fname, @user_lname, @user_position, @user_photo, @user_activity, @log_time)";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(tblSaved_query, conn))
                {
                    cmd.Parameters.AddWithValue("@prNo", _ris.PRNo);
                    cmd.Parameters.AddWithValue("@is_issued", _ris.is_issued);
                    cmd.Parameters.AddWithValue("@totalIssuedQuantity", _ris.totalIssuedQuantity);
                    cmd.Parameters.AddWithValue("@stockNo", _ris.StockNo);

                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@stockNo", _ris.StockNo);
                    cmd.Parameters.AddWithValue("@remarks", _ris.Remarks);
                    cmd.Parameters.AddWithValue("@RIS_ReceivedBy", _ris.RIS_ReceivedBy);
                    cmd.Parameters.AddWithValue("@RIS_ReceiverDesignation", _ris.RIS_ReceiverDesignation);
                    cmd.Parameters.AddWithValue("@RIS_ReceivedDate", _ris.RIS_ReceivedDate);
                    cmd.Parameters.AddWithValue("@prNumber", _ris.PRNo);
                    cmd.Parameters.AddWithValue("@Issued_by", _ris.Issued_By);
                    cmd.Parameters.AddWithValue("@Issuer_Designation", _ris.Issuer_Designation);
                    cmd.Parameters.AddWithValue("@Issued_Quantity", _ris.Issued_Quantity);
                    cmd.Parameters.AddWithValue("@Issued_Date", _ris.Issued_Date);

                    int risID = Convert.ToInt32(cmd.ExecuteScalar());

                    using (SqlCommand cmd2 = new SqlCommand(UPDATE_STOCK_CARD, conn))
                    {

                        cmd2.Parameters.AddWithValue("@stockNo", _ris.StockNo);
                        cmd2.Parameters.AddWithValue("@poNo", _ris.PONo);
                        cmd2.Parameters.AddWithValue("@ris_id", risID);

                        cmd2.ExecuteNonQuery();
                    }
                }

                using(SqlCommand cmd = new SqlCommand(recordLog_query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_lname", _ris.user_lname);
                    cmd.Parameters.AddWithValue("@user_fname", _ris.user_fname);
                    cmd.Parameters.AddWithValue("@user_position", _ris.user_position);
                    cmd.Parameters.AddWithValue("@user_photo", _ris.user_photo);
                    cmd.Parameters.AddWithValue("@user_activity", _ris.user_activity);
                    cmd.Parameters.AddWithValue("@log_time", DateTime.Now);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }

                return new JsonResult("Successfully Updated!");
            }
        }
    }
}
