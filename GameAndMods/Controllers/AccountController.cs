using GameAndMods.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using GameAndMods.ViewModel;

namespace GameAndMods.Controllers
{
    public class AccountController : Controller
    {
        public FileContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccountController(FileContext content, IWebHostEnvironment hostEnvironment)
        {
            this.db = content;
            webHostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Create()
        {
            return await Task.Run(() => View());
        } 


        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            return await Task.Run(() => View());
        }

        private string UploadedFile(UsersModel model)
        {
            string uniqueFileName = null;

            if (model.AvatarURL != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/Avatars");
                uniqueFileName = /*Guid.NewGuid().ToString() + "_" +*/ model.AvatarURL.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.AvatarURL.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsersModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueFileName = UploadedFile(model);

                    Users employee = new Users
                    {
                        Email = model.Email,
                        Password = model.Password,
                        AvatarURL = uniqueFileName,
                    };
                    if (model.Position == "Адинистратор") model.Position = "Admin";
                    db.Add(employee);
                    await db.SaveChangesAsync();
                    if (model.Position != null)
                    {
                        await Authenticate(model.Email, model.Position);
                    }
                    else
                    {
                        await AuthenticateDontPodition(model.Email);
                    }

                    return RedirectToAction(nameof(Profile));
                }
                return await Task.Run(() => View(model)); ;
            }
            catch
            {
                return RedirectToAction("Create", "Account");
            }
        }
    
        [Authorize]
        public async Task<IActionResult> Profile()
        {
           string email = HttpContext.User.Claims.FirstOrDefault().Value;
           Users user = db.Users.Where(x => x.Email == email).FirstOrDefault();
           return View(user);
        }
        [HttpGet]
        public  IActionResult Autorise()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Autorise(Users model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Users users = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                    if (users != null)
                    {
                        if (users.Position != null)
                        {
                            await Authenticate(model.Email, users.Position);
                        }
                        else
                        {
                            await AuthenticateDontPodition(model.Email);
                        }
                        return RedirectToAction("Profile", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                    }
                }
                return await Task.Run(() => View(model));
            }
            catch
            {
                return await Task.Run(() => View(model));
            }
        }
        private async Task Authenticate(string userName, string positionUser)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, positionUser)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        private async Task AuthenticateDontPodition(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Autorise", "Account");
        }
        
    }
}
