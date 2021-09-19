using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameAndMods.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress(ErrorMessage = "Введите правильный Email")]
        [Required(ErrorMessage = "Не указан Email")]
        [Display(Name = "Email адрес")]
        [Remote(action: "CheckEmail", controller: "Account", ErrorMessage = "Email уже используется")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Введите пароль")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина пароля должна быть от 3 до 50 символов")]
        public string Password { get; set; }
        public string Position { get; set; }
        public string AvatarURL { get; set; }
    }
}
