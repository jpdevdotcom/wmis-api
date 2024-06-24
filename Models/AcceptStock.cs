namespace UEPWarehouse_API.Models
{
    public class AcceptStock
    {
        public int StockNo { get; set; }
        public string PONo { get; set; }
        public string PRNo { get; set; }
        public int? Partial_Quantity { get; set; }
        public bool is_accepted { get; set; }
        public bool is_available { get; set; }
    }
}
