using Microsoft.EntityFrameworkCore;
using BackendNETAPI.Model;

namespace BackendNETAPI.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Products> Products{ get; set; }
        
        public DbSet<ProductsOrder> ProductsOrder { get; set; }
        public DbSet<UserAccessrights> UserAccessrights { get; set; }
        public DbSet<Modules> Modules { get; set; }
        public DbSet<SubModule> SubModules { get    ; set; }

        public DbSet<AccessRightSubModule> AccessRightSubModules {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id); // Configuring the primary key

            modelBuilder.Entity<Products>()
             .HasKey(e => e.Id);

            modelBuilder.Entity<ProductsOrder>()
           .HasKey(e => e.OrderId);

            modelBuilder.Entity<UserAccessrights>()
           .HasKey(b => b.Id);

            modelBuilder.Entity<Modules>()
           .HasKey(b => b.Id);

            modelBuilder.Entity<SubModule>()
           .HasKey(b => b.Id); 

            modelBuilder.Entity<AccessRightSubModule>()
           .HasKey(b => b.Id);

            base.OnModelCreating(modelBuilder);
        }

    }
}
