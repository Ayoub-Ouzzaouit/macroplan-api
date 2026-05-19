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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produit>>> GetAll()
        {
            return Ok(await _context.Produits.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Produit>> GetById(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
                return NotFound();

            return Ok(produit);
        }

        [HttpPost]
        public async Task<ActionResult<Produit>> Create(Produit produit)
        {
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = produit.Id }, produit);
        }

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

            // Calculer un score de pertinence pour chaque produit
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
            // Score basé sur le ratio protéines/calories
            double ratioProteine = (double)produit.Proteines / produit.Calories;
            double cibleRatio = (double)cible.ProteinesG / cible.CaloriesCibles;

            // Plus le ratio est proche de la cible, plus le score est élevé
            double score = 100 - (Math.Abs(ratioProteine - cibleRatio) * 1000);
            return Math.Max(0, score);
        }
    }


}

