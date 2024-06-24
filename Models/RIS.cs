namespace UEPWarehouse_API.Models
{
    public class RIS
    {
        public int StockNo { get; set; }
        public string Remarks { get; set; }
        public string RIS_ReceivedBy { get; set; }
        public string RIS_ReceiverDesignation { get; set; }
        public DateTime RIS_ReceivedDate { get; set; }
        public string PONo { get; set; }
        public string PRNo { get; set; }
        public string? Issued_By { get; set; }
        public string? Issuer_Designation { get; set; }
        public int? Issued_Quantity { get; set; }
        public DateTime Issued_Date { get; set; }
        public bool is_available { get; set; }
        public bool is_issued { get; set; }
        public int totalIssuedQuantity { get; set; }

        // PAR Table
        public string AcceptedBy { get; set; }

        // Stock Card
        public string FundCluster { get; set; }
        public DateTime Estimated_StockOutDate { get; set; }
        public int? Estimated_StockOutQuantity { get; set; }

        //RECORD LOGS
        public string user_fname { get; set; }
        public string user_lname { get; set; }
        public string user_position { get; set; }
        public string user_photo { get; set; }
        public string user_activity { get; set; }
    }
}
