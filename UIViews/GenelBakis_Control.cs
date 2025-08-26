// UI/Views/GenelBakis_Control.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Models;
using TekstilScada.Properties;
using TekstilScada.Repositories;
using TekstilScada.Services;
using TekstilScada.UI.Controls;

namespace TekstilScada.UI.Views
{
    public partial class GenelBakis_Control : UserControl
    {
        private PlcPollingService _pollingService;
        private MachineRepository _machineRepository;
        private DashboardRepository _dashboardRepository;
        private AlarmRepository _alarmRepository;
        private ProcessLogRepository _logRepository;
        private ProductionRepository _productionRepository;
        private Dictionary<int, bool> _previousBatchStatuses;
        private readonly Dictionary<int, DashboardMachineCard_Control> _machineCards = new Dictionary<int, DashboardMachineCard_Control>();
        private System.Windows.Forms.Timer _uiUpdateTimer;

        // YENİ: KPI kartları için özel alanlar ekleyin
        private KpiCard_Control _kpiTotalMachines;
        private KpiCard_Control _kpiRunningMachines;
        private KpiCard_Control _kpiAlarmMachines;
        private KpiCard_Control _kpiIdleMachines;

        public GenelBakis_Control()
        {
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
            InitializeComponent();
            // Akıcı çizim için Double Buffering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // YENİ: Göz kırpmayı önlemek için FlowLayoutPanel'e Double Buffering uygulayın
            typeof(FlowLayoutPanel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, flpTopKpis, new object[] { true });

            ApplyLocalization();
        }

        public void InitializeControl(PlcPollingService pollingService, MachineRepository machineRepo, DashboardRepository dashboardRepo, AlarmRepository alarmRepo, ProcessLogRepository logRepo,ProductionRepository productionRepo)
        {
            _pollingService = pollingService;
            _machineRepository = machineRepo;
            _dashboardRepository = dashboardRepo;
            _alarmRepository = alarmRepo;
            _logRepository = logRepo;
            _productionRepository = productionRepo; // YENİ: Atama işlemi
        }
        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();
        }
        private void GenelBakis_Control_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            // YENİ: KPI Kartlarını bir kereliğine oluşturun ve panele ekleyin
            InitializeKpiCards();
            // YENİ: Başlangıçta tüm makinelerin batch durumunu al.
            _previousBatchStatuses = _pollingService.MachineDataCache
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.IsInRecipeMode);

            BuildMachineCards();

            _pollingService.OnMachineDataRefreshed += PollingService_OnMachineDataRefreshed;

            _uiUpdateTimer = new System.Windows.Forms.Timer { Interval = 2000 }; // 2 saniyede bir güncelleme
            _uiUpdateTimer.Tick += (s, a) => RefreshDashboard();
            _uiUpdateTimer.Start();

            RefreshDashboard(); // İlk yüklemede çalıştır
        }

        // YENİ METOT: KPI kartlarını başlangıçta oluşturur
        private void InitializeKpiCards()
        {
            _kpiTotalMachines = new KpiCard_Control();
            _kpiRunningMachines = new KpiCard_Control();
            _kpiAlarmMachines = new KpiCard_Control();
            _kpiIdleMachines = new KpiCard_Control();

            flpTopKpis.Controls.Add(_kpiTotalMachines);
            flpTopKpis.Controls.Add(_kpiRunningMachines);
            flpTopKpis.Controls.Add(_kpiAlarmMachines);
            flpTopKpis.Controls.Add(_kpiIdleMachines);
        }

        private void BuildMachineCards()
        {
            // YENİ: Güncelleme süresince düzeni askıya alarak göz kırpmasını engelle.
            flpMachineGroups.SuspendLayout();

            var allMachines = _machineRepository.GetAllEnabledMachines();
            _machineCards.Clear();
            flpMachineGroups.Controls.Clear();

            var machineCache = _pollingService.MachineDataCache;

            // YENİ SIRALAMA:
            // 1. Üretim modunda olanlar en üstte.
            // 2. Kendi içlerinde, en yeni başlayanlar en üstte.
            // 3. Diğer makineler.
            var sortedMachines = allMachines
                .OrderByDescending(m =>
                {
                    if (machineCache.TryGetValue(m.Id, out var status))
                    {
                        return status.IsInRecipeMode;
                    }
                    return false;
                })
                .ThenByDescending(m =>
                {
                    if (machineCache.TryGetValue(m.Id, out var status) && status.IsInRecipeMode)
                    {
                        // ProductionRepository'den okunan batch başlangıç zamanını kullanın.
                        var batchTimes = _productionRepository.GetBatchTimestamps(status.BatchNumarasi, m.Id);
                        return batchTimes.StartTime ?? DateTime.MinValue;
                    }
                    return DateTime.MinValue;
                });

            var groupedMachines = sortedMachines
                .GroupBy(m => m.MachineSubType ?? "Diğer");

            foreach (var group in groupedMachines)
            {
                var groupPanel = new GroupBox
                {
                    Text = group.Key,
                    Width = flpMachineGroups.Width - 25,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold)
                };

                var innerPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Padding = new Padding(5)
                };

                foreach (var machine in group)
                {
                    var card = new DashboardMachineCard_Control(machine);
                    _machineCards.Add(machine.Id, card);
                    innerPanel.Controls.Add(card);
                }
                groupPanel.Controls.Add(innerPanel);
                flpMachineGroups.Controls.Add(groupPanel);
            }

            // YENİ: Düzeni devam ettir ve tüm değişiklikleri tek seferde çizdir.
            flpMachineGroups.ResumeLayout();
        }

        private void RefreshDashboard()
        {
            if (this.IsDisposed) return;

            // YENİ: Makine sıralamasını dinamik olarak kontrol et
            var currentBatchStatuses = _pollingService.MachineDataCache
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.IsInRecipeMode);

            if (!_previousBatchStatuses.SequenceEqual(currentBatchStatuses))
            {
                // Batch durumları değiştiyse, kartları yeniden oluştur ve sırala.
                BuildMachineCards();
                _previousBatchStatuses = currentBatchStatuses;
            }

            UpdateKpiCards();
            UpdateSidebarCharts();
        }

        private void PollingService_OnMachineDataRefreshed(int machineId, FullMachineStatus status)
        {
            if (_machineCards.TryGetValue(machineId, out var cardToUpdate))
            {
                // Sparkline için son 15 dakikalık veriyi çek
                var trendData = _logRepository.GetLogsForBatch(machineId, status.BatchNumarasi, DateTime.Now.AddMinutes(-15), DateTime.Now);
                cardToUpdate.UpdateData(status, trendData);
            }
        }

        // GÜNCELLENMİŞ METOT: Artık kontrolleri silip yeniden oluşturmuyor
        private void UpdateKpiCards()
        {
            var allStatuses = _pollingService.MachineDataCache.Values;

            int totalMachines = allStatuses.Count;
            int runningMachines = allStatuses.Count(s => s.IsInRecipeMode && !s.HasActiveAlarm);
            int alarmMachines = allStatuses.Count(s => s.HasActiveAlarm);
            int idleMachines = totalMachines - runningMachines - alarmMachines;

            // Mevcut kartların verilerini güncelle
            _kpiTotalMachines.SetData($"{Resources.AllMachines}", totalMachines.ToString(), Color.FromArgb(41, 128, 185));
            _kpiRunningMachines.SetData($"{Resources.aktifüretim}", runningMachines.ToString(), Color.FromArgb(46, 204, 113));
            _kpiAlarmMachines.SetData($"{Resources.alarmdurum}", alarmMachines.ToString(), Color.FromArgb(231, 76, 60));
            _kpiIdleMachines.SetData($"{Resources.bosbekleyen}", idleMachines.ToString(), Color.FromArgb(243, 156, 18));
        }

        private void UpdateSidebarCharts()
        {
            // Saatlik Tüketim Grafiği
            // Saatlik Elektrik Tüketimi
            var hourlyElecData = _dashboardRepository.GetHourlyFactoryConsumption(DateTime.Today);
            formsPlotHourly.Plot.Clear();
            if (hourlyElecData.Rows.Count > 0)
            {
                double[] hours = hourlyElecData.AsEnumerable().Select(row => row.IsNull("Saat") ? 0.0 : Convert.ToDouble(row["Saat"])).ToArray();
                double[] consumption = hourlyElecData.AsEnumerable().Select(row => row.IsNull("ToplamElektrik") ? 0.0 : Convert.ToDouble(row["ToplamElektrik"])).ToArray();
                var barPlot = formsPlotHourly.Plot.Add.Bars(hours, consumption);
                barPlot.Color = ScottPlot.Colors.SteelBlue;
            }
           // formsPlotHourly.Plot.Title(Resources.SaatlikElektrik);
            formsPlotHourly.Plot.Axes.AutoScale();
            formsPlotHourly.Refresh();

            // Saatlik Su Tüketimi
            var hourlyWaterData = _dashboardRepository.GetHourlyFactoryConsumption(DateTime.Today);
            formsPlotHourlyWater.Plot.Clear();
            if (hourlyWaterData.Rows.Count > 0)
            {
                double[] hours = hourlyWaterData.AsEnumerable().Select(row => row.IsNull("Saat") ? 0.0 : Convert.ToDouble(row["Saat"])).ToArray();
                double[] consumption = hourlyWaterData.AsEnumerable().Select(row => row.IsNull("ToplamSu") ? 0.0 : Convert.ToDouble(row["ToplamSu"])).ToArray();
                var barPlot = formsPlotHourlyWater.Plot.Add.Bars(hours, consumption);
                barPlot.Color = ScottPlot.Colors.CornflowerBlue; // Farklı bir renk
            }
           // formsPlotHourlyWater.Plot.Title(Resources.SaatlikSu);
            formsPlotHourlyWater.Plot.Axes.AutoScale();
            formsPlotHourlyWater.Refresh();

            // Saatlik Buhar Tüketimi
            var hourlySteamData = _dashboardRepository.GetHourlyFactoryConsumption(DateTime.Today);
            formsPlotHourlySteam.Plot.Clear();
            if (hourlySteamData.Rows.Count > 0)
            {
                double[] hours = hourlySteamData.AsEnumerable().Select(row => row.IsNull("Saat") ? 0.0 : Convert.ToDouble(row["Saat"])).ToArray();
                double[] consumption = hourlySteamData.AsEnumerable().Select(row => row.IsNull("ToplamBuhar") ? 0.0 : Convert.ToDouble(row["ToplamBuhar"])).ToArray();
                var barPlot = formsPlotHourlySteam.Plot.Add.Bars(hours, consumption);
                barPlot.Color = ScottPlot.Colors.DimGray; // Farklı bir renk
            }
           // formsPlotHourlySteam.Plot.Title(Resources.SaatlikBuhar);
            formsPlotHourlySteam.Plot.Axes.AutoScale();
            formsPlotHourlySteam.Refresh();

            // Popüler Alarmlar Grafiği
            var topAlarms = _alarmRepository.GetTopAlarmsByFrequency(DateTime.Now.AddDays(-1), DateTime.Now);
            formsPlotTopAlarms.Plot.Clear();
            if (topAlarms.Any())
            {
                double[] counts = topAlarms.Select(a => (double)a.Count).ToArray();
                var labels = topAlarms.Select(a => a.AlarmText).ToArray();
                var barPlot = formsPlotTopAlarms.Plot.Add.Bars(counts);
                barPlot.Color = ScottPlot.Colors.OrangeRed;
                var ticks = Enumerable.Range(0, labels.Length).Select(i => new ScottPlot.Tick(i, labels[i])).ToArray();
                formsPlotTopAlarms.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
                formsPlotTopAlarms.Plot.Axes.Bottom.TickLabelStyle.Rotation = 45;
            }
          //  formsPlotTopAlarms.Plot.Title(Resources.ensikalarm);
            formsPlotTopAlarms.Plot.Axes.AutoScale();
            formsPlotTopAlarms.Refresh();
            var hourlyOeeData = _dashboardRepository.GetHourlyAverageOee(DateTime.Today);
            formsPlotHourlyOee.Plot.Clear();
            if (hourlyOeeData.Rows.Count > 0)
            {
                double[] hours = hourlyOeeData.AsEnumerable().Select(row => row.IsNull("Saat") ? 0.0 : Convert.ToDouble(row["Saat"])).ToArray();
                double[] oeeValues = hourlyOeeData.AsEnumerable().Select(row => row.IsNull("AverageOEE") ? 0.0 : Convert.ToDouble(row["AverageOEE"])).ToArray();

                var linePlot = formsPlotHourlyOee.Plot.Add.Scatter(hours, oeeValues);
                linePlot.Color = ScottPlot.Colors.Orange;
                linePlot.LineStyle.Width = 2;
                linePlot.MarkerStyle.Shape = ScottPlot.MarkerShape.FilledCircle;
                linePlot.MarkerStyle.Size = 5;

                formsPlotHourlyOee.Plot.Axes.Bottom.Label.Text = "Saat";
                formsPlotHourlyOee.Plot.Axes.Left.Label.Text = "Ortalama OEE (%)";
            }
           // formsPlotHourlyOee.Plot.Title("24 Saatlik OEE");
            formsPlotHourlyOee.Plot.Axes.AutoScale();
            formsPlotHourlyOee.Refresh();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_pollingService != null)
            {
                _pollingService.OnMachineDataRefreshed -= PollingService_OnMachineDataRefreshed;
            }
            _uiUpdateTimer?.Stop();
            _uiUpdateTimer?.Dispose();
            base.OnHandleDestroyed(e);
        }
        public void ApplyLocalization()
        {

            gbHourlyConsumption.Text = Resources.saatlik;

            gbTopAlarms.Text = Resources.son24topalarm;

           // gbHourlyOee.Text = Resources.hourlyoee;
            //btnSave.Text = Resources.Save;


        }
    }
}