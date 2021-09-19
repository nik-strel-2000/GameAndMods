using System.ComponentModel.DataAnnotations;
namespace GameAndMods.Models
{
    public class Game
    {
        [Key]
        public int Id { set; get; }
        [Required(ErrorMessage = "Название")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина от 3 до 50 символов")]
        public string Title { set; get; }
        [Required(ErrorMessage = "Описание ")]
        [StringLength(300, MinimumLength = 3, ErrorMessage = "Длина от 3 до 300 символов")]
        public string Description { set; get; }
        

        [Required(ErrorMessage = "Код категории")]
        public int Categori_ID { set; get; }
        [Required(ErrorMessage = "Картинка")]
        public string Img1 { set; get; }
        public string Img2 { set; get; }
    }
}
