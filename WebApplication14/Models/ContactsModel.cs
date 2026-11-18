using System.ComponentModel.DataAnnotations;

namespace WebApplication14.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите сообщение")]
        public string Message { get; set; }
    }
}