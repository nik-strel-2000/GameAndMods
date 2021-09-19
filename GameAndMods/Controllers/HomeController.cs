using GameAndMods.Models;
using GameAndMods.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;


namespace GameAndMods.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public FileContext db;

        public HomeController(FileContext content)
        {
            db = content;
        }
        public IActionResult MainMenu()
        {
            return View();
        }
        public async Task<IActionResult> Buy()
        {
            var basket = db.Basket.Where(p => p.Email_user == User.Identity.Name);
            foreach (var lol in basket)
            {
                Basket basket1 = await db.Basket.FirstOrDefaultAsync(p => p.ID_Record == lol.ID_Record);

                Order order = new Order
                {
                    Email_user = User.Identity.Name,
                    Game_ID = lol.Game_ID
                };
                db.Remove(basket1);
                db.Add(order);

            }
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Basket));
        }
        public async Task<IActionResult> Repos()
        {
            var reqest = db.Users.FromSqlRaw("Select Id FROM Users").ToListAsync();
            var all = await db.Database.ExecuteSqlRawAsync($"SELECT id from [Game]");
            await db.SaveChangesAsync();
            return await Task.Run(() => View());
        }
        public async Task<IActionResult> AddToElement()
        {
            int ID = Convert.ToInt32(Request.Query["ID"]);

            string Email = User.Identity.Name;

            Basket basket = new Basket
            {
                Email_user = Email,
                Game_ID = ID
            };
            await db.AddAsync(basket);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Catalog));
        }

        public async Task<IActionResult> Remove()
        {
            int ID = Convert.ToInt32(Request.Query["ID"]);
            Basket basket = await db.Basket.FirstOrDefaultAsync(p => p.ID_Record == ID);
            if (basket != null)
            {
                db.Remove(basket);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Basket));
            }
            return RedirectToAction(nameof(Basket));
        }
        public async Task<IActionResult> TestAnimation()
        {
            return await Task.Run(() => View());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public async Task<IActionResult> Basket()
        {
            var Record = await (from m in db.Basket
                                join t in db.Game on m.Game_ID equals t.Id
                                select new DobleBasket
                                {
                                    ID_Record = m.ID_Record,
                                    Email_user = m.Email_user,
                                    Game_ID = m.Game_ID,
                                    Name_Game = t.Title
                                }).ToListAsync();
            var recordSelect = Record.Where(p => p.Email_user == User.Identity.Name);
            return await Task.Run(() => View(recordSelect));
        }
        [Authorize]
        public async Task<IActionResult> Order()
        {
            var orders = await (from m in db.Order
                                join t in db.Game on m.Game_ID equals t.Id
                                select new DoubleOrder
                                {
                                    ID_Order = m.ID_Order,
                                    Email_user = m.Email_user,
                                    Game_ID = m.Game_ID,
                                    Name_Game = t.Title
                                }).ToListAsync();
            var orderSelect = orders.Where(p => p.Email_user == User.Identity.Name);
            return await Task.Run(() => View(orderSelect));
        }
        public async Task<IActionResult> Catalog()
        {
            var obj = await (from m in db.Game
                             join t in db.Categori on m.Categori_ID equals t.ID_Categori
                             select new DobleModel
                             {
                                 ID_Categori = t.ID_Categori,
                                 NameCategori = t.NameCategori,
                                 Id = m.Id,
                                 Title = m.Title,
                                 Description = m.Description,
                                 Categori_ID = m.Categori_ID,
                                 CategoriName = t.NameCategori,
                                 Img1 = m.Img1,
                                 Img2 = m.Img2
                             }).ToListAsync();
            return View(obj);
        }
        public async Task<IActionResult> AddProduct()
        {
            return await Task.Run(() => View());
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Game gameModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    Game game = new Game
                    {
                        Title = gameModel.Title,
                        Description = gameModel.Description,
                        Img1 = gameModel.Img1,
                        Img2 = gameModel.Img1,
                        Categori_ID = gameModel.Categori_ID
                    };
                    db.Add(game);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(AddProduct));
                }
                return await Task.Run(() => View(gameModel));
            }
            catch
            {
                return RedirectToAction(nameof(AddProduct));
            }

        }
    }
}
