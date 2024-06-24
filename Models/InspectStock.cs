namespace UEPWarehouse_API.Models
{
    public class InspectStock
    {
        public int StockNo { get; set; }
        public string PRNo { get; set; }
        public string RCC { get; set; }
        public bool is_inspected { get; set; }
        public string InspectedBy { get; set; }
        public bool is_complete { get; set; }
        public bool is_partial { get; set; }
        public int Partial_Quantity { get; set; }

        //RECORD LOGS
        public string user_fname { get; set; }
        public string user_lname { get; set; }
        public string user_position { get; set; }
        public string user_photo { get; set; }
        public string user_activity { get; set; }
    }
}
