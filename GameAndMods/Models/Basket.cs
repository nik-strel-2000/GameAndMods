using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAndMods.Models
{
    public class Basket
    {
        [Key]
        public int ID_Record { set; get; }
        public string Email_user { set; get; }
        public int Game_ID { set; get; }
    }
}
