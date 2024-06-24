namespace UEPWarehouse_API.Models
{
    public class Stocks
    {
        public int SaveId { get; set; }
        public int StockNo { get; set; }
        public string Unit {  get; set; }
        public string Amount { get; set; }
        public string PRNo { get; set; }
        public string RCC { get; set; }
        public int Quantity { get; set; }
        public bool is_inspected { get; set; }
        public bool is_accepted { get; set; }
        public bool is_issued { get; set; } 
        public bool is_available { get; set; }
    }
}
