using System.ComponentModel.DataAnnotations;

namespace METALLIST.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле 'Полное имя' обязательно.")]
        [StringLength(100, ErrorMessage = "Полное имя должно быть не более 100 символов.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Поле 'Номер телефона' обязательно.")]
        [Phone(ErrorMessage = "Некорректный номер телефона.")]
        public string? PhoneNumber { get; set; }
        public string? Requisites { get; set; }
        public string? OrganizationName { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
