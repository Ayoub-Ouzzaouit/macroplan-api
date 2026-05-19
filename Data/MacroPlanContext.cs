using MacroPlan.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MacroPlan.API.Data
{
    public class MacroPlanContext : IdentityDbContext<IdentityUser>
    {
        public MacroPlanContext(DbContextOptions<MacroPlanContext> options) : base(options)
        {
        }

        public DbSet<Produit> Produits { get; set; }
    }
}