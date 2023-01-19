using Microsoft.EntityFrameworkCore;

namespace Models 
{
    public class DataContext : DbContext 
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CoinHistorySql> Coins { get; set; }
        public DataContext(DbContextOptions options) : base(options){}
    }
}
