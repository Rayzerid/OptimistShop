using Microsoft.EntityFrameworkCore;
using OptimistShop.Models.DbTables;

namespace OptimistShop.Core
{
    public class StoreDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                //host
                optionsBuilder.UseSqlServer(@$"Server=217.28.223.127,17160;User Id=user_5d0ce;Password=Xy6=i$F92w?T;Database=db_e828a;TrustServerCertificate=True");
            }
            catch
            {
                //local
                string hostname = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
                optionsBuilder.UseSqlServer(@$"Data Source={hostname}\SQLEXPRESS;Initial Catalog=optimist_store_ordering_system_db;Integrated security=True;TrustServerCertificate=True");
            }
        }
        public DbSet<Clothes> Clothes { get; set; }
        public DbSet<ClothesContain> ClothesContain { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderContain> OrderContain { get; set; }
    }
}
