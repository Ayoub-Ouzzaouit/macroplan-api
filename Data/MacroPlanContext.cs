using MacroPlan.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MacroPlan.API.Data
{
    public class MacroPlanContext : DbContext
    {
        public MacroPlanContext(DbContextOptions<MacroPlanContext> options) : base(options)
        {
        }

        public DbSet<Produit> Produits { get; set; }
    }
}