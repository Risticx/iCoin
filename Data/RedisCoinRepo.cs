using iCoin.Models;
using Microsoft.AspNetCore.SignalR;
using Models;
using Newtonsoft.Json;
using SignalRChat.Hubs;
using StackExchange.Redis;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace iCoin.Data
{
    public interface ICoinRepo
    {
        void unsubrscribeAll();
        void getNotification(string coins);
        bool dumpHistories();
        CoinHistory getCoinsHistory(string coin);
        Task CreateCoin();
        IEnumerable<Coin?>? GetAllCoins();
        IEnumerable<Coin?>? GetSpecificCoins(string[] SubscribedCoins);
    }

    public class RedisCoinRepo : ICoinRepo
    {
        private static bool subscribed = false;
        private readonly IHubContext<CoinHub> _hubContext;
        private IConnectionMultiplexer _redis;
        public DataContext Context { get; set; }

        public RedisCoinRepo(IConnectionMultiplexer redis, DataContext context, IHubContext<CoinHub> hubContext)
        {
            _redis = redis;
            Context = context;
            _hubContext = hubContext;
        }

        public async Task CreateCoin()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://api.coingecko.com/api/v3/coins/");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var coinsData = JsonConvert.DeserializeObject<dynamic[]>(json);

                foreach (var coinData in coinsData ?? new dynamic[0])
                {
                    var name = coinData.name;
                    var symbol = coinData.symbol;
                    var price = coinData.market_data.current_price.usd;
                    var image = coinData.image.thumb;
                    var db = _redis.GetDatabase();

                    DateTime dt = DateTime.Now;
                    string time = dt.ToString("yyyy-MM-dd hh:mm:ss");

                    db.ListRightPush("priceHistory:" + symbol, $"time:{time};price:{price}");

                    if (db.HashExists("coin:" + symbol, "name"))
                    {
                        _redis.GetSubscriber().Publish("coin:" + symbol, $"Nova cena {(string)symbol}a je {price}");

                        db.HashSet("coin:" + symbol, new HashEntry[] { new HashEntry("price", $"{price}") });
                    }
                    else
                    {
                        db.HashSet("coin:" + symbol, new HashEntry[] { new HashEntry("image", $"{image}") });
                        db.HashSet("coin:" + symbol, new HashEntry[] { new HashEntry("name", $"{name}"), new HashEntry("symbol", $"{symbol}"), new HashEntry("price", $"{price}") });
                    }
                }
            }
        }

        public IEnumerable<Coin?>? GetSpecificCoins(string[] SubscribedCoins)
        {
            var db = _redis.GetDatabase();
            List<Coin> list = new List<Coin>();

            foreach (var key in SubscribedCoins)
            {
                Coin coin = new Coin();
                coin.Id = key;
                coin.Name = db.HashGet("coin:" + key, "name");
                coin.Symbol = db.HashGet("coin:" + key, "symbol");
                coin.Image = db.HashGet("coin:" + key, "image");
                decimal.TryParse(db.HashGet("coin:" + key, "price").ToString(), out decimal a);
                coin.Price = a;
                list.Add(coin);
            }
            IEnumerable<Coin> coins = list;

            return coins;
        }

        public IEnumerable<Coin?>? GetAllCoins()
        {
            var db = _redis.GetDatabase();
            EndPoint endPoint = _redis.GetEndPoints().First();
            RedisKey[] keys = _redis.GetServer(endPoint).Keys(pattern: "coin:*").ToArray();
            List<Coin> list = new List<Coin>();

            foreach (var key in keys)
            {
                Coin coin = new Coin();
                coin.Id = key;
                coin.Name = db.HashGet(key, "name");
                coin.Symbol = db.HashGet(key, "symbol");
                coin.Image = db.HashGet(key, "image");
                decimal.TryParse(db.HashGet(key, "price").ToString(), out decimal a);
                coin.Price = a;
                list.Add(coin);
            }
            IEnumerable<Coin> coins = list;

            return coins;
        }

        public bool dumpHistories()
        {
            var db = _redis.GetDatabase();
            EndPoint endPoint = _redis.GetEndPoints().First();
            RedisKey[] keys = _redis.GetServer(endPoint).Keys(pattern: "priceHistory:*").ToArray();

            foreach (var key in keys)
            {
                var ll = db.ListRange(key, 0, 288);
                foreach (var l in ll)
                {
                    CoinHistorySql sqlCoin = new CoinHistorySql();
                    sqlCoin.Name = key;

                    var temp = l.ToString().Split(";");
                    var dt = temp[0].Replace("time:", "");
                    DateTime newDt = new DateTime();
                    DateTime.TryParse(dt, out newDt);
                    decimal.TryParse(temp[1].Replace("price:", ""), out decimal price);
                    sqlCoin.DateAndTime = newDt;
                    sqlCoin.Price = price;

                    Context.Coins.Add(sqlCoin);
                }
                Context.SaveChanges();
            }
            _redis.GetServer(endPoint).FlushDatabase();

            return true;
        }

        public void unsubrscribeAll()
        {
           _redis.GetSubscriber().UnsubscribeAll();
        }

        public void getNotification(string coins)
        {
            if (!subscribed)
            {
                var values = coins.Split(";");
                Debug.WriteLine(values.First());

                var pubsub = _redis.GetSubscriber();

                foreach (var value in values)
                {
                    CoinHub c = new CoinHub();
                    pubsub.Subscribe("coin:" + value, (channel, message) =>
                    {
                        _hubContext.Clients.All.SendAsync("ReceiveMessage", message.ToString());
                        Debug.WriteLine(message);
                    });
                }
                subscribed = true;
            }
        }

        public CoinHistory getCoinsHistory(string coin)
        {
            var db = _redis.GetDatabase();
            CoinHistory coinHistory = new CoinHistory();
            coinHistory.Id = "priceHistory:" + coin;
            var ll = db.ListRange(coinHistory.Id, db.ListLength(coinHistory.Id)-12, db.ListLength(coinHistory.Id));

            foreach (var l in ll)
            {
               coinHistory.PriceAndDateTime.Add(l.ToString());
            }

            return coinHistory;
        }  
    }
}
