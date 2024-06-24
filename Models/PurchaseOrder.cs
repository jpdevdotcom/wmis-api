namespace UEPWarehouse_API.Models
{
    public class PurchaseOrder
    {
        public string POId { get; set; }
        public string Supplier {  get; set; }
        public string Address { get; set; }
        public string Purpose { get; set; }
        public string PONumber { get; set; }
        public string PRNumber { get; set; }

    }
}
