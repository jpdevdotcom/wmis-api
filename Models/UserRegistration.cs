namespace UEPWarehouse_API.Models
{
    public class UserRegistration
    {
        public string f_name { get; set; }
        public string m_name { get; set; }
        public string l_name { get; set; }
        public string photoFilename { get; set; }
        public string contactNumber { get; set; }
        public string emailAddress { get; set; }
        public int age { get; set; }
        public string gender { get; set; }
        public string position { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string userType { get; set; }
    }
}
