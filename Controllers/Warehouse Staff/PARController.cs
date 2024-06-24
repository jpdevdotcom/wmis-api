using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using UEPWarehouse_API.Models;

namespace UEPWarehouse_API.Controllers
{
    [Route("api/par")]
    [ApiController]
    public class PARController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public PARController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
                
        [HttpPost("stock-out")]
        public JsonResult PostPAR(StockOut _par)
        {
            string INSERT_PAR = @"INSERT INTO dbo.PAR (PRNo, Entity_Name, PropertyNo, PARDate_received, Accepted_by)
                            VALUES (@prno, @entity_name, @propertyNo, @pardate_received, @accepted_by)";

            string INSERT_RIS = @"INSERT INTO dbo.RIS (PRNo, Remarks, RIS_ReceivedBy, RIS_ReceiverDesignation, RIS_ReceivedDate, Issued_by,
                                Issuer_Designation, Issued_Date, is_received)
                                VALUES (@prno, @remarks, @ris_receivedBy, @ris_receiverDesignation, @ris_receivedDate, @issued_by,
                                @issuer_designation, @issued_date, 'true')";

            string UPDATE_STOCK = @"UPDATE dbo.Saved SET RCC = @rcc, is_inspected = 'true', is_issued = 'true' 
                                DRNo = @drno WHERE PRNo = @prNo";

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
                    cmd.Parameters.AddWithValue("@pardate_received", _par.PARDate_received);
                    cmd.Parameters.AddWithValue("@accepted_by", _par.Accepted_by);

                    myReader = cmd.ExecuteReader(); 
                    table.Load(myReader);
                }

                using (SqlCommand cmd = new SqlCommand(UPDATE_STOCK, conn))
                {
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

        // Displaying of data for the issuance of condemned stocks
        [HttpGet("display-par-data")]
        public JsonResult GetCondemnedStocks_PAR()
        {
            string query = @"SELECT DISTINCT * FROM dbo.Saved AS Stocks
                            INNER JOIN dbo.PurchaseOrder AS PO ON Stocks.PRNo = PO.PRNo
                            INNER JOIN dbo.PurchaseRequest AS PR ON Stocks.PRNo = PR.PRNo 
							INNER JOIN dbo.RIS AS RIS ON Stocks.PRNo = RIS.PRNo
							WHERE Stocks.is_issued = 'true' AND Stocks.is_available = 'true' 
                            AND RIS.is_received = 'false'";

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

        //Insertion of PAR for Condemned Stocks (Condemn_PAR Table)
        [HttpPost("stock-out-condemned-par")]
        public JsonResult PostCondemnPAR(IssueCondemnedStock_PAR _issueCondemnedStock_PAR)
        {
            string query = @"INSERT INTO dbo.Condemn_PAR (PRNo, StockNo, Condemn_PropertyNo, Condemn_ReceivedBy, Condemn_EntityName, Condemn_PAR_DateReceived)
                            VALUES (@prno, @stockno, @condemn_propertyNo, @condemn_receivedBy, @condemn_entityName, @Condemn_PAR_DateReceived)";

            string recordLog_query = @"INSERT INTO dbo.wmis_logs (user_fname, user_lname, user_position, user_photo, user_activity, log_time) 
                            VALUES (@user_fname, @user_lname, @user_position, @user_photo, @user_activity, @log_time)";

            string update_Condemn_RIS = @"UPDATE dbo.Condemned_Stocks SET Condemn_isIssued = 'true' WHERE StockNo = @stockNo";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("WarehouseDB");
            SqlDataReader myReader;

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@prno", _issueCondemnedStock_PAR.PRNo);
                    cmd.Parameters.AddWithValue("@stockno", _issueCondemnedStock_PAR.StockNo);
                    cmd.Parameters.AddWithValue("@condemn_propertyNo", _issueCondemnedStock_PAR.Condemn_PropertyNo);
                    cmd.Parameters.AddWithValue("@condemn_receivedBy", _issueCondemnedStock_PAR.Condemn_ReceivedBy);
                    cmd.Parameters.AddWithValue("@condemn_entityName", _issueCondemnedStock_PAR.Condemn_EntityName);
                    cmd.Parameters.AddWithValue("@Condemn_PAR_DateReceived", _issueCondemnedStock_PAR.Condemn_PAR_DateReceived);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                }

                using (SqlCommand cmd = new SqlCommand(recordLog_query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_lname", _issueCondemnedStock_PAR.user_lname);
                    cmd.Parameters.AddWithValue("@user_fname", _issueCondemnedStock_PAR.user_fname);
                    cmd.Parameters.AddWithValue("@user_position", _issueCondemnedStock_PAR.user_position);
                    cmd.Parameters.AddWithValue("@user_photo", _issueCondemnedStock_PAR.user_photo);
                    cmd.Parameters.AddWithValue("@user_activity", _issueCondemnedStock_PAR.user_activity);
                    cmd.Parameters.AddWithValue("@log_time", DateTime.Now);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                }

                using (SqlCommand cmd = new SqlCommand(update_Condemn_RIS, conn))
                {
                    cmd.Parameters.AddWithValue("@stockNo", _issueCondemnedStock_PAR.StockNo);

                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Condemned Stock Successfully Acknowledged!");
        }
    }
}
