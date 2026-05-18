using MacroPlan.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MacroPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProduitsController : ControllerBase
    {
        private static List<Produit> _produits = new List<Produit>
        {
            new Produit { Id = 1, Nom = "Chicken Bowl", Prix = 8.50m, Calories = 450, Proteines = 42, Glucides = 35, Lipides = 8 },
            new Produit { Id = 2, Nom = "Beef & Rice", Prix = 9.00m, Calories = 520, Proteines = 48, Glucides = 40, Lipides = 10 }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Produit>> GetAll()
        {
            return Ok(_produits);
        }

        [HttpGet("{id}")]
        public ActionResult<Produit> GetById(int id)
        {
            var produit = _produits.FirstOrDefault(p => p.Id == id);
            if (produit == null)
                return NotFound();

            return Ok(produit);
        }
        [HttpPost]
        public ActionResult<Produit> Create(Produit produit)
        {
            produit.Id = _produits.Max(p => p.Id) + 1;
            _produits.Add(produit);
            return CreatedAtAction(nameof(GetById), new { id = produit.Id }, produit);
        }

        [HttpPut("{id}")]
        public ActionResult<Produit> Update(int id, Produit produit)
        {
            var existing = _produits.FirstOrDefault(p => p.Id == id);
            if (existing == null)
                return NotFound();

            existing.Nom = produit.Nom;
            existing.Prix = produit.Prix;
            existing.Calories = produit.Calories;
            existing.Proteines = produit.Proteines;
            existing.Glucides = produit.Glucides;
            existing.Lipides = produit.Lipides;

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var produit = _produits.FirstOrDefault(p => p.Id == id);
            if (produit == null)
                return NotFound();

            _produits.Remove(produit);
            return NoContent();
        }
    }

}