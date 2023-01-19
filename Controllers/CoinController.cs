using iCoin.Data;
using iCoin.Models;
using Microsoft.AspNetCore.Mvc;

namespace iCoin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinController : ControllerBase
    {
        private readonly ICoinRepo _repo;

        public CoinController(ICoinRepo repo)
        {
            _repo = repo;
        }

        [Route("addCoin")]
        [HttpPost]
        public ActionResult<Coin> addCoin()
        {
            _repo.CreateCoin();

            return Ok("OK");
        }

        [Route("getAllCoins")]
        [HttpGet]
        public ActionResult getAllCoins()
        {
            return Ok(_repo.GetAllCoins());
        }

        [Route("getCoinHistory/{coin}")]
        [HttpGet]
        public ActionResult getCoinsHistory(string coin)
        {
            return Ok(_repo.getCoinsHistory(coin));
        }

        [Route("dumpHistory")]
        [HttpGet]
        public ActionResult dumpHistory()
        {
            return Ok(_repo.dumpHistories());
        }

    }
}