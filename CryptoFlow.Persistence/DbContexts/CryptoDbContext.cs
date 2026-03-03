using CryptoFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Persistence.DbContexts;

public class CryptoDbContext: IdentityDbContext<User, Role, Guid>
{
    public CryptoDbContext(DbContextOptions<CryptoDbContext> options):  base(options) {}
    
    public DbSet<Coin> Coins { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletItem> WalletItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Eğer IdentityDbContext kullanıyorsan base metodunu çağırmayı unutma
        base.OnModelCreating(modelBuilder); 

        // User - Wallet Bire-Bir (1-1) İlişkisi
        modelBuilder.Entity<User>()
            .HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<Wallet>(w => w.UserId); // Yabancı anahtarın Wallet içinde olduğunu belirtiyoruz
    }
    
}