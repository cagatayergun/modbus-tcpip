// UI/ProductionDetail_Form.cs
using ScottPlot; // ScottPlot'u kullanmak için
using System;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TekstilScada.Core;
using TekstilScada.Core.Models;
using TekstilScada.Models;
using TekstilScada.Repositories;
using ChartLegend = System.Windows.Forms.DataVisualization.Charting.Legend;
namespace TekstilScada.UI
{
    public partial class ProductionDetail_Form : Form
    {
        private readonly ProductionReportItem _reportItem;
        private readonly ProductionRepository _productionRepo;
        private readonly ProcessLogRepository _processLogRepo;
        private readonly AlarmRepository _alarmRepo; // Alarmlar için eklendi
        private readonly RecipeRepository _recipeRepository; // YENİ

        public ProductionDetail_Form(ProductionReportItem reportItem, RecipeRepository recipeRepo, ProcessLogRepository processLogRepo, AlarmRepository alarmRepo)
        {
            InitializeComponent();
            _reportItem = reportItem;
            _productionRepo = new ProductionRepository();
            _processLogRepo = processLogRepo;
            _alarmRepo = alarmRepo;
            _recipeRepository = recipeRepo;
        }

        private void ProductionDetail_Form_Load(object sender, EventArgs e)
        {
            this.Text = $"Production Report Detail - {_reportItem.BatchId}";

            // 1. Başlık bilgilerini doldur
            txtMachineName.Text = _reportItem.MachineName;
            txtRecipeName.Text = _reportItem.RecipeName;
            txtOperator.Text = _reportItem.OperatorName;
            txtStartTime.Text = _reportItem.StartTime.ToString("dd.MM.yyyy HH:mm:ss");
            txtStopTime.Text = _reportItem.EndTime.ToString("dd.MM.yyyy HH:mm:ss");
            txtTotalDuration.Text = _reportItem.CycleTime;

            // Diğer bilgileri de doldur
            txtCustomerNo.Text = _reportItem.MusteriNo;
            txtOrderNo.Text = _reportItem.SiparisNo;


            // 2. Adım detaylarını DataGridView'e yükle
            dgvStepDetails.DataSource = _productionRepo.GetProductionStepDetails(_reportItem.BatchId, _reportItem.MachineId);

            // 3. Alarm detaylarını yükle
            dgvAlarms.DataSource = _alarmRepo.GetAlarmDetailsForBatch(_reportItem.BatchId, _reportItem.MachineId);
            dgvStepDetails.CellFormatting += dgvStepDetails_CellFormatting;
            // 4. Zaman çizgisi grafiğini yükle
            // 1. Alarm ve Grafik Verilerini Yükle
        
            LoadTimelineChart(); // Mevcut zaman çizelgesi grafiğini yükle

            // 2. YENİ: Pasta Grafik Verilerini Hesapla ve Yükle
            LoadPieChart();

            
        }
        // TekstilScada/UI/ProductionDetail_Form.cs

        private void LoadPieChart()
        {
            // 1. HAZIR VERİLERİ SANİYE OLARAK AL
            double totalMachineAlarmSeconds = _reportItem.MachineAlarmDurationSeconds;
            double totalOperatorPauseSeconds = _reportItem.OperatorPauseDurationSeconds;
            double totalBatchSeconds = (_reportItem.EndTime - _reportItem.StartTime).TotalSeconds;

            // 2. AKTİF SÜREYİ SANİYE OLARAK HESAPLA
            double activeWorkingSeconds = totalBatchSeconds - totalMachineAlarmSeconds - totalOperatorPauseSeconds;
            if (activeWorkingSeconds < 0) activeWorkingSeconds = 0;

            // 3. GRAFİĞİ TEMİZLE
            pieChartControl.Series.Clear();
            pieChartControl.Legends.Clear();

            // 4. YENİ SERİYİ OLUŞTUR
            System.Windows.Forms.DataVisualization.Charting.Series series = new System.Windows.Forms.DataVisualization.Charting.Series("Time Distribution")
            {
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie,
                Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold),
                LabelForeColor = System.Drawing.Color.White,
                IsValueShownAsLabel = true
            };
            pieChartControl.Series.Add(series);

            // 5. VERİ NOKTALARINI SANİYE DEĞERLERİYLE EKLE
            if (activeWorkingSeconds > 0)
            {
                // ✅ DÜZELTME: Tam yolu belirtildi
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, activeWorkingSeconds);
                dp.Color = System.Drawing.Color.DodgerBlue; // ✅ DÜZELTME
                dp.LegendText = $"Active Work ({TimeSpan.FromSeconds(activeWorkingSeconds):hh\\:mm\\:ss})";
                series.Points.Add(dp);
            }
            if (totalMachineAlarmSeconds > 0)
            {
                // ✅ DÜZELTME: Tam yolu belirtildi
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, totalMachineAlarmSeconds);
                dp.Color = System.Drawing.Color.Crimson; // ✅ DÜZELTME
                dp.LegendText = $"Machine Alarm ({TimeSpan.FromSeconds(totalMachineAlarmSeconds):hh\\:mm\\:ss})";
                series.Points.Add(dp);
            }
            if (totalOperatorPauseSeconds > 0)
            {
                // ✅ DÜZELTME: Tam yolu belirtildi
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, totalOperatorPauseSeconds);
                dp.Color = System.Drawing.Color.Orange; // ✅ DÜZELTME
                dp.LegendText = $"Operator Pause ({TimeSpan.FromSeconds(totalOperatorPauseSeconds):hh\\:mm\\:ss})";
                series.Points.Add(dp);
            }

            if (series.Points.Count == 0)
            {
                // ✅ DÜZELTME: Tam yolu belirtildi
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, 1);
                dp.Color = System.Drawing.Color.Gray; // ✅ DÜZELTME
                dp.LegendText = "No Data";
                dp.IsValueShownAsLabel = false;
                series.Points.Add(dp);
            }

            // 6. ETİKET VE GÖSTERGELERİ AYARLA
            series.Label = "#PERCENT{P0}";

            System.Windows.Forms.DataVisualization.Charting.Legend legend = new System.Windows.Forms.DataVisualization.Charting.Legend("Süreler")
            {
                Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom,
                Alignment = System.Drawing.StringAlignment.Center,
                Font = new System.Drawing.Font("Arial", 9f)
            };
            pieChartControl.Legends.Add(legend);

            series.LegendText = "#LEGENDTEXT";

            pieChartControl.Invalidate();
        }

        // FormLoad metodundan LoadPieChart çağrısını parametresiz yap



        // dgvStepDetails_CellFormatting metodunun içine bu kodu ekleyin.

        private void dgvStepDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Sadece "Gerçekleşen Süre" (WorkingTime) sütununda çalış ve satırın boş olmadığından emin ol.
            if (e.RowIndex >= 0 && dgvStepDetails.Columns[e.ColumnIndex].Name == "WorkingTime")
            {
                // "İşleniyor..." yazan adımları atla
                if (e.Value == null || e.Value.ToString() == "Processing...")
                {
                    e.CellStyle.BackColor = dgvStepDetails.DefaultCellStyle.BackColor;
                    return;
                }

                try
                {
                    // Teorik ve Gerçekleşen süreleri TimeSpan nesnelerine çevir
                    TimeSpan theoreticalTime;
                    TimeSpan.TryParse(dgvStepDetails.Rows[e.RowIndex].Cells["TheoreticalTime"].Value.ToString(), out theoreticalTime);

                    TimeSpan workingTime;
                    TimeSpan.TryParse(e.Value.ToString(), out workingTime);

                    // Dakika bazında farkı hesapla (saniyeleri göz ardı et)
                    int theoreticalMinutes = (int)theoreticalTime.TotalMinutes;
                    int workingMinutes = (int)workingTime.TotalMinutes;

                    // Fark 1 dakikadan fazlaysa kırmızı yap
                    if (workingMinutes > theoreticalMinutes)
                    {
                        e.CellStyle.BackColor = System.Drawing.Color.LightCoral;
                        e.CellStyle.ForeColor = System.Drawing.Color.Black; // Yazı rengini siyah yap
                    }
                    // Fark -1 dakikadan azsa yeşil yap
                    else if (workingMinutes < theoreticalMinutes)
                    {
                        e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
                        e.CellStyle.ForeColor = System.Drawing.Color.Black;
                    }
                    // Fark 1 dakika içindeyse veya eşitse varsayılan renge döndür
                    else
                    {
                        e.CellStyle.BackColor = dgvStepDetails.DefaultCellStyle.BackColor;
                        e.CellStyle.ForeColor = dgvStepDetails.DefaultCellStyle.ForeColor;
                    }
                }
                catch (Exception)
                {
                    // Bir hata olursa varsayılan renkte bırak
                    e.CellStyle.BackColor = dgvStepDetails.DefaultCellStyle.BackColor;
                }
            }
        }
        private void LoadTimelineChart()
        {
            var dataPoints = _processLogRepo.GetLogsForBatch(_reportItem.MachineId, _reportItem.BatchId);
            formsPlot1.Plot.Clear();
            if (dataPoints.Any())
            {
                double[] timeData = dataPoints.Select(p => p.Timestamp.ToOADate()).ToArray();
                // DEĞİŞİKLİK: Sıcaklık verisini 10'a böl
                double[] tempData = dataPoints.Select(p => (double)p.Temperature / 10.0).ToArray();


                var tempPlot = formsPlot1.Plot.Add.Scatter(timeData, tempData);
                tempPlot.Color = ScottPlot.Colors.Red;
                tempPlot.LegendText = "Temperature";
                // 2. YENİ: Teorik Veri Grafiğini Çiz
                var productionRepo = new ProductionRepository();
                var batchRecipe = productionRepo.GetBatchRecipe(_reportItem.MachineId, _reportItem.BatchId);

                // 2. Eğer reçete verisi bulunduysa, RampCalculator'ı kullanarak teorik veriyi oluşturun.
                if (batchRecipe != null && batchRecipe.Steps.Any())
                {
                    var (theoTimestamps, theoTemperatures) = RampCalculator.GenerateTheoreticalRamp(batchRecipe, _reportItem.StartTime);

                    if (theoTimestamps.Any())
                    {
                        var theoPlot = formsPlot1.Plot.Add.Scatter(theoTimestamps, theoTemperatures);
                        theoPlot.Color = ScottPlot.Colors.Blue;
                        theoPlot.LegendText = "Theoretical Temperature";
                        theoPlot.LineStyle.Pattern = ScottPlot.LinePattern.Dashed;
                        theoPlot.LineWidth = 2;
                    }
                }

                formsPlot1.Plot.Axes.DateTimeTicksBottom();
                formsPlot1.Plot.Title($"{_reportItem.MachineName} - Process Chart");
                formsPlot1.Plot.ShowLegend(ScottPlot.Alignment.UpperLeft);
                
        // --- YENİ KOD: GRAFİĞİ OTOMATİK YAKINLAŞTIRMA ---
        // AutoScale() yerine, eksen limitlerini manuel olarak belirliyoruz.
        DateTime startTime = _reportItem.StartTime;
        // Eğer üretim bitmemişse, bitiş zamanı olarak şimdiki zamanı al
        DateTime endTime = (_reportItem.EndTime == DateTime.MinValue) ? DateTime.Now : _reportItem.EndTime;

        // Grafiğin kenarlara yapışmaması için küçük bir boşluk (marj) ekleyelim
        startTime = startTime.AddMinutes(-5);
        endTime = endTime.AddMinutes(5);

        // X ekseninin (zaman) sınırlarını ayarla
        formsPlot1.Plot.Axes.SetLimitsX(startTime.ToOADate(), endTime.ToOADate());
        // Y eksenini (sıcaklık) ise kendi verisine göre otomatik ayarlamasını söyle
        formsPlot1.Plot.Axes.AutoScaleY(); 
                formsPlot1.Refresh();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            ExcelExporter.ExportProductionDetailToExcel(_reportItem, dgvStepDetails, dgvAlarms,formsPlot1);
            //ExportProductionDetailToExcel(ProductionReportItem headerData, DataGridView dgvSteps, DataGridView dgvAlarms)
        }
    }
}