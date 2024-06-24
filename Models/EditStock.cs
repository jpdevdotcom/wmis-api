namespace UEPWarehouse_API.Models
{
    public class EditStock
    {
        public int StockNo { get; set; }
        public string PRNo { get; set; }
        public string? RIS_ReceivedBy { get; set; }
        public string? RIS_ReceiverDesignation { get; set; }
        public string? Accepted_by { get; set; }

    }
}
