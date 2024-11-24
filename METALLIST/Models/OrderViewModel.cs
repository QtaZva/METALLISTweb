namespace METALLIST.Models
{
    public class OrderViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string TypeOfPaint { get; set; }
        public string Difficulty { get; set; }
        public int CustomerId { get; set; }
        public List<OrderMaterialViewModel> OrderMaterials { get; set; } = new();
    }
    public class OrderMaterialViewModel
    {
        public int MaterialId { get; set; }
        public int Quantity { get; set; }
    }
}
