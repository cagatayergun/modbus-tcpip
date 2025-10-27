// Dosya: TekstilScada.WebAPI/Controllers/FtpController.cs (Güncellenmiş)

using Microsoft.AspNetCore.Mvc;
using TekstilScada.Core;
using TekstilScada.Models; // ScadaRecipe, Machine gibi modeller buradaysa
using TekstilScada.Repositories;
using TekstilScada.Services;
using System.Collections.Generic;

namespace TekstilScada.WebAPI.Controllers
{
    // DTO'lar (Request Body Modelleri - API projenizde tanımlanmalıdır)
    public class QueueSendJobRequest
    {
        public List<int> RecipeIds { get; set; } = new List<int>();
        public List<int> MachineIds { get; set; } = new List<int>();
        public int StartNumber { get; set; }
    }

    public class QueueReceiveJobRequest
    {
        public List<string> FileNames { get; set; } = new List<string>();
        public int MachineId { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class FtpController : ControllerBase
    {
        private readonly MachineRepository _machineRepository;
        private readonly RecipeRepository _recipeRepository;
        private readonly FtpTransferService _transferService;
        private readonly PlcPollingService _plcPollingService;

        public FtpController(
            MachineRepository machineRepository,
            RecipeRepository recipeRepository,
            FtpTransferService transferService,
            PlcPollingService plcPollingService)
        {
            _machineRepository = machineRepository;
            _recipeRepository = recipeRepository;
            _transferService = transferService;
            _plcPollingService = plcPollingService;
        }

        // 1. ENDPOINT: HMI/PLC'deki reçete isimlerini okur (Blazor: GetHmiRecipeNamesAsync)
        [HttpGet("hmi-recipe-names/{machineId}")]
        public async Task<ActionResult<Dictionary<int, string>>> GetHmiRecipeNames(int machineId)
        {
            var machine = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == machineId);
            if (machine == null) return NotFound("Makine bulunamadı.");

            try
            {
                if (!_plcPollingService.GetPlcManagers().TryGetValue(machine.Id, out var plcManager))
                {
                    return StatusCode(500, "Makine için aktif PLC yöneticisi bulunamadı.");
                }

                // PLC'den Slot -> Name eşleşmesini okur (WinForms mantığı)
                var readResult = await plcManager.ReadRecipeNamesFromPlcAsync();

                if (readResult.IsSuccess)
                {
                    // Dictionary<int, string> { SlotNumber: RecipeName } döndürür
                    return Ok(readResult.Content);
                }
                else
                {
                    return StatusCode(500, $"PLC'den reçete isimleri okunamadı: {readResult.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"HMI reçeteleri okunurken beklenmedik hata: {ex.Message}");
            }
        }

        // 2. ENDPOINT: Seçili HMI reçetesinin içeriğini indirip önizleme için döndürür (Blazor: GetHmiRecipePreviewAsync)
        [HttpGet("hmi-recipe-preview/{machineId}")]
        public async Task<ActionResult<ScadaRecipe>> GetHmiRecipePreview(int machineId, [FromQuery] string fileName)
        {
            var machine = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == machineId);
            if (machine == null) return NotFound("Makine bulunamadı.");
            if (string.IsNullOrEmpty(fileName)) return BadRequest("Dosya adı belirtilmedi.");

            try
            {
                var ftpService = new FtpService(machine.IpAddress, machine.FtpUsername, machine.FtpPassword);
                string csvContent = await ftpService.DownloadFileAsync($"/{fileName}");

                // CSV içeriğini ScadaRecipe nesnesine çevirir (WinForms mantığı)
                var previewRecipe = RecipeCsvConverter.ToRecipe(csvContent, fileName);

                return Ok(previewRecipe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Reçete ön izleme yüklenemedi: {ex.Message}");
            }
        }

        // 3. ENDPOINT: Gönderim işlerini kuyruğa alır (Blazor: QueueSequentiallyNamedSendJobsAsync)
        [HttpPost("queue-send-jobs")]
        public ActionResult QueueSequentiallyNamedSendJobs([FromBody] QueueSendJobRequest request)
        {
            if (!request.RecipeIds.Any() || !request.MachineIds.Any())
            {
                return BadRequest("En az bir reçete ve bir makine ID'si belirtilmeli.");
            }

            try
            {
                // ÇÖZÜM: Yeni metot çağırmak yerine, mevcut GetAll metotlarını ve LINQ'u kullanıyoruz.
                // 1. Reçeteleri al ve filtrele
                var allRecipes = _recipeRepository.GetAllRecipes();
                var recipes = allRecipes.Where(r => request.RecipeIds.Contains(r.Id)).ToList();

                // 2. Makineleri al ve filtrele
                var allMachines = _machineRepository.GetAllMachines(); // GetAllMachines kullanıldı
                var machines = allMachines.Where(m => request.MachineIds.Contains(m.Id)).ToList();

                if (recipes.Count != request.RecipeIds.Count || machines.Count != request.MachineIds.Count)
                {
                    return NotFound("Belirtilen reçetelerden veya makinelerden bazıları bulunamadı.");
                }

                // Gönderim işlerini FtpTransferService'e kuyruğa alır
                _transferService.QueueSequentiallyNamedSendJobs(recipes, machines, request.StartNumber);

                return Ok(new { Message = "Gönderim işleri başarıyla kuyruğa alındı." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gönderim işleri kuyruğa alınırken hata: {ex.Message}");
            }
        }

        // 4. ENDPOINT: Alma işlerini kuyruğa alır (Blazor: QueueReceiveJobsAsync)
        [HttpPost("queue-receive-jobs")]
        public ActionResult QueueReceiveJobs([FromBody] QueueReceiveJobRequest request)
        {
            if (!request.FileNames.Any() || request.MachineId == 0)
            {
                return BadRequest("En az bir dosya adı ve makine ID'si belirtilmeli.");
            }

            try
            {
                var machine = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == request.MachineId);
                if (machine == null) return NotFound("Makine bulunamadı.");

                // Alma işlerini FtpTransferService'e kuyruğa alır (WinForms mantığı)
                _transferService.QueueReceiveJobs(request.FileNames, machine);

                return Ok(new { Message = "Alma işleri başarıyla kuyruğa alındı." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Alma işleri kuyruğa alınırken hata: {ex.Message}");
            }
        }
    }
}