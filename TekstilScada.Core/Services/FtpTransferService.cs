// ======================================================
// FILE: TekstilScada.Core/Services/FtpTransferService.cs
// TransferJob sınıfına yeni özellik eklendi ve kuyruk işleme mantığı güncellendi.
// ======================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TekstilScada.Core;
using TekstilScada.Models;
using TekstilScada.Repositories;

namespace TekstilScada.Services
{
    public enum TransferType { Gonder, Al }
    public enum TransferStatus { Beklemede, Aktarılıyor, Başarılı, Hatalı }

    public class TransferJob : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string HedefDosyaAdi { get; set; }

        // YENİ EKLENEN ÖZELLİK
        public int RecipeNumber { get; set; }

        public string ReceteAdi => IslemTipi == TransferType.Gonder
                                   ? (!string.IsNullOrEmpty(HedefDosyaAdi) ? $"{YerelRecete?.RecipeName} -> {HedefDosyaAdi}" : YerelRecete?.RecipeName)
                                   : UzakDosyaAdi;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private TransferStatus _durum = TransferStatus.Beklemede;
        private int _ilerleme = 0;
        private string _hataMesaji = string.Empty;

        public Guid Id { get; } = Guid.NewGuid();
        public Machine Makine { get; set; }
        public ScadaRecipe? YerelRecete { get; set; }
        public string? UzakDosyaAdi { get; set; }
        public TransferType IslemTipi { get; set; }

        public TransferStatus Durum
        {
            get => _durum;
            set
            {
                if (_durum != value)
                {
                    _durum = value;
                    OnPropertyChanged(nameof(Durum));
                }
            }
        }
        public int Ilerleme
        {
            get => _ilerleme;
            set
            {
                if (_ilerleme != value)
                {
                    _ilerleme = value;
                    OnPropertyChanged(nameof(Ilerleme));
                }
            }
        }
        public string HataMesaji
        {
            get => _hataMesaji;
            set
            {
                if (_hataMesaji != value)
                {
                    _hataMesaji = value;
                    OnPropertyChanged(nameof(HataMesaji));
                }
            }
        }

        public string MakineAdi => Makine.MachineName;
    }

    public class FtpTransferService
    {
        // YENİ: Sadece bir `private static` örnek alanı bırakıldı.
        private static readonly FtpTransferService _instance = new FtpTransferService();
        public static FtpTransferService Instance => _instance;
        public event EventHandler RecipeListChanged;
        public BindingList<TransferJob> Jobs { get; } = new BindingList<TransferJob>();
        private bool _isProcessing = false;
        private SynchronizationContext _syncContext;
        private PlcPollingService _plcPollingService;

        // YENİ: Yapıcı metot, PlcPollingService'i parametre olarak alıyor.
        public FtpTransferService(PlcPollingService plcPollingService)
        {
            _plcPollingService = plcPollingService;
        }

        // YENİ: Parametresiz yapıcı, sadece singleton başlatma için.
        private FtpTransferService()
        {
            // Diğer bağımlılıklar burada yoksa, boş bırakın
        }

        public void SetSyncContext(SynchronizationContext context)
        {
            _syncContext = context;
        }

        public void QueueSendJobs(List<ScadaRecipe> receteler, Machine makine)
        {
            foreach (var recete in receteler)
            {
                if (!Jobs.Any(j => j.Makine.Id == makine.Id && j.YerelRecete?.Id == recete.Id && j.IslemTipi == TransferType.Gonder))
                {
                    Jobs.Add(new TransferJob { Makine = makine, YerelRecete = recete, IslemTipi = TransferType.Gonder });
                }
            }
            StartProcessingIfNotRunning();
        }
        public void QueueSendJobs(List<ScadaRecipe> receteler, List<Machine> makineler)
        {
            foreach (var makine in makineler)
            {
                foreach (var recete in receteler)
                {
                    if (!Jobs.Any(j => j.Makine.Id == makine.Id && j.YerelRecete?.Id == recete.Id && j.IslemTipi == TransferType.Gonder))
                    {
                        Jobs.Add(new TransferJob { Makine = makine, YerelRecete = recete, IslemTipi = TransferType.Gonder });
                    }
                }
            }
            StartProcessingIfNotRunning();
        }
        public void QueueReceiveJobs(List<string> dosyaAdlari, Machine makine)
        {
            foreach (var dosya in dosyaAdlari)
            {
                Jobs.Add(new TransferJob { Makine = makine, UzakDosyaAdi = dosya, IslemTipi = TransferType.Al });
            }
            StartProcessingIfNotRunning();
        }

        private void StartProcessingIfNotRunning()
        {
            if (!_isProcessing)
            {
                Task.Run(() => ProcessQueue(new RecipeRepository()));
            }
        }
        private string GenerateNewRecipeName(TransferJob job, ScadaRecipe recipe, RecipeRepository recipeRepo)
        {
            string machineName = job.Makine.MachineName;
            string recipeNumberPart = "0";
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(job.UzakDosyaAdi);
                Match match = Regex.Match(fileName, @"\d+$");
                if (match.Success)
                {
                    recipeNumberPart = int.Parse(match.Value).ToString();
                }
            }
            catch
            {
                recipeNumberPart = "NO_HATA";
            }
            string asciiPart = "BILGI_YOK";
            try
            {
                var step99 = recipe.Steps.FirstOrDefault(s => s.StepNumber == 99);
                if (step99 != null && step99.StepDataWords.Length >= 5)
                {
                    byte[] asciiBytes = new byte[10];
                    for (int i = 0; i < 5; i++)
                    {
                        short word = step99.StepDataWords[i];
                        byte[] wordBytes = BitConverter.GetBytes(word);
                        asciiBytes[i * 2] = wordBytes[0];
                        asciiBytes[i * 2 + 1] = wordBytes[1];
                    }

                    asciiPart = Encoding.ASCII.GetString(asciiBytes).Replace("\0", "").Trim();
                    if (string.IsNullOrEmpty(asciiPart))
                    {
                        asciiPart = "BOS";
                    }
                }
                else
                {
                    asciiPart = "ADIM99_YOK";
                }
            }
            catch
            {
                asciiPart = "HATA";
            }

            string baseName = $"{machineName}-{recipeNumberPart}-{asciiPart}";
            string finalName = baseName;
            int copyCounter = 1;
            var existingNames = new HashSet<string>(recipeRepo.GetAllRecipes().Select(r => r.RecipeName));

            while (existingNames.Contains(finalName))
            {
                finalName = $"{baseName}_Kopya{copyCounter}";
                copyCounter++;
            }

            return finalName;
        }

        private async Task ProcessQueue(RecipeRepository recipeRepo)
        {
            _isProcessing = true;
            while (Jobs.Any(j => j.Durum == TransferStatus.Beklemede))
            {
                var job = Jobs.FirstOrDefault(j => j.Durum == TransferStatus.Beklemede);
                if (job == null) continue;

                try
                {
                    job.Durum = TransferStatus.Aktarılıyor;
                    var ftpService = new FtpService(job.Makine.VncAddress, job.Makine.FtpUsername, job.Makine.FtpPassword);
                    job.Ilerleme = 20;

                    if (job.IslemTipi == TransferType.Gonder)
                    {
                        var fullRecipe = recipeRepo.GetRecipeById(job.YerelRecete.Id);
                        if (fullRecipe == null || !fullRecipe.Steps.Any())
                        {
                            throw new Exception("Reçete veritabanında bulunamadı veya adımları boş.");
                        }

                        // Reçete adını PLC'ye yaz
                        if (_plcPollingService.GetPlcManagers().TryGetValue(job.Makine.Id, out var plcManager))
                        {
                            // Hedef dosya adından reçete numarasını çıkar
                            var recipeNumberMatch = Regex.Match(job.HedefDosyaAdi, @"XPR(\d+)\.csv");
                            if (!recipeNumberMatch.Success || !int.TryParse(recipeNumberMatch.Groups[1].Value, out int recipeNumber))
                            {
                                throw new Exception("Geçersiz hedef dosya adı formatı. Reçete numarası çıkarılamadı.");
                            }

                            // PLC'ye yazma işlemi için metodu çağır
                            var writeResult = await plcManager.WriteRecipeNameAsync(recipeNumber, fullRecipe.RecipeName);
                            if (!writeResult.IsSuccess)
                            {
                                throw new Exception($"Reçete adı PLC'ye yazılamadı: {writeResult.Message}");
                            }
                        }
                        else
                        {
                            throw new Exception("PLC bağlantısı aktif değil, reçete adı PLC'ye yazılamadı.");
                        }

                        // Reçete adını PLC'ye gömmek için 99. adımı güncelle
                        string nameToEmbed = job.YerelRecete.RecipeName;
                        if (nameToEmbed.Length > 10)
                        {
                            nameToEmbed = nameToEmbed.Substring(0, 10);
                        }
                        byte[] asciiBytes = new byte[10];
                        Encoding.ASCII.GetBytes(nameToEmbed, 0, nameToEmbed.Length, asciiBytes, 0);
                        var step99 = fullRecipe.Steps.FirstOrDefault(s => s.StepNumber == 99);
                        if (step99 == null)
                        {
                            step99 = new ScadaRecipeStep { StepNumber = 99 };
                            fullRecipe.Steps.Add(step99);
                            fullRecipe.Steps = fullRecipe.Steps.OrderBy(s => s.StepNumber).ToList();
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            step99.StepDataWords[i] = BitConverter.ToInt16(asciiBytes, i * 2);
                        }

                        job.Ilerleme = 50;

                        string csvContent = RecipeCsvConverter.ToCsv(fullRecipe);
                        await ftpService.UploadFileAsync(job.HedefDosyaAdi, csvContent);
                    }
                    else
                    {
                        var csvContent = await ftpService.DownloadFileAsync(job.UzakDosyaAdi);
                        job.Ilerleme = 50;
                        var tempRecipe = RecipeCsvConverter.ToRecipe(csvContent, job.UzakDosyaAdi);
                        string newFormattedName = this.GenerateNewRecipeName(job, tempRecipe, recipeRepo);
                        tempRecipe.RecipeName = newFormattedName;
                        tempRecipe.TargetMachineType = !string.IsNullOrEmpty(job.Makine.MachineSubType) ? job.Makine.MachineSubType : job.Makine.MachineType;
                        recipeRepo.AddOrUpdateRecipe(tempRecipe);
                        RecipeListChanged?.Invoke(this, EventArgs.Empty);
                    }

                    job.Ilerleme = 100;
                    job.Durum = TransferStatus.Başarılı;

                }
                catch (Exception ex)
                {
                    job.Durum = TransferStatus.Hatalı;
                    job.HataMesaji = ex.Message;
                }
                finally
                {
                    _syncContext?.Post(_ => { }, null);
                }
            }
            _isProcessing = false;
        }

        public void QueueSequentiallyNamedSendJobs(List<ScadaRecipe> receteler, List<Machine> makineler, int startNumber)
        {
            int currentRecipeNumber = startNumber;
            foreach (var recete in receteler)
            {
                string hedefDosyaAdi = $"XPR{currentRecipeNumber:D5}.csv";
                foreach (var makine in makineler)
                {
                    // DÜZELTME: Sadece beklemede olan işler için kontrol yap.
                    // Eğer aynı makine ve reçete için beklemede bir iş yoksa yenisini ekle.
                    if (!Jobs.Any(j => j.Makine.Id == makine.Id && j.YerelRecete?.Id == recete.Id && j.HedefDosyaAdi == hedefDosyaAdi && j.Durum == TransferStatus.Beklemede))
                    {
                        Jobs.Add(new TransferJob
                        {
                            Makine = makine,
                            YerelRecete = recete,
                            IslemTipi = TransferType.Gonder,
                            HedefDosyaAdi = hedefDosyaAdi,
                            RecipeNumber = currentRecipeNumber
                        });
                    }
                }
                currentRecipeNumber++;
            }
            StartProcessingIfNotRunning();
        }
    }
}