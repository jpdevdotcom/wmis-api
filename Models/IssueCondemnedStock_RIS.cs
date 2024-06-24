namespace UEPWarehouse_API.Models
{
    public class IssueCondemnedStock_RIS
    {
        public string PONo { get; set; }
        public string PRNo { get; set; }
        public int StockNo { get; set; }
        public string Condemn_RequestedBy { get; set; }
        public int Condemn_Amount { get; set; }
        public string Condemn_RequestorDesignation { get; set; }
        public int Condemn_RequestedQuantity { get; set; }
        public string Condemn_RequestorOffice { get; set;    }
        public string Condemn_RequestorDepartment { get; set; }
        public DateTime Condemn_DateRequested { get; set; }
        public string Condemn_Receiver { get; set; }
        public string Condemn_ReceiverDesignation { get; set; }
        public DateTime Condemn_DateReceived { get; set; }
        public string Condemn_IssuedBy { get; set; }
        public string Condemn_IssuerDesignation { get; set; }
        public string Condemn_Purpose { get; set; }

        public int AvailableQty { get; set; } //Updated Available Quantity (AvailableQty - RequestedQuantity)
        public int ConsumedQty { get; set; }
    }
}
