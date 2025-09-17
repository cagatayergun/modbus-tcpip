// ======================================================
// FILE: TekstilScada.Core/Services/FtpTransferService.cs
// New property added to the TransferJob class and queue processing logic updated.
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
    public enum TransferType { Send, Receive }
    public enum TransferStatus { Pending, Transferring, Successful, Failed }

    public class TransferJob : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string TargetFileName { get; set; }

        // NEWLY ADDED PROPERTY
        public int RecipeNumber { get; set; }

        public string RecipeName => OperationType == TransferType.Send
                                    ? (!string.IsNullOrEmpty(TargetFileName) ? $"{LocalRecipe?.RecipeName} -> {TargetFileName}" : LocalRecipe?.RecipeName)
                                    : RemoteFileName;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private TransferStatus _status = TransferStatus.Pending;
        private int _progress = 0;
        private string _errorMessage = string.Empty;

        public Guid Id { get; } = Guid.NewGuid();
        public Machine Machine { get; set; }
        public ScadaRecipe? LocalRecipe { get; set; }
        public string? RemoteFileName { get; set; }
        public TransferType OperationType { get; set; }

        public TransferStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        public int Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged(nameof(Progress));
                }
            }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        public string MachineName => Machine.MachineName;
    }

    public class FtpTransferService
    {
        // NEW: Only one 'private static' instance field is left.
        private static readonly FtpTransferService _instance = new FtpTransferService();
        public static FtpTransferService Instance => _instance;
        public event EventHandler RecipeListChanged;
        public BindingList<TransferJob> Jobs { get; } = new BindingList<TransferJob>();
        private bool _isProcessing = false;
        private SynchronizationContext _syncContext;
        private PlcPollingService _plcPollingService;

        // NEW: The constructor method takes PlcPollingService as a parameter.
        public FtpTransferService(PlcPollingService plcPollingService)
        {
            _plcPollingService = plcPollingService;
        }

        // NEW: Parameterless constructor, only for singleton initialization.
        private FtpTransferService()
        {
            // If there are no other dependencies here, leave it empty
        }

        public void SetSyncContext(SynchronizationContext context)
        {
            _syncContext = context;
        }

        public void QueueSendJobs(List<ScadaRecipe> recipes, Machine machine)
        {
            foreach (var recipe in recipes)
            {
                if (!Jobs.Any(j => j.Machine.Id == machine.Id && j.LocalRecipe?.Id == recipe.Id && j.OperationType == TransferType.Send))
                {
                    Jobs.Add(new TransferJob { Machine = machine, LocalRecipe = recipe, OperationType = TransferType.Send });
                }
            }
            StartProcessingIfNotRunning();
        }
        public void QueueSendJobs(List<ScadaRecipe> recipes, List<Machine> machines)
        {
            foreach (var machine in machines)
            {
                foreach (var recipe in recipes)
                {
                    if (!Jobs.Any(j => j.Machine.Id == machine.Id && j.LocalRecipe?.Id == recipe.Id && j.OperationType == TransferType.Send))
                    {
                        Jobs.Add(new TransferJob { Machine = machine, LocalRecipe = recipe, OperationType = TransferType.Send });
                    }
                }
            }
            StartProcessingIfNotRunning();
        }
        public void QueueReceiveJobs(List<string> fileNames, Machine machine)
        {
            foreach (var file in fileNames)
            {
                Jobs.Add(new TransferJob { Machine = machine, RemoteFileName = file, OperationType = TransferType.Receive });
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
            string machineName = job.Machine.MachineName;
            string recipeNumberPart = "0";
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(job.RemoteFileName);
                Match match = Regex.Match(fileName, @"\d+$");
                if (match.Success)
                {
                    recipeNumberPart = int.Parse(match.Value).ToString();
                }
            }
            catch
            {
                recipeNumberPart = "NO_ERROR";
            }
            string asciiPart = "NO_INFO";
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
                        asciiPart = "EMPTY";
                    }
                }
                else
                {
                    asciiPart = "STEP99_MISSING";
                }
            }
            catch
            {
                asciiPart = "ERROR";
            }

            string baseName = $"{machineName}-{recipeNumberPart}-{asciiPart}";
            string finalName = baseName;
            int copyCounter = 1;
            var existingNames = new HashSet<string>(recipeRepo.GetAllRecipes().Select(r => r.RecipeName));

            while (existingNames.Contains(finalName))
            {
                finalName = $"{baseName}_Copy{copyCounter}";
                copyCounter++;
            }

            return finalName;
        }

        private async Task ProcessQueue(RecipeRepository recipeRepo)
        {
            _isProcessing = true;
            while (Jobs.Any(j => j.Status == TransferStatus.Pending))
            {
                var job = Jobs.FirstOrDefault(j => j.Status == TransferStatus.Pending);
                if (job == null) continue;

                try
                {
                    job.Status = TransferStatus.Transferring;
                    var ftpService = new FtpService(job.Machine.VncAddress, job.Machine.FtpUsername, job.Machine.FtpPassword);
                    job.Progress = 20;

                    if (job.OperationType == TransferType.Send)
                    {
                        var fullRecipe = recipeRepo.GetRecipeById(job.LocalRecipe.Id);
                        if (fullRecipe == null || !fullRecipe.Steps.Any())
                        {
                            throw new Exception("Recipe not found in database or steps are empty.");
                        }

                        // Write the recipe name to the PLC
                        if (_plcPollingService.GetPlcManagers().TryGetValue(job.Machine.Id, out var plcManager))
                        {
                            // Extract the recipe number from the target filename
                            var recipeNumberMatch = Regex.Match(job.TargetFileName, @"XPR(\d+)\.csv");
                            if (!recipeNumberMatch.Success || !int.TryParse(recipeNumberMatch.Groups[1].Value, out int recipeNumber))
                            {
                                throw new Exception("Invalid target filename format. Recipe number could not be extracted.");
                            }

                            // Call the method to write to the PLC
                            var writeResult = await plcManager.WriteRecipeNameAsync(recipeNumber, fullRecipe.RecipeName);
                            if (!writeResult.IsSuccess)
                            {
                                throw new Exception($"Recipe name could not be written to PLC: {writeResult.Message}");
                            }
                        }
                        else
                        {
                            throw new Exception("PLC connection is not active, recipe name could not be written to PLC.");
                        }

                        // Update step 99 to embed the recipe name in the PLC
                        string nameToEmbed = job.LocalRecipe.RecipeName;
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

                        job.Progress = 50;

                        string csvContent = RecipeCsvConverter.ToCsv(fullRecipe);
                        await ftpService.UploadFileAsync(job.TargetFileName, csvContent);
                    }
                    else
                    {
                        var csvContent = await ftpService.DownloadFileAsync(job.RemoteFileName);
                        job.Progress = 50;
                        var tempRecipe = RecipeCsvConverter.ToRecipe(csvContent, job.RemoteFileName);
                        string newFormattedName = this.GenerateNewRecipeName(job, tempRecipe, recipeRepo);
                        tempRecipe.RecipeName = newFormattedName;
                        tempRecipe.TargetMachineType = !string.IsNullOrEmpty(job.Machine.MachineSubType) ? job.Machine.MachineSubType : job.Machine.MachineType;
                        recipeRepo.AddOrUpdateRecipe(tempRecipe);
                        RecipeListChanged?.Invoke(this, EventArgs.Empty);
                    }

                    job.Progress = 100;
                    job.Status = TransferStatus.Successful;

                }
                catch (Exception ex)
                {
                    job.Status = TransferStatus.Failed;
                    job.ErrorMessage = ex.Message;
                }
                finally
                {
                    _syncContext?.Post(_ => { }, null);
                }
            }
            _isProcessing = false;
        }

        public void QueueSequentiallyNamedSendJobs(List<ScadaRecipe> recipes, List<Machine> machines, int startNumber)
        {
            int currentRecipeNumber = startNumber;
            foreach (var recipe in recipes)
            {
                string targetFileName = $"XPR{currentRecipeNumber:D5}.csv";
                foreach (var machine in machines)
                {
                    // CORRECTION: Check only for pending jobs.
                    // If there is no pending job for the same machine and recipe, add a new one.
                    if (!Jobs.Any(j => j.Machine.Id == machine.Id && j.LocalRecipe?.Id == recipe.Id && j.TargetFileName == targetFileName && j.Status == TransferStatus.Pending))
                    {
                        Jobs.Add(new TransferJob
                        {
                            Machine = machine,
                            LocalRecipe = recipe,
                            OperationType = TransferType.Send,
                            TargetFileName = targetFileName,
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