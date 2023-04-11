namespace WebExtension.Models.Customer
{
    public class RegisterUserRequest
    {
        public string displayName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string Id { get; set; }
        public bool corporateUser { get; set; }
    }
}
