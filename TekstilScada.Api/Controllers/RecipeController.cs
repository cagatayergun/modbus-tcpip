using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Repositories;
using Microsoft.AspNetCore.Authorization; // Bu satırı ekle
namespace TekstilScada.Api.Controllers
{
    //[Authorize] // Bu etiketi ekle
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeRepository _recipeRepository;
        private readonly MachineRepository _machineRepository;

        public RecipeController(RecipeRepository recipeRepository, MachineRepository machineRepository)
        {
            _recipeRepository = recipeRepository;
            _machineRepository = machineRepository;
        }

        // Tüm reçeteleri listeler
        [HttpGet]
        public IActionResult GetAllRecipes()
        {
            var recipes = _recipeRepository.GetAllRecipes();
            return Ok(recipes);
        }

        // Belirli bir reçeteyi ID'sine göre getirir
        [HttpGet("{id}")]
        public IActionResult GetRecipeById(int id)
        {
            var recipe = _recipeRepository.GetRecipeById(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        // Yeni bir reçete kaydeder veya mevcut bir reçeteyi günceller
        [HttpPost]
        public IActionResult SaveRecipe([FromBody] ScadaRecipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _recipeRepository.SaveRecipe(recipe);
                return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Reçete kaydedilirken bir hata oluştu: {ex.Message}");
            }
        }

        // Bir reçeteyi siler
        [HttpDelete("{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            try
            {
                var existingRecipe = _recipeRepository.GetRecipeById(id);
                if (existingRecipe == null)
                {
                    return NotFound();
                }
                _recipeRepository.DeleteRecipe(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Reçete silinirken bir hata oluştu: {ex.Message}");
            }
        }

        // Tüm makine tiplerini listeler (UI'daki gibi)
        [HttpGet("machine-types")]
        public IActionResult GetMachineTypes()
        {
            var machineTypes = _machineRepository.GetAllMachines()
                .Select(m => !string.IsNullOrEmpty(m.MachineSubType) ? m.MachineSubType : m.MachineType)
                .Distinct()
                .ToList();
            return Ok(machineTypes);
        }
    }
}