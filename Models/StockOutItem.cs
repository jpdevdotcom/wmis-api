namespace UEPWarehouse_API.Models
{
    public class StockOutItem
    {
        public string? PRNo { get; set; }
        public string? DRNo { get; set; }
        public string? Description { get; set; }
        public string? RIS_ReceivedBy { get; set; }
        public string? RIS_ReceiverDesignation { get; set; }
        public int? StockOutQuantity { get; set; }
    }
}
