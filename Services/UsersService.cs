using Models;
using System.Web.Helpers;

namespace Services 
{
    public interface IUsersService 
    {
        bool isUserAlreadyRegistered(string username);
        bool authUser(string username, string password);
        bool addUser(string username, string password);
        void SubscribeCoin(string coinChannel, string username);
        string[] GetSubscribedCoins(string? username);
    }

    public class UsersService : IUsersService 
    {
        public DataContext Context { get; set; }
        public UsersService(DataContext context) 
        {
            Context = context;
        }

        public bool isUserAlreadyRegistered(string username) 
        {
            var user = Context.Users.Where(p => p.Username == username).FirstOrDefault();

            if(user == null)
            {
                return false;
            }

            return true;
        }
       
        public bool authUser(string username, string password) 
        {
            var user = Context.Users.Where(p => p.Username == username).FirstOrDefault();

            if(user != null) 
            {
                if(Crypto.VerifyHashedPassword(user.Password, password) == false) 
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public bool addUser(string username, string password) 
        {
            User user = new User
            {
                Username = username,
                Password =  Crypto.HashPassword(password),
            };
            
            Context.Users.Add(user);
            Context.SaveChanges();

            return true;
        }

        public void SubscribeCoin(string coinChannel, string username)
        {
            var user = Context.Users.Where(p => p.Username == username).FirstOrDefault();
            if (user != null)
            {
                var coins = Context.Users.Where(p => p.Username == username).Select(p => p.SubscribedCoins).FirstOrDefault();
                if (coins == null)
                {
                    user.SubscribedCoins = coinChannel;
                    Context.Users.Update(user);

                }
                else if (!coins.Contains(coinChannel))
                {
                    user.SubscribedCoins += $";{coinChannel}";
                    Context.Users.Update(user);
                }
            }
            Context.SaveChanges();
        }

        public string[] GetSubscribedCoins(string? username)
        {
            var user = Context.Users.Where(p => p.Username == username).FirstOrDefault();
            if (user != null)
            {
                var coins = Context.Users.Where(p => p.Username == username).Select(p => p.SubscribedCoins).FirstOrDefault();
                if (coins != null)
                {
                    return coins.Split(";");
                }
            }

            return new string[] { };
        }
    }
}