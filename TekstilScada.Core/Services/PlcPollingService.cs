// Dosya: TekstilScada.Core/Services/PlcPollingService.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TekstilScada.Core;
using TekstilScada.Core.Services;
using TekstilScada.Models;
using TekstilScada.Repositories;

namespace TekstilScada.Services
{
    public class PlcPollingService
    {
        public event Action<int, FullMachineStatus> OnMachineDataRefreshed;
        private ConcurrentDictionary<int, IPlcManager> _plcManagers;
        public ConcurrentDictionary<int, FullMachineStatus> MachineDataCache { get; private set; }
        private readonly AlarmRepository _alarmRepository;
        private readonly ProcessLogRepository _processLogRepository;
        private readonly ProductionRepository _productionRepository;
        private ConcurrentDictionary<int, string> _currentBatches;
        private ConcurrentDictionary<int, DateTime> _reconnectAttempts;
        private ConcurrentDictionary<int, ConnectionStatus> _connectionStates;
        private System.Threading.Timer _loggingTimer;
        public event Action<int, FullMachineStatus> OnMachineConnectionStateChanged;
        public event Action<int, FullMachineStatus> OnActiveAlarmStateChanged;
        private ConcurrentDictionary<int, AlarmDefinition> _alarmDefinitionsCache;
        private ConcurrentDictionary<int, ConcurrentDictionary<int, DateTime>> _activeAlarmsTracker;
        private readonly ConcurrentDictionary<int, LiveStepAnalyzer> _liveAnalyzers;
        private readonly RecipeRepository _recipeRepository;
        private readonly ConcurrentDictionary<int, (int machineAlarmSeconds, int operatorPauseSeconds)> _liveAlarmCounters;

        private readonly int _pollingIntervalMs = 1000;
        private readonly int _loggingIntervalMs = 5000;

        // GÜNCELLENDİ: Hataları giderilen ve multi-threading için gerekli olan tüm değişkenler burada.
        private CancellationTokenSource _cancellationTokenSource;
        private List<Task> _pollingTasks;
        private readonly ConcurrentDictionary<int, double> _batchTotalTheoreticalTimes;
        private readonly ConcurrentDictionary<int, DateTime> _batchStartTimes;
        private readonly ConcurrentDictionary<int, double> _batchNonProductiveSeconds;

        public PlcPollingService(AlarmRepository alarmRepository, ProcessLogRepository processLogRepository, ProductionRepository productionRepository, RecipeRepository recipeRepository)
        {
            _alarmRepository = alarmRepository;
            _processLogRepository = processLogRepository;
            _productionRepository = productionRepository;
            _recipeRepository = recipeRepository;

            _plcManagers = new ConcurrentDictionary<int, IPlcManager>();
            MachineDataCache = new ConcurrentDictionary<int, FullMachineStatus>();
            _reconnectAttempts = new ConcurrentDictionary<int, DateTime>();
            _connectionStates = new ConcurrentDictionary<int, ConnectionStatus>();
            _activeAlarmsTracker = new ConcurrentDictionary<int, ConcurrentDictionary<int, DateTime>>();
            _currentBatches = new ConcurrentDictionary<int, string>();
            _liveAnalyzers = new ConcurrentDictionary<int, LiveStepAnalyzer>();
            _liveAlarmCounters = new ConcurrentDictionary<int, (int, int)>();
            _pollingTasks = new List<Task>();

            // HATA GİDERİLDİ: Eksik olan değişkenlerin başlatılması eklendi.
            _batchTotalTheoreticalTimes = new ConcurrentDictionary<int, double>();
            _batchStartTimes = new ConcurrentDictionary<int, DateTime>();
            _batchNonProductiveSeconds = new ConcurrentDictionary<int, double>();
        }

        public void Start(List<Models.Machine> machines)
        {
            Stop();
            _cancellationTokenSource = new CancellationTokenSource();

            LoadAlarmDefinitionsCache();
            foreach (var machine in machines)
            {
                try
                {
                    var plcManager = PlcManagerFactory.Create(machine);
                    _plcManagers.TryAdd(machine.Id, plcManager);
                    _connectionStates.TryAdd(machine.Id, ConnectionStatus.Disconnected);
                    MachineDataCache.TryAdd(machine.Id, new FullMachineStatus { MachineId = machine.Id, MachineName = machine.MachineName, ConnectionState = ConnectionStatus.Disconnected });
                    _activeAlarmsTracker.TryAdd(machine.Id, new ConcurrentDictionary<int, DateTime>());
                    _currentBatches.TryAdd(machine.Id, null);

                    var machineTask = Task.Run(() => PollMachineLoop(machine, plcManager, _cancellationTokenSource.Token));
                    _pollingTasks.Add(machineTask);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Makine {machine.Id} başlatılırken hata: {ex.Message}");
                }
            }
            _loggingTimer = new System.Threading.Timer(LoggingTimer_Tick, null, 1000, _loggingIntervalMs);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            try
            {
                if (_pollingTasks.Any())
                {
                    Task.WhenAll(_pollingTasks).Wait(2000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Polling task'leri durdurulurken hata: {ex.Message}");
            }
            _loggingTimer?.Change(Timeout.Infinite, 0);
            _loggingTimer?.Dispose();
            if (_plcManagers != null && !_plcManagers.IsEmpty)
            {
                foreach (var manager in _plcManagers.Values)
                {
                    manager.Disconnect();
                }
            }
            _plcManagers?.Clear();
            MachineDataCache?.Clear();
            _connectionStates?.Clear();
            _activeAlarmsTracker?.Clear();
            _currentBatches?.Clear();
            _cancellationTokenSource?.Dispose();
            _pollingTasks?.Clear();
        }

        private async Task PollMachineLoop(Machine machine, IPlcManager manager, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!MachineDataCache.TryGetValue(machine.Id, out var status)) return;

                    if (status.ConnectionState != ConnectionStatus.Connected)
                    {
                        HandleReconnection(machine.Id, manager);
                    }
                    else
                    {
                        var readResult = manager.ReadLiveStatusData();
                        if (readResult.IsSuccess)
                        {
                            var newStatus = readResult.Content;
                            newStatus.MachineId = machine.Id;
                            newStatus.MachineName = status.MachineName;
                            newStatus.ConnectionState = ConnectionStatus.Connected;

                            newStatus.AktifAdimAdi = GetStepTypeName(newStatus.AktifAdimTipiWordu);

                            var analyzer = _liveAnalyzers.TryGetValue(machine.Id, out var a) ? a : null;
                            if (newStatus.IsInRecipeMode && analyzer != null && _batchTotalTheoreticalTimes.TryGetValue(machine.Id, out double totalTheoreticalTime) && totalTheoreticalTime > 0)
                            {
                                double timeInCurrentStep = (DateTime.Now - analyzer.CurrentStepStartTime).TotalSeconds;
                                var remainingSteps = analyzer.Recipe.Steps.Where(s => s.StepNumber >= newStatus.AktifAdimNo);
                                double remainingTheoreticalTime = RecipeAnalysis.CalculateTheoreticalTimeForSteps(remainingSteps);
                                double completedStepsTime = totalTheoreticalTime - remainingTheoreticalTime;
                                double totalProgressSeconds = completedStepsTime + timeInCurrentStep;
                                double percentage = (totalProgressSeconds / totalTheoreticalTime) * 100.0;
                                newStatus.ProsesYuzdesi = (short)Math.Min(100.0, Math.Max(0.0, percentage));
                            }
                            else
                            {
                                newStatus.ProsesYuzdesi = 0;
                            }

                            ProcessLiveStepAnalysis(machine.Id, newStatus);
                            CheckAndLogBatchStartAndEnd(machine.Id, newStatus);
                            CheckAndLogAlarms(machine.Id, newStatus);
                            status = newStatus;

                            if (_currentBatches.TryGetValue(machine.Id, out var activeBatch) && activeBatch != null)
                            {
                                if (_liveAlarmCounters.TryGetValue(machine.Id, out var counters))
                                {
                                    // Alarm durumu, duraklatma durumundan önceliklidir.
                                    if (newStatus.HasActiveAlarm)
                                    {
                                        counters.machineAlarmSeconds += _pollingIntervalMs / 1000;
                                    }
                                    else if (newStatus.IsPaused)
                                    {
                                        counters.operatorPauseSeconds += _pollingIntervalMs / 1000;
                                    }
                                    _liveAlarmCounters[machine.Id] = counters;
                                }
                            }
                        }
                        else
                        {
                            HandleDisconnection(machine.Id);
                            if (MachineDataCache.ContainsKey(machine.Id))
                                status = MachineDataCache[machine.Id];
                        }
                    }
                    if (MachineDataCache.ContainsKey(machine.Id))
                    {
                        MachineDataCache[machine.Id] = status;
                    }
                    OnMachineDataRefreshed?.Invoke(machine.Id, status);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Makine {machine.Id} için polling döngüsünde hata: {ex.Message}");
                }

                await Task.Delay(_pollingIntervalMs, token);
            }
        }

        #region Mevcut Metotlar (Değişiklik Yok)
        private string GetStepTypeName(short controlWord)
        {
            var stepTypes = new List<string>();
            if ((controlWord & 1) != 0) stepTypes.Add("Su Alma");
            if ((controlWord & 2) != 0) stepTypes.Add("Isıtma");
            if ((controlWord & 4) != 0) stepTypes.Add("Çalışma");
            if ((controlWord & 8) != 0) stepTypes.Add("Dozaj");
            if ((controlWord & 16) != 0) stepTypes.Add("Boşaltma");
            if ((controlWord & 32) != 0) stepTypes.Add("Sıkma");
            return stepTypes.Any() ? string.Join(" + ", stepTypes) : "Bekliyor...";
        }
        private async void CheckAndLogBatchStartAndEnd(int machineId, FullMachineStatus currentStatus)
        {
            // O anki makine için hangi batch'i takip ettiğimizi alıyoruz.
            _currentBatches.TryGetValue(machineId, out string lastTrackedBatchId);

            // BATCH BAŞLANGICI TESPİTİ
            // Koşul: Makine reçete modunda, PLC'den bir batch ID geliyor VE bu ID bizim son takip ettiğimizden farklı.
            if (currentStatus.IsInRecipeMode && !string.IsNullOrEmpty(currentStatus.BatchNumarasi) && currentStatus.BatchNumarasi != lastTrackedBatchId)
            {
                // Takip ettiğimiz Batch ID'yi PLC'den gelen YENİ ID ile güncelliyoruz.
                // Bu sayede bu blok sadece bir kez, yani batch ilk başladığında çalışır.
                _currentBatches[machineId] = currentStatus.BatchNumarasi;

                // Veritabanına, operatörün girdiği Batch ID ile yeni bir kayıt atıyoruz.
                _productionRepository.StartNewBatch(currentStatus);

                // Diğer sayaçları sıfırla
                _liveAlarmCounters[machineId] = (0, 0);
                var recipe = _recipeRepository.GetRecipeByName(currentStatus.RecipeName);
                if (recipe != null)
                {
                    var fullRecipe = _recipeRepository.GetRecipeById(recipe.Id);
                    _liveAnalyzers[machineId] = new LiveStepAnalyzer(fullRecipe);
                    double totalSeconds = RecipeAnalysis.CalculateTotalTheoreticalTimeSeconds(fullRecipe);
                    _batchTotalTheoreticalTimes[machineId] = totalSeconds;
                    _batchStartTimes[machineId] = DateTime.Now;
                    _batchNonProductiveSeconds[machineId] = 0;
                }
            }
            // BATCH BİTİŞİ TESPİTİ
            // Koşul: Makine artık reçete modunda değil VE biz hala bir batch takip ediyorduk.
            else if (!currentStatus.IsInRecipeMode && lastTrackedBatchId != null)
            {
                int actualProducedQuantity = currentStatus.ActualQuantityProduction;
                _liveAlarmCounters.TryGetValue(machineId, out var finalCounters);
                int totalDowntimeFromScada = finalCounters.machineAlarmSeconds + finalCounters.operatorPauseSeconds;
                _batchTotalTheoreticalTimes.TryGetValue(machineId, out double theoreticalTime);


                // YENİ KOD: Tüketim verilerini PLC'den oku ve kaydet
               
                // Batch'i kapatırken, bizim takip ettiğimiz son Batch ID'yi (`lastTrackedBatchId`) kullanıyoruz.
                _productionRepository.EndBatch(
              machineId, lastTrackedBatchId, currentStatus,
              finalCounters.machineAlarmSeconds, finalCounters.operatorPauseSeconds,
              actualProducedQuantity, totalDowntimeFromScada, theoreticalTime);

                // Artık bu makine için bir batch takip etmiyoruz, bir sonraki yeni batch'e kadar temizliyoruz.
                _currentBatches[machineId] = null;

                // Diğer verileri temizle
                _liveAlarmCounters.TryRemove(machineId, out _);
                _liveAnalyzers.TryRemove(machineId, out _);
                _batchTotalTheoreticalTimes.TryRemove(machineId, out _);
                _batchStartTimes.TryRemove(machineId, out _);
                _batchNonProductiveSeconds.TryRemove(machineId, out _);
                if (_plcManagers.TryGetValue(machineId, out var plcManager))
                {
                    var summaryResult = await plcManager.ReadBatchSummaryDataAsync();
                    if (summaryResult.IsSuccess)
                    {
                        _productionRepository.UpdateBatchSummary(machineId, lastTrackedBatchId, summaryResult.Content);
                    }
                    else
                    {
                        Console.WriteLine($"Batch {lastTrackedBatchId} için özet verileri okunamadı: {summaryResult.Message}");
                    }
                    Task.Run(async () => {
                        await plcManager.IncrementProductionCounterAsync();
                        await plcManager.ResetOeeCountersAsync();
                    });
                }
            }
        }
        private void HandleDisconnection(int machineId)
        {
            if (!MachineDataCache.TryGetValue(machineId, out var status)) return;
            status.ConnectionState = ConnectionStatus.ConnectionLost;
            status.ProsesYuzdesi = 0;
            _connectionStates[machineId] = ConnectionStatus.ConnectionLost;
            _reconnectAttempts.TryAdd(machineId, DateTime.UtcNow);
            OnMachineConnectionStateChanged?.Invoke(machineId, status);
            LiveEventAggregator.Instance.Publish(new LiveEvent { Source = status.MachineName, Message = "İletişim koptu!", Type = EventType.SystemWarning });
        }
        private void ProcessLiveStepAnalysis(int machineId, FullMachineStatus currentStatus)
        {
            if (!currentStatus.IsInRecipeMode || string.IsNullOrEmpty(currentStatus.BatchNumarasi)) return;
            if (_liveAnalyzers.TryGetValue(machineId, out var analyzer))
            {
                if (analyzer.ProcessData(currentStatus))
                {
                    var completedStepAnalysis = analyzer.GetLastCompletedStep();
                    if (completedStepAnalysis != null)
                    {
                        _productionRepository.LogSingleStepDetail(completedStepAnalysis, machineId, currentStatus.BatchNumarasi);
                    }
                }
            }
        }
        private void HandleReconnection(int machineId, IPlcManager manager)
        {
            if (!_reconnectAttempts.ContainsKey(machineId) || (DateTime.UtcNow - _reconnectAttempts[machineId]).TotalSeconds > 10)
            {
                _reconnectAttempts[machineId] = DateTime.UtcNow;
                if (!MachineDataCache.TryGetValue(machineId, out var status)) return;
                status.ConnectionState = ConnectionStatus.Connecting;
                status.ProsesYuzdesi = 0;
                _connectionStates[machineId] = ConnectionStatus.Connecting;
                OnMachineConnectionStateChanged?.Invoke(machineId, status);
                var connectResult = manager.Connect();
                if (connectResult.IsSuccess)
                {
                    status.ConnectionState = ConnectionStatus.Connected;
                    _connectionStates[machineId] = ConnectionStatus.Connected;
                    _reconnectAttempts.TryRemove(machineId, out _);
                    OnMachineConnectionStateChanged?.Invoke(machineId, status);
                    LiveEventAggregator.Instance.Publish(new LiveEvent { Timestamp = DateTime.Now, Source = status.MachineName, Message = "İletişim yeniden kuruldu.", Type = EventType.SystemSuccess });
                }
                else
                {
                    _connectionStates[machineId] = ConnectionStatus.Disconnected;
                }
            }
        }
        private void LoggingTimer_Tick(object state)
        {
            if (_cancellationTokenSource.IsCancellationRequested) return;
            foreach (var machineStatus in MachineDataCache.Values)
            {
                if (machineStatus.ConnectionState == ConnectionStatus.Connected)
                {
                    try
                    {
                        if (machineStatus.IsInRecipeMode)
                        {
                            _processLogRepository.LogData(machineStatus);
                        }
                        else
                        {
                            _processLogRepository.LogManualData(machineStatus);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Makine {machineStatus.MachineId} için veri loglama hatası: {ex.Message}");
                    }
                }
            }
        }
        private void LoadAlarmDefinitionsCache()
        {
            try
            {
                var definitions = _alarmRepository.GetAllAlarmDefinitions();
                _alarmDefinitionsCache = new ConcurrentDictionary<int, AlarmDefinition>(
                    definitions.ToDictionary(def => def.AlarmNumber, def => def)
                );
            }
            catch (Exception)
            {
                _alarmDefinitionsCache = new ConcurrentDictionary<int, AlarmDefinition>();
            }
        }
        private void CheckAndLogAlarms(int machineId, FullMachineStatus currentStatus)
        {
            if (_activeAlarmsTracker == null || !_activeAlarmsTracker.TryGetValue(machineId, out var machineActiveAlarms))
            {
                _activeAlarmsTracker?.TryAdd(machineId, new ConcurrentDictionary<int, DateTime>());
                return;
            }
            MachineDataCache.TryGetValue(machineId, out var previousStatus);
            int previousAlarmNumber = previousStatus?.ActiveAlarmNumber ?? 0;
            int currentAlarmNumber = currentStatus.ActiveAlarmNumber;
            if (currentAlarmNumber > 0)
            {
                if (!machineActiveAlarms.ContainsKey(currentAlarmNumber) && _alarmDefinitionsCache.TryGetValue(currentAlarmNumber, out var newAlarmDef))
                {
                    _alarmRepository.WriteAlarmHistoryEvent(machineId, newAlarmDef.Id, "ACTIVE");
                    LiveEventAggregator.Instance.PublishAlarm(currentStatus.MachineName, newAlarmDef.AlarmText);
                }
                machineActiveAlarms[currentAlarmNumber] = DateTime.Now;
            }
            var timedOutAlarms = machineActiveAlarms.Where(kvp => (DateTime.Now - kvp.Value).TotalSeconds > 30).ToList();
            foreach (var timedOutAlarm in timedOutAlarms)
            {
                if (_alarmDefinitionsCache.TryGetValue(timedOutAlarm.Key, out var oldAlarmDef))
                {
                    _alarmRepository.WriteAlarmHistoryEvent(machineId, oldAlarmDef.Id, "INACTIVE");
                    LiveEventAggregator.Instance.Publish(new LiveEvent { Type = EventType.SystemInfo, Source = currentStatus.MachineName, Message = $"{oldAlarmDef.AlarmText} - TEMİZLENDİ" });
                }
                machineActiveAlarms.TryRemove(timedOutAlarm.Key, out _);
            }
            if (currentAlarmNumber == 0 && !machineActiveAlarms.IsEmpty)
            {
                foreach (var activeAlarm in machineActiveAlarms)
                {
                    if (_alarmDefinitionsCache.TryGetValue(activeAlarm.Key, out var oldAlarmDef))
                    {
                        _alarmRepository.WriteAlarmHistoryEvent(machineId, oldAlarmDef.Id, "INACTIVE");
                    }
                }
                machineActiveAlarms.Clear();
            }
            currentStatus.HasActiveAlarm = !machineActiveAlarms.IsEmpty;
            if (currentStatus.HasActiveAlarm)
            {
                currentStatus.ActiveAlarmNumber = machineActiveAlarms.OrderByDescending(kvp => kvp.Value).First().Key;
                if (_alarmDefinitionsCache.TryGetValue(currentStatus.ActiveAlarmNumber, out var def))
                {
                    currentStatus.ActiveAlarmText = def.AlarmText;
                }
                else
                {
                    currentStatus.ActiveAlarmText = $"TANIMSIZ ALARM ({currentStatus.ActiveAlarmNumber})";
                }
            }
            else
            {
                currentStatus.ActiveAlarmNumber = 0;
                currentStatus.ActiveAlarmText = "";
            }
            if ((previousStatus?.HasActiveAlarm ?? false) != currentStatus.HasActiveAlarm || previousAlarmNumber != currentStatus.ActiveAlarmNumber)
            {
                OnActiveAlarmStateChanged?.Invoke(machineId, currentStatus);
            }
        }
        public List<AlarmDefinition> GetActiveAlarmsForMachine(int machineId)
        {
            var activeAlarms = new List<AlarmDefinition>();
            if (_activeAlarmsTracker.TryGetValue(machineId, out var machineActiveAlarms) && !machineActiveAlarms.IsEmpty)
            {
                foreach (var alarmNumber in machineActiveAlarms.Keys)
                {
                    if (_alarmDefinitionsCache.TryGetValue(alarmNumber, out var alarmDef))
                    {
                        activeAlarms.Add(alarmDef);
                    }
                }
            }
            return activeAlarms.OrderByDescending(a => a.Severity).ThenBy(a => a.AlarmNumber).ToList();
        }
        public Dictionary<int, IPlcManager> GetPlcManagers()
        {
            return new Dictionary<int, IPlcManager>(_plcManagers);
        }
        #endregion
    }
}