using MacroPlan.API.Data;
using MacroPlan.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MacroPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProduitsController : ControllerBase
    {
        private readonly MacroPlanContext _context;

        public ProduitsController(MacroPlanContext context)
        {
            _context = context;
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
    }
}