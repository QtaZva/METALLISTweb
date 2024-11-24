namespace METALLIST.Models
{
    public class OrderMaterial
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int MaterialId { get; set; }
        public Material? Material { get; set; }
        public int Quantity { get; set; }
    }
}
