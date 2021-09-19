using GameAndMods.Models;
using Microsoft.EntityFrameworkCore;
namespace GameAndMods.Models
{
    public class FileContext : DbContext
    {
        public DbSet<Game> Game { get; set; }
        public DbSet<Categori> Categori { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<Order> Order{ get; set; }
        public FileContext(DbContextOptions<FileContext> options) ////ПОПРаВЬ ЕСЛИ НЕ РАБОТАЕТ
        : base(options)
        {
            //Database.EnsureCreatedAsync();
        }
        
    }
}
