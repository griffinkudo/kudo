namespace WebExtension.Models.Order
{
    public class UpdateBasetypeAndCreateNewUserRequest
    {
        public string associateID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int associateBaseType { get; set; }
    }
}
