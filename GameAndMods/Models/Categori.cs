using System.ComponentModel.DataAnnotations;
namespace GameAndMods.Models
{
    public class Categori
    {
        [Key]
        public int ID_Categori { set; get; }
        public string NameCategori { set; get; }
    }
}
