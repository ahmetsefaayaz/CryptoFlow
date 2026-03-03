using Microsoft.AspNetCore.Identity;

namespace CryptoFlow.Domain.Entities;

public class User: IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Wallet Wallet { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
}