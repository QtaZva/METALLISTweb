namespace METALLIST.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
