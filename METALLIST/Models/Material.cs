using System.ComponentModel.DataAnnotations;

namespace METALLIST.Models
{
    public class Material
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле 'Название' обязательно.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public ICollection<OrderMaterial>? OrderMaterials { get; set; }
    }
}
