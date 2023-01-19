using iCoin.Data;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace iCoin_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ICoinRepo _repo;
        private readonly IUsersService usersService;
        
        public UserController(ICoinRepo repo, IUsersService users)
        {
            _repo = repo;
            usersService = users;
        }

        [Route("subscribeCoin/{coinChannel}")]
        [HttpPost]
        public ActionResult subsrcibeCoin(string coinChannel)
        {
            if(User?.Identity?.IsAuthenticated == true && User.Identity.Name != null) 
            {
                var username = User.Identity.Name;
                usersService.SubscribeCoin(coinChannel, username);

                return Ok("Ok");
            }

            return BadRequest("Niste Ulogovani");
        }

        [Route("isLoggedIn")]
        [HttpGet]
        public ActionResult isLoggedIn()
        {
            if (User?.Identity?.IsAuthenticated == true) 
                return Ok("Ok");

            return BadRequest("Niste Ulogovani");
        }

        [Route("getSubscribedCoins")]
        [HttpGet]
        public ActionResult getSubscribedCoins()
        {

            if (User?.Identity?.IsAuthenticated == true && User.Identity.Name != null)
            {
                var username = User.Identity.Name;
                if (usersService.GetSubscribedCoins(username) != null)
                    return Ok(usersService.GetSubscribedCoins(username));

                return BadRequest("Niste Ulogovani");
            }
            else
            {
                if (User?.Identity?.Name != null)
                {
                    var username = User.Identity.Name;
                    return Ok(usersService.GetSubscribedCoins(username));
                }

                return BadRequest("Error");
            }
        }

        [Route("getSpecificCoins")]
        [HttpGet]
        public ActionResult getSpecificCoins()
        {
            var specificCoins = usersService.GetSubscribedCoins(User?.Identity?.Name);

            return Ok(_repo.GetSpecificCoins(specificCoins));
        }

        [Route("getNotification/{coins}")]
        [HttpGet]
        public ActionResult getNotification(string coins)
        {
            _repo.getNotification(coins);
            
            return Ok("ok");
        }
    }
}
