namespace METALLIST.Models
{
    public class Order
    {
        public int Id {  get; set; }
        public string? Name {  get; set; }
        public string? Description {  get; set; }
        public decimal Price {  get; set; }
        public string? TypeOfPaint { get; set; }
        public string? Difficulty {  get; set; }
        public int CustomerId {  get; set; }
        public string? OrderDate { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<OrderMaterial>? OrderMaterials { get; set; }
    }
}
