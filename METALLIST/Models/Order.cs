namespace METALLIST.Models
{
    public class Order
    {
        private int Id {  get; set; }
        public string Name {  get; set; }
        public string Description {  get; set; }
        public decimal Price {  get; set; }
        public string TypeOfPaint { get; set; }
        public string Difficulty {  get; set; }
        public int MaterialsId {  get; set; }
        public Material Material { get; set; }
        public int CustomerId {  get; set; }
        public Customer Customer { get; set; }
    }
}
