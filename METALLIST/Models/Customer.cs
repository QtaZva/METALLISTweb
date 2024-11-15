namespace METALLIST.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Requisites { get; set; }
        public string OrganizationName { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
