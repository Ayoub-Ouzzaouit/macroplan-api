using MacroPlan.API.Data;
using MacroPlan.API.Models;
using MacroPlan.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MacroPlan.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilController : ControllerBase
    {
        private readonly MacroPlanContext _context;
        private readonly NutritionService _nutritionService;

        public ProfilController(MacroPlanContext context, NutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        [HttpPost]
        public async Task<ActionResult<ResultatNutritionnel>> CreerProfil(ProfilNutritionnel profil)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            profil.UserId = userId!;

            _context.ProfilsNutritionnels.Add(profil);
            await _context.SaveChangesAsync();

            var resultat = _nutritionService.CalculerBesoins(profil);
            return Ok(resultat);
        }
    }
}