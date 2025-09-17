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
        private Random _random = new Random(); // Kategori renkleri için

        // GÜNCELLENMİŞ: Daha fazla ve belirgin renk paleti
        private readonly List<Color> _darkColors = new List<Color>
        {
            Color.FromArgb(44, 62, 80),  // Koyu gri
            Color.FromArgb(46, 204, 113), // Zümrüt yeşili
            Color.FromArgb(231, 76, 60),  // Koyu kırmızı
            Color.FromArgb(155, 89, 182), // Menekşe
            Color.FromArgb(52, 152, 219), // Açık mavi
            Color.FromArgb(241, 196, 15),  // Güneş sarısı
            Color.FromArgb(22, 160, 133),  // Koyu Orman Yeşili
            Color.FromArgb(192, 57, 43),   // Derin Kırmızı
            Color.FromArgb(41, 128, 185),  // Koyu Mavi
            Color.FromArgb(243, 156, 18),  // Koyu Turuncu
            Color.FromArgb(211, 84, 0),    // Turuncu
            Color.FromArgb(127, 140, 141), // Gri Mavi
            Color.FromArgb(52, 73, 94),    // Koyu Mavi-Gri
            Color.FromArgb(249, 105, 14),  // Parlak Turuncu
            Color.FromArgb(189, 195, 199), // Açık Gri
            Color.FromArgb(149, 165, 166), // Orta Gri
            Color.FromArgb(236, 240, 241), // Çok Açık Gri
            Color.FromArgb(101, 159, 105), // Çam Yeşili
            Color.FromArgb(10, 61, 98),    // Gece Mavisi
            Color.FromArgb(119, 177, 169)  // Su Yeşili
        };
        private int _colorIndex = 0;


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

        public void InitializeControl(PlcPollingService pollingService, MachineRepository machineRepo, DashboardRepository dashboardRepo, AlarmRepository alarmRepo, ProcessLogRepository logRepo, ProductionRepository productionRepo)
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
                .GroupBy(m => m.MachineSubType ?? $"{Resources.diger}");
            _colorIndex = 0;
            foreach (var group in groupedMachines)
            {
                var groupPanel = new GroupBox
                {
                    Text = group.Key,
                    Width = flpMachineGroups.Width - 25,
                    AutoSize = true,
                    // YENİ: Yazı tipi, punto ve stil
                    Font = new Font("Times New Roman", 13F, FontStyle.Bold),
                    // YENİ: Başlık rengini daima beyaz yap
                    ForeColor = Color.White,
                };

                Color groupColor = _darkColors[_colorIndex % _darkColors.Count];
                groupPanel.BackColor = groupColor;

                var innerPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Padding = new Padding(5),
                    FlowDirection = FlowDirection.LeftToRight, // Yatay akış
                    WrapContents = false, // Kaydırmayı sağlamak için satır sonuna sarmayı devre dışı bırak
                    AutoScroll = true // Yatay kaydırma çubuğu
                };

                foreach (var machine in group)
                {
                    var card = new DashboardMachineCard_Control(machine);
                    _machineCards.Add(machine.Id, card);
                    innerPanel.Controls.Add(card);
                }
                groupPanel.Controls.Add(innerPanel);
                flpMachineGroups.Controls.Add(groupPanel);
                _colorIndex++;
            }
            // YENİ: Düzeni devam ettir ve tüm değişiklikleri tek seferde çizdir.
            flpMachineGroups.ResumeLayout();
        }

        // Bu metot artık kullanılmıyor, çünkü başlık metin rengini doğrudan beyaz olarak ayarladık.
        // private Color GetContrastColor(Color color)
        // {
        //     double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
        //     return luminance > 0.5 ? Color.Black : Color.White;
        // }

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

        // DÜZELTME: Bu metotta, makine dururken de manuel logları çekerek grafik verisi olmasını sağlıyoruz.
        private void PollingService_OnMachineDataRefreshed(int machineId, FullMachineStatus status)
        {
            if (_machineCards.TryGetValue(machineId, out var cardToUpdate))
            {
                List<ProcessDataPoint> trendData;

                if (!string.IsNullOrEmpty(status.BatchNumarasi))
                {
                    // Makine bir parti işliyor, o partiye ait verileri çek
                    trendData = _logRepository.GetLogsForBatch(machineId, status.BatchNumarasi, DateTime.Now.AddMinutes(-15), DateTime.Now);
                }
                else
                {
                    // Makine duruyor, son 15 dakikalık manuel logları çek
                    trendData = _logRepository.GetManualLogs(machineId, DateTime.Now.AddMinutes(-15), DateTime.Now);
                }

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
           
            var hourlyElecData = _dashboardRepository.GetHourlyFactoryConsumption(DateTime.Today);
            formsPlotHourly.Plot.Clear();
            if (hourlyElecData.Rows.Count > 0)
            {
                double[] hours = hourlyElecData.AsEnumerable().Select(row => row.IsNull("Saat") ? 0.0 : Convert.ToDouble(row["Saat"])).ToArray();
                double[] consumption = hourlyElecData.AsEnumerable().Select(row => row.IsNull("ToplamElektrik") ? 0.0 : Convert.ToDouble(row["ToplamElektrik"])).ToArray();
                var barPlot = formsPlotHourly.Plot.Add.Scatter(hours, consumption);
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
                var barPlot = formsPlotHourlyWater.Plot.Add.Scatter(hours, consumption);
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
                var barPlot = formsPlotHourlySteam.Plot.Add.Scatter(hours, consumption);
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
             //   formsPlotHourlyOee.Plot.Axes.Left.Label.Text = "Ortalama OEE (%)";
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

            gbHourlyConsumption.Text = Resources.Saatlikelektrik;
            gbTopAlarms.Text = Resources.ensikalarm;
            gbHourlyConsumptionWater.Text = Resources.ortalamasutuketimi;
            gbHourlyConsumptionSteam.Text = Resources.ortalamabuhartuketimi;
            gbHourlyOee.Text = "24 Hourly OEE";
            //btnSave.Text = Resources.Save;
        }
    }
}