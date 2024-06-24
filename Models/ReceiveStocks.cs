namespace UEPWarehouse_API.Models
{
    public class ReceiveStocks
    {
        public int DRNo { get; set; }
        public string PRNo { get; set; }
        public int StockNo { get; set; }
        public string RCC { get; set; }
        public bool is_available { get; set; }
    }
}
