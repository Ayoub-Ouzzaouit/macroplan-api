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
    public class ProduitsController : ControllerBase
    {
        private readonly MacroPlanContext _context;
        private readonly NutritionService _nutritionService;

        public ProduitsController(MacroPlanContext context, NutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        /// <summary>Retourne tous les produits du catalogue</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produit>>> GetAll()
        {
            return Ok(await _context.Produits.ToListAsync());
        }

        /// <summary>Retourne un produit par son ID</summary>
        /// <param name="id">Identifiant du produit</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Produit>> GetById(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
                return NotFound();

            return Ok(produit);
        }

        /// <summary>Crée un nouveau produit dans le catalogue</summary>
        [HttpPost]
        public async Task<ActionResult<Produit>> Create(Produit produit)
        {
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = produit.Id }, produit);
        }

        /// <summary>Met à jour un produit existant</summary>
        /// <param name="id">Identifiant du produit à modifier</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<Produit>> Update(int id, Produit produit)
        {
            var existing = await _context.Produits.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Nom = produit.Nom;
            existing.Prix = produit.Prix;
            existing.Calories = produit.Calories;
            existing.Proteines = produit.Proteines;
            existing.Glucides = produit.Glucides;
            existing.Lipides = produit.Lipides;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        /// <summary>Supprime un produit du catalogue</summary>
        /// <param name="id">Identifiant du produit à supprimer</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
                return NotFound();

            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Retourne les 5 meilleurs repas selon le profil nutritionnel de l'utilisateur connecté.
        /// Score calculé sur le ratio protéines/calories par rapport à la cible TDEE.
        /// </summary>
        [HttpGet("suggestions")]
        public async Task<ActionResult> GetSuggestions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profil = await _context.ProfilsNutritionnels
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profil == null)
                return BadRequest("Créez d'abord votre profil nutritionnel");

            var resultat = _nutritionService.CalculerBesoins(profil);
            var produits = await _context.Produits.ToListAsync();

            var suggestions = produits.Select(p => new
            {
                Produit = p,
                Score = CalculerScore(p, resultat),
                CaloriesParRepas = resultat.CaloriesCibles / 3,
                EcartCalories = Math.Abs(p.Calories - (resultat.CaloriesCibles / 3))
            })
            .OrderByDescending(s => s.Score)
            .Take(5)
            .ToList();

            return Ok(new
            {
                ProfilCible = resultat,
                Suggestions = suggestions
            });
        }

        private double CalculerScore(Produit produit, ResultatNutritionnel cible)
        {
            double ratioProteine = (double)produit.Proteines / produit.Calories;
            double cibleRatio = (double)cible.ProteinesG / cible.CaloriesCibles;
            double score = 100 - (Math.Abs(ratioProteine - cibleRatio) * 1000);
            return Math.Max(0, score);
        }
    }
}