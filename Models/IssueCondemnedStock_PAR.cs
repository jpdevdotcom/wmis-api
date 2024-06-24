namespace UEPWarehouse_API.Models
{
    public class IssueCondemnedStock_PAR
    {
        public string PRNo { get; set; }
        public int StockNo { get; set; }
        public int Condemn_PropertyNo { get; set; }
        public string Condemn_ReceivedBy { get; set; }
        public string Condemn_EntityName { get; set; }
        public DateTime Condemn_PAR_DateReceived { get; set; }

        //RECORD LOGS
        public string user_fname { get; set; }
        public string user_lname { get; set; }
        public string user_position { get; set; }
        public string user_photo { get; set; }
        public string user_activity { get; set; }
    }
}
