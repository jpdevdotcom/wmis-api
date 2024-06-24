using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UEPWarehouse_API.Models
{
    public class WMIS_Users
    {
        public int UserID { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string UserType { get; set; }
        public string PhotoFileName { get; set; }
    }
}
