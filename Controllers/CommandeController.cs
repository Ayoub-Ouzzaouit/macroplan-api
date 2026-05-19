using MacroPlan.API.Data;
using MacroPlan.API.Models;
using MacroPlan.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MacroPlan.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommandeController : ControllerBase
    {
        private readonly MacroPlanContext _context;
        private readonly NutritionService _nutritionService;

        public CommandeController(MacroPlanContext context, NutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        [HttpPost]
        public async Task<ActionResult> PasserCommande(CommandeDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var commande = new Commande
            {
                UserId = userId!,
                AdresseLivraison = dto.AdresseLivraison,
                NombreLivraisonsParSemaine = dto.NombreLivraisonsParSemaine,
                CommandeProduits = dto.CommandeProduits.Select(cp => new CommandeProduit
                {
                    ProduitId = cp.ProduitId,
                    Quantite = cp.Quantite,
                    DoubleViande = cp.DoubleViande
                }).ToList()
            };

            var macrosTotales = await CalculerMacrosCommande(commande);

            var profil = await _context.ProfilsNutritionnels
                .FirstOrDefaultAsync(p => p.UserId == userId);

            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            var response = new
            {
                CommandeId = commande.Id,
                MacrosTotales = macrosTotales,
                Ecart = profil != null ? new
                {
                    Calories = macrosTotales.Calories - _nutritionService.CalculerBesoins(profil).CaloriesCibles,
                    Proteines = macrosTotales.ProteinesG - _nutritionService.CalculerBesoins(profil).ProteinesG,
                    Glucides = macrosTotales.GlucidesG - _nutritionService.CalculerBesoins(profil).GlucidesG,
                    Lipides = macrosTotales.LipidesG - _nutritionService.CalculerBesoins(profil).LipidesG,
                } : null
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetMesCommandes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var commandes = await _context.Commandes
                .Include(c => c.CommandeProduits)
                .ThenInclude(cp => cp.Produit)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(commandes);
        }

        private async Task<dynamic> CalculerMacrosCommande(Commande commande)
        {
            int calories = 0, proteines = 0, glucides = 0, lipides = 0;

            foreach (var cp in commande.CommandeProduits)
            {
                var produit = await _context.Produits.FindAsync(cp.ProduitId);
                if (produit == null) continue;

                double multiplicateur = cp.DoubleViande ? 1.5 : 1.0;
                double quantite = cp.Quantite * multiplicateur;

                calories += (int)(produit.Calories * quantite);
                proteines += (int)(produit.Proteines * quantite);
                glucides += (int)(produit.Glucides * quantite);
                lipides += (int)(produit.Lipides * quantite);
            }

            return new { Calories = calories, ProteinesG = proteines, GlucidesG = glucides, LipidesG = lipides };
        }
    }
}