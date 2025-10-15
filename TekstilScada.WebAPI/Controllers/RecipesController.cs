// Dosya: TekstilScada.WebAPI/Controllers/RecipesController.cs
using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Repositories;
using TekstilScada.Services; // PlcPollingService için eklendi

namespace TekstilScada.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeRepository _recipeRepository;
        private readonly PlcPollingService _pollingService; // PLC yöneticilerine erişim için eklendi

        public RecipesController(RecipeRepository recipeRepository, PlcPollingService pollingService)
        {
            _recipeRepository = recipeRepository;
            _pollingService = pollingService;
        }

        // ... GetAllRecipes, GetRecipeById, SaveRecipe, DeleteRecipe metotları aynı kalacak ...
        [HttpGet]
        public ActionResult<IEnumerable<ScadaRecipe>> GetAllRecipes()
        {
            return Ok(_recipeRepository.GetAllRecipes());
        }

        [HttpGet("{id}")]
        public ActionResult<ScadaRecipe> GetRecipeById(int id)
        {
            var recipe = _recipeRepository.GetRecipeById(id);
            if (recipe == null) return NotFound();
            return Ok(recipe);
        }

        [HttpPost]
        public ActionResult<ScadaRecipe> SaveRecipe([FromBody] ScadaRecipe recipe)
        {
            _recipeRepository.SaveRecipe(recipe);
            return Ok(recipe);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            _recipeRepository.DeleteRecipe(id);
            return Ok();
        }


        // === YENİ METOT: Reçeteyi PLC'ye Gönderme ===
        [HttpPost("{recipeId}/send-to-plc/{machineId}")]
        public async Task<IActionResult> SendRecipeToPlc(int recipeId, int machineId)
        {
            var recipe = _recipeRepository.GetRecipeById(recipeId);
            if (recipe == null) return NotFound("Reçete bulunamadı.");

            var plcManagers = _pollingService.GetPlcManagers();
            if (!plcManagers.TryGetValue(machineId, out var plcManager))
            {
                return BadRequest("Hedef makine için PLC yöneticisi bulunamadı.");
            }

            var result = await plcManager.WriteRecipeToPlcAsync(recipe);
            if (!result.IsSuccess)
            {
                return StatusCode(500, result.Message);
            }
            return Ok();
        }

        // === YENİ METOT: PLC'den Reçete Okuma ===
        [HttpGet("read-from-plc/{machineId}")]
        public async Task<ActionResult<ScadaRecipe>> ReadRecipeFromPlc(int machineId)
        {
            var plcManagers = _pollingService.GetPlcManagers();
            if (!plcManagers.TryGetValue(machineId, out var plcManager))
            {
                return BadRequest("Hedef makine için PLC yöneticisi bulunamadı.");
            }

            var result = await plcManager.ReadRecipeFromPlcAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(500, result.Message);
            }

            // PLC'den gelen ham veriyi ScadaRecipe formatına dönüştür
            var recipeFromPlc = new ScadaRecipe { RecipeName = $"PLC_OKUNAN_{DateTime.Now:HHmm}" };
            for (int i = 0; i < result.Content.Length / 25; i++)
            {
                var step = new ScadaRecipeStep { StepNumber = i + 1 };
                Array.Copy(result.Content, i * 25, step.StepDataWords, 0, 25);
                recipeFromPlc.Steps.Add(step);
            }

            return Ok(recipeFromPlc);
        }
        // YENİ METOT: Reçete Kullanım Geçmişini ve Tüketimi Getirme
        [HttpGet("{recipeId}/usage-history")]
        public ActionResult<IEnumerable<ProductionReportItem>> GetRecipeUsageHistory(int recipeId)
        {
            try
            {
                // RecipeRepository'deki mevcut metot zaten consumption verilerini döndürür.
                var history = _recipeRepository.GetRecipeUsageHistory(recipeId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Reçete kullanım geçmişi alınırken bir hata oluştu: {ex.Message}");
            }
        }
    }
}