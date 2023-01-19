using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using iCoin.Data;

namespace WebProjekat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly ICoinRepo _repo;
        public DataContext Context { get; set; }
        private readonly IUsersService usersService;

        public HomeController( DataContext context, IUsersService usersService, ICoinRepo repo) 
        {
            Context = context;
            this.usersService = usersService;
            _repo = repo;
        }

        [Route("Login/{username}/{password}")]
        [HttpPost]
        public async Task<IActionResult> LoginCheck(string username, string password)
        {
            string usernamee = username;
            string passwordd = password;

            bool userExists = usersService.authUser(usernamee, passwordd);
            if (userExists)
            {
                var claims = new List<Claim>
                {
                    new Claim("username", username),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.IsPersistent, "True"),
                    new Claim("session", Guid.NewGuid().ToString())
                };

                var ci = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var cp = new ClaimsPrincipal(ci);
                await HttpContext.SignInAsync(cp);
                return Redirect("/Index");
            }
            else 
            {
                return BadRequest("Error");
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            _repo.unsubrscribeAll();
            await HttpContext.SignOutAsync();
            return Redirect("/Index");
        }

        [Route("Register/{username}/{password}")]
        [HttpPost]
        public ActionResult Register(string username, string password)
        {
            try
            {
                if (usersService.isUserAlreadyRegistered(username))
                {
                    return BadRequest("Korisnik je vec registrovan!");
                }

                if (usersService.addUser(username, password))
                {
                    return Ok("Uspesna registracija!");
                }

                return BadRequest("Doslo je do neocekivane greske!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
