namespace UEPWarehouse_API.Models
{
    public class CondemnedStocks
    {
        public int StockNo { get; set; }
        public string PRNo { get; set; }
        public int ConsumedQty { get; set; }
        public int AvailableQty { get; set; }
        public string Status { get; set; }
        public bool is_returned { get; set; }
    }
}
