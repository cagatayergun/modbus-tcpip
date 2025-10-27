// Dosya: TekstilScada.WebAPI/Controllers/RecipesController.cs
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TekstilScada.Models;
using TekstilScada.Repositories;
using TekstilScada.Services; // PlcPollingService için eklendi

namespace TekstilScada.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class json : ControllerBase
    {

        private readonly RecipeConfigurationRepository _configRepo;
        public json(RecipeConfigurationRepository configRepo)
        {
            
            _configRepo = configRepo;
        }

        [HttpGet("config/layout/{machineSubType}/{stepId}")]
        public IActionResult GetStepLayout(string machineSubType, int stepId)
        {
            try
            {
                // Sizin sağladığınız senkron metodu çağırıyoruz
                string layoutJson = _configRepo.GetLayoutJson(machineSubType, stepId);

                if (string.IsNullOrEmpty(layoutJson))
                {
                    return NotFound(new { Message = $"Layout not found for {machineSubType} / {stepId}" });
                }

                // JSON string'ini doğrudan content olarak dönüyoruz.
                return Content(layoutJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // --- YENİ METOTLAR (RecipeStepDesigner için) ---
        // Repository'nizdeki diğer metotları da API'ye açabilirsiniz.

        [HttpGet("config/steptypes")]
        public IActionResult GetStepTypes()
        {
            try
            {
                var dt = _configRepo.GetStepTypes();

                // DataTable'ı JSON'a çevir (Basit bir örnek)
                var stepTypes = dt.AsEnumerable().Select(row => new
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("StepName")
                    // Veritabanı kolon adınız 'StepName' ise
                }).ToList();

                return Ok(stepTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("config/machinesubtypes")]
        public IActionResult GetMachineSubTypes()
        {
            try
            {
                return Ok(_configRepo.GetMachineSubTypes());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Layout kaydetmek için bir DTO (Data Transfer Object)
        public class SaveLayoutRequest
        {
            public string LayoutName { get; set; }
            public string MachineSubType { get; set; }
            public int StepTypeId { get; set; }
            public string LayoutJson { get; set; }
        }

        [HttpPost("config/savelayout")]
        public IActionResult SaveLayout([FromBody] SaveLayoutRequest request)
        {
            try
            {
                _configRepo.SaveLayout(request.LayoutName, request.MachineSubType, request.StepTypeId, request.LayoutJson);
                return Ok(new { Message = "Layout saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}