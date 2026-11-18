using System.ComponentModel.DataAnnotations;

namespace WebApplication14.Models
{
    public class ItemFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Введите описание")]
        public string Description { get; set; }

        [Range(0, 100000, ErrorMessage = "Цена от 0 до 100000")]
        public int Price { get; set; }
    }

    public class ProfileModel
    {
        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный email")]
        public string Email { get; set; }

        public DateTime RegisteredAt { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Введите старый пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [MinLength(6, ErrorMessage = "Минимум 6 символов")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}