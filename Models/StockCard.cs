namespace UEPWarehouse_API.Models
{
    public class StockCard
    {
        public int StockNo { get; set; }
        public string PONo { get; set; }
        public string PRNo { get; set; }
        public string FundCluster { get; set; }
        public DateTime? Estimated_StockOutDate { get; set; }
        public int Estimated_StockOutQuantity { get; set; }
        public int DaysToConsume { get; set; }
    }
}
