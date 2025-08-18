// UI/Views/MakineDetay_Control.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Models;
using TekstilScada.Properties;
using TekstilScada.Repositories;
using TekstilScada.Services;

namespace TekstilScada.UI.Views
{
    public partial class MakineDetay_Control : UserControl
    {
        public event EventHandler BackRequested;

        private PlcPollingService _pollingService;
        private ProcessLogRepository _logRepository;
        private AlarmRepository _alarmRepository;
        private RecipeRepository _recipeRepository;
        private ProductionRepository _productionRepository;
        private Machine _machine;
        private ScottPlot.Plottables.Scatter _tempPlot;
        private ScottPlot.Plottables.Scatter _rpmPlot;
        private ScottPlot.Plottables.Scatter _waterLevelPlot; // Eğer su seviyesi çizgisini de eklediyseniz veya ekleyecekseniz
        private List<string> _currentlyDisplayedAlarms = new List<string>(); // BU SATIRI EKLEYİN

        private System.Windows.Forms.Timer _uiUpdateTimer;
        private string _lastLoadedBatchIdForChart = null; // Sadece bu değişken kalacak

        public MakineDetay_Control()
        {
            
            InitializeComponent();
            btnGeri.Click += (sender, args) => BackRequested?.Invoke(this, EventArgs.Empty);
            this.progressTemp.Paint += new System.Windows.Forms.PaintEventHandler(this.progressTemp_Paint);
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
        }

        public void InitializeControl(Machine machine, PlcPollingService service, ProcessLogRepository logRepo, AlarmRepository alarmRepo, RecipeRepository recipeRepo, ProductionRepository productionRepo)
        {
            _machine = machine;
            _pollingService = service;

            _logRepository = logRepo;
            _alarmRepository = alarmRepo;
            _recipeRepository = recipeRepo;
            _productionRepository = productionRepo;
            _uiUpdateTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _uiUpdateTimer.Tick += (sender, args) => UpdateLiveGauges();
            _uiUpdateTimer.Start();
            _pollingService.OnMachineDataRefreshed += OnDataRefreshed;
            _pollingService.OnMachineConnectionStateChanged += OnConnectionStateChanged;
            _pollingService.OnActiveAlarmStateChanged += OnAlarmStateChanged; // BU SATIRI EKLEYİN
            this.VisibleChanged += MakineDetay_Control_VisibleChanged;
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            if (_pollingService.MachineDataCache.TryGetValue(_machine.Id, out var status))
            {
                UpdateUI(status);
                UpdateAlarmList(); // İLK YÜKLEME: Alarm listesini doldur

                LoadRecipeStepsFromPlcAsync();
            }

        }
        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();

        }
        public void ApplyLocalization()
        {
            btnGeri.Text = Resources.geri;
            label1.Text = Resources.makinebilgileri;
            label2.Text = Resources.RecipeName;
            label3.Text = Resources.Operator;
            label4.Text = Resources.CustomerNo;
            label5.Text = Resources.BatchNo;
            label6.Text = Resources.OrderNo;
            lblTempTitle.Text = Resources.Temperature;
            lstAlarmlar.Text = Resources.baglantibekleniyro;


            //btnSave.Text = Resources.Save;


        }
        private void OnConnectionStateChanged(int machineId, FullMachineStatus status)
        {
            if (machineId == _machine.Id && this.IsHandleCreated && !this.IsDisposed)
            {
                this.BeginInvoke(new Action(() => UpdateUI(status)));
            }
        }

        private void OnDataRefreshed(int machineId, FullMachineStatus status)
        {
            if (machineId == _machine.Id && this.IsHandleCreated && !this.IsDisposed)
            {
                this.BeginInvoke(new Action(() => UpdateUI(status)));
            }
        }

        private void MakineDetay_Control_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && _machine != null)
            {
                _lastLoadedBatchIdForChart = null; // Sayfa göründüğünde izleyiciyi sıfırla
                if (_pollingService.MachineDataCache.TryGetValue(_machine.Id, out var status))
                {
                    UpdateUI(status);
                    UpdateAlarmList(); // GÖRÜNÜR OLDUĞUNDA: Alarm listesini doldur
                }

            }
        }

        private void UpdateLiveGauges()
        {
            if (_machine != null && _pollingService.MachineDataCache.TryGetValue(_machine.Id, out var status))
            {
                SafeInvoke(() =>
                {
                    gaugeRpm.Value = status.AnlikDevirRpm;
                    gaugeRpm.Text = status.AnlikDevirRpm.ToString();

                    // Anlık sıcaklık değerini Panel'in Tag özelliğine atıyoruz.
                    // Maksimum değeri (150) burada veya Paint metodunda sabit tutabilirsiniz,
                    // veya onu da Tag'in farklı bir parçası olarak geçirebilirsiniz.
                    // DEĞİŞİKLİK: Anlık sıcaklığı 10'a bölerek ondalıklı hale getir
                    decimal anlikSicaklikDecimal = status.AnlikSicaklik / 10.0m;
                    progressTemp.Tag = anlikSicaklikDecimal;

                    // DEĞİŞİKLİK: Label'da ondalıklı ve formatlı göster (F1 -> bir ondalık basamak)
                    lblTempValue.Text = $"{anlikSicaklikDecimal:F1} °C";
                    lblTempValue.ForeColor = GetTemperatureColor((int)anlikSicaklikDecimal); // Rengi de ondalıklı değere göre al

                 //   lblTempValue.ForeColor = GetTemperatureColor(status.AnlikSicaklik);
                    progressTemp.Invalidate(); // Panel'in Paint olayını tetikler

                    waterTankGauge1.Value = status.AnlikSuSeviyesi;
                });
            }
        }

        private void UpdateUI(FullMachineStatus status)
        {
            // 1. Temel bilgileri her zaman güncelle
            lblMakineAdi.Text = status.MachineName;
            lblOperator.Text = string.IsNullOrEmpty(status.OperatorIsmi) ? "---" : status.OperatorIsmi;
            lblReceteAdi.Text = string.IsNullOrEmpty(status.RecipeName) ? "---" : status.RecipeName;
            lblMusteriNo.Text = string.IsNullOrEmpty(status.MusteriNumarasi) ? "---" : status.MusteriNumarasi;
            lblBatchNo.Text = string.IsNullOrEmpty(status.BatchNumarasi) ? "---" : status.BatchNumarasi;
            lblSiparisNo.Text = string.IsNullOrEmpty(status.SiparisNumarasi) ? "---" : status.SiparisNumarasi;
            lblCalisanAdim.Text = $"#{status.AktifAdimNo} - {status.AktifAdimAdi}";

            // 2. Bağlantı durumunu kontrol et
            if (status.ConnectionState != ConnectionStatus.Connected)
            {
                ClearAllFieldsWithMessage($"{Resources.baglantibekleniyro}");
                return;
            }

            if (!string.IsNullOrEmpty(status.BatchNumarasi))
            {
                if (status.BatchNumarasi != _lastLoadedBatchIdForChart)
                {
                    LoadDataForBatch(status);
                }
            }
            else
            {
                if (_lastLoadedBatchIdForChart != null)
                {
                    LoadDataForLive(status);
                    UpdateAlarmList(); // Canlı moda geçildiğinde alarm listesini yenile
                }
                _lastLoadedBatchIdForChart = null;
                LoadDataForLive(status);
            }

            HighlightCurrentStep(status.AktifAdimNo);
        }

        private void LoadDataForBatch(FullMachineStatus status)
        {
            _lastLoadedBatchIdForChart = status.BatchNumarasi;

            // Partiye özel geçmiş alarmları veritabanından yükle
            var alarms = _alarmRepository.GetAlarmDetailsForBatch(status.BatchNumarasi, _machine.Id);
            var alarmStrings = alarms.Any() ? alarms.Select(a => a.AlarmDescription).ToList() : new List<string> { $"{Resources.bupartiicinalarmyok}" };

            // Geçmiş raporu görüntülerken de mevcut durumu hafızaya al
            _currentlyDisplayedAlarms = alarmStrings;
            lstAlarmlar.DataSource = _currentlyDisplayedAlarms;


            LoadTimelineChartForBatch(status.BatchNumarasi);
        }

        private void LoadDataForLive(FullMachineStatus status)
        {


            // PLC'de o an yüklü olan reçetenin adımlarını yükle


            // Son 30 dakikanın canlı grafiğini yükle
            LoadTimelineChartForLive();
        }

        private async void LoadRecipeStepsFromPlcAsync()
        {
            dgvAdimlar.DataSource = new List<object> { new { Adım = "...", Açıklama = $"{Resources.receteplcdenokunuyor}" } };

            if (_pollingService.GetPlcManagers().TryGetValue(_machine.Id, out var plcManager))
            {
                var result = await plcManager.ReadRecipeFromPlcAsync();
                if (result.IsSuccess)
                {
                    var steps = new List<ScadaRecipeStep>();
                    var rawData = result.Content;

                    if (_machine.MachineType == $"{Resources.kurutmamakinesi}")
                    {
                        var step = new ScadaRecipeStep { StepNumber = 1 };
                        Array.Copy(rawData, 0, step.StepDataWords, 0, Math.Min(rawData.Length, 6));
                        steps.Add(step);
                    }
                    else // BYMakinesi
                    {
                        for (int i = 0; i < 98; i++)
                        {
                            var step = new ScadaRecipeStep { StepNumber = i + 1 };
                            int offset = i * 25;
                            if (offset + 25 <= rawData.Length)
                            {
                                Array.Copy(rawData, offset, step.StepDataWords, 0, 25);
                                steps.Add(step);
                            }
                        }
                    }
                    dgvAdimlar.DataSource = steps.Select(s => new { Adım = s.StepNumber, Açıklama = GetStepTypeName(s) }).ToList();
                }
                else
                {
                    dgvAdimlar.DataSource = new List<object> { new { Adım = "!", Açıklama = $"{Resources.plcdenreceteokunmadı} {result.Message}" } };
                }
            }
            else
            {
                dgvAdimlar.DataSource = new List<object> { new { Adım = "!", Açıklama = $"{Resources.makinebaglantısıbulunamadı}" } };
            }
        }

        private void LoadTimelineChartForBatch(string batchId)
        {
            SafeInvoke(() =>
            {
                formsPlot1.Reset();
                var (startTime, endTime) = _productionRepository.GetBatchTimestamps(batchId, _machine.Id);

                if (!startTime.HasValue)
                {
                    formsPlot1.Plot.Title($"{Resources.partibaslangıczamanıkayip}");
                    formsPlot1.Refresh();
                    return;
                }

                DateTime effectiveEndTime = endTime ?? DateTime.Now;
                var dataPoints = _logRepository.GetLogsForDateRange(_machine.Id, startTime.Value, effectiveEndTime);

                if (!dataPoints.Any())
                {
                    formsPlot1.Plot.Title($"{Resources.bupartihenüzkaydedilmemis}");
                    formsPlot1.Refresh();
                    return;
                }

                formsPlot1.Plot.Title($"{_machine.MachineName} - ${Resources.proseszamancizgisi} ({batchId})");
                var tempPlot = formsPlot1.Plot.Add.Scatter(
                    dataPoints.Select(p => p.Timestamp.ToOADate()).ToArray(),
                   dataPoints.Select(p => (double)p.Temperature / 10.0).ToArray());

                tempPlot.Color = ScottPlot.Colors.Red;
                tempPlot.LegendText = $"{Resources.Temperature}";
                tempPlot.LineWidth = 2;

                var rpmAxis = formsPlot1.Plot.Axes.AddLeftAxis();
                rpmAxis.Label.Text = $"{Resources.devir}";
                var rpmPlot = formsPlot1.Plot.Add.Scatter(
                    dataPoints.Select(p => p.Timestamp.ToOADate()).ToArray(),
                    dataPoints.Select(p => (double)p.Rpm).ToArray());
                rpmPlot.Color = ScottPlot.Colors.Blue;
                rpmPlot.LegendText = $"{Resources.devir}";
                rpmPlot.Axes.YAxis = rpmAxis;

                // YENİ EKLENEN KOD: Su Seviyesi (Water Level) verisini grafiğe ekle
                var waterLevelAxis = formsPlot1.Plot.Axes.AddRightAxis();
                waterLevelAxis.Label.Text = $"{Resources.suseviyesi}";
                var waterLevelPlot = formsPlot1.Plot.Add.Scatter(
                    dataPoints.Select(p => p.Timestamp.ToOADate()).ToArray(),
                    dataPoints.Select(p => (double)p.WaterLevel).ToArray());
                waterLevelPlot.Color = ScottPlot.Colors.Green; // Su seviyesi için yeşil renk
                waterLevelPlot.LegendText = $"{Resources.suseviyesi}";
                waterLevelPlot.Axes.YAxis = waterLevelAxis;


                formsPlot1.Plot.Axes.DateTimeTicksBottom();
                formsPlot1.Plot.ShowLegend(ScottPlot.Alignment.UpperLeft);
                formsPlot1.Plot.Axes.AutoScale();
                formsPlot1.Refresh();
            });
        }

        // MakineDetay_Control.cs
        private void LoadTimelineChartForLive()
        {

            SafeInvoke(() =>
            {
                formsPlot1.Plot.Clear();
                DateTime endTime = DateTime.Now;
                // Son 5 saati (300 dakika) kapsayacak şekilde başlangıç zamanı
                DateTime startTime = endTime.AddMinutes(-100);

                var dataPoints = _logRepository.GetManualLogs(_machine.Id, startTime, endTime);
              
                if (!dataPoints.Any())
                {
                    formsPlot1.Plot.Clear();
                    formsPlot1.Plot.Title($"{Resources.canlidata}");
                    formsPlot1.Refresh();
                    return;
                }

                double[] timeData = dataPoints.Select(p => p.Timestamp.ToOADate()).ToArray();
                double[] tempData = dataPoints.Select(p => (double)p.Temperature / 10.0).ToArray(); // DEĞİŞİKLİK
                double[] rpmData = dataPoints.Select(p => (double)p.Rpm).ToArray();
                double[] waterLevelData = dataPoints.Select(p => (double)p.WaterLevel).ToArray();

                // Grafik nesneleri henüz oluşturulmadıysa (yani proses detay sayfası yeni açıldıysa)
                if (_tempPlot == null)
                {
                    formsPlot1.Plot.Clear(); // İlk oluşturmada her şeyi temizle
                    formsPlot1.Plot.Title($"{_machine.MachineName} - ${Resources.canliprosesdata}");
                    formsPlot1.Plot.Axes.DateTimeTicksBottom();
                    formsPlot1.Plot.ShowLegend(ScottPlot.Alignment.UpperLeft);

                    // Sıcaklık Çizgisi
                    _tempPlot = formsPlot1.Plot.Add.Scatter(timeData, tempData);
                    _tempPlot.Color = ScottPlot.Colors.Red;
                    _tempPlot.LegendText = $"{Resources.Temperature}";
                    _tempPlot.LineWidth = 2;

                    // Devir Çizgisi
                    _rpmPlot = formsPlot1.Plot.Add.Scatter(timeData, rpmData);
                    _rpmPlot.Color = ScottPlot.Colors.Blue;
                    _rpmPlot.LegendText = $"{Resources.devir}";

                    // Su Seviyesi Çizgisi
                    _waterLevelPlot = formsPlot1.Plot.Add.Scatter(timeData, waterLevelData);
                    _waterLevelPlot.Color = ScottPlot.Colors.Green;
                    _waterLevelPlot.LegendText = $"{Resources.suseviyesi}";

                    // SADECE İLK AÇILIŞTA EKSEN REFERANSLAMASI
                    // X eksenini endTime'a (şu anki zamana) göre ayarla ve geçmiş 5 saati göster
                    formsPlot1.Plot.Axes.SetLimitsX(startTime.ToOADate(), endTime.ToOADate());

                    // Y eksenlerini mevcut verilere göre otomatik ölçeklendir
                    formsPlot1.Plot.Axes.AutoScaleY();
                }
                else
                {
                    // Eğer grafik nesneleri zaten oluşturulduysa (yani sayfa açıkken sonraki güncellemeler geliyorsa)
                    // Mevcut çizgi grafiklerini kaldır
                    formsPlot1.Plot.Remove(_tempPlot);
                    formsPlot1.Plot.Remove(_rpmPlot);
                    formsPlot1.Plot.Remove(_waterLevelPlot);

                    // Yeni verilerle çizgi grafiklerini yeniden oluştur ve formsPlot1.Plot'a ekle
                    _tempPlot = formsPlot1.Plot.Add.Scatter(timeData, tempData);
                    _tempPlot.Color = ScottPlot.Colors.Red;
                    _tempPlot.LegendText = $"{Resources.Temperature}";
                    _tempPlot.LineWidth = 2;

                    _rpmPlot = formsPlot1.Plot.Add.Scatter(timeData, rpmData);
                    _rpmPlot.Color = ScottPlot.Colors.Blue;
                    _rpmPlot.LegendText = $"{Resources.devir}";

                    _waterLevelPlot = formsPlot1.Plot.Add.Scatter(timeData, waterLevelData);
                    _waterLevelPlot.Color = ScottPlot.Colors.Green;
                    _waterLevelPlot.LegendText = $"{Resources.suseviyesi}";

                    // Sonraki güncellemelerde eksen limitlerini otomatik olarak değiştirmeyin,
                    // kullanıcının yaptığı zoom ve kaydırmaları koruyun.
                    // Sadece yeni veri mevcut görünümün dışına taştığında X eksenini biraz kaydırabilirsiniz.
                    // Bu kısım, kullanıcı etkileşimini korumak için önemlidir.

                    // Opsiyonel: Eğer kullanıcı herhangi bir zoom veya kaydırma yapmadıysa ve son veri
                    // görünür alanın dışına çıktıysa, görünümü son veriye kaydırabiliriz.
                    var xRange = formsPlot1.Plot.Axes.Bottom.Range;
                    if (timeData.Any() && timeData.Last() > xRange.Max)
                    {
                        // Mevcut aralığı koruyarak sadece sonuna eklemek için
                        // formsPlot1.Plot.Axes.SetLimitsX(xRange.Min, timeData.Last() + xRange.Span * 0.05);
                        // Veya daha basitçe, tüm aralığı en yeni veriye göre güncelle:
                        formsPlot1.Plot.Axes.SetLimitsX(startTime.ToOADate(), endTime.ToOADate());
                        // Y ekseni için de benzer bir mantık düşünebilirsiniz veya AutoScaleY() çağırarak güncelleyebilirsiniz.
                        // formsPlot1.Plot.Axes.AutoScaleY(); 
                    }
                }

                formsPlot1.Refresh();
            });
        }

        private void ClearAllFieldsWithMessage(string message)
        {
            ClearBatchSpecificFieldsWithMessage(message);
            lblReceteAdi.Text = "---";
            lblOperator.Text = "---";
            lblMusteriNo.Text = "---";
            lblBatchNo.Text = "---";
            lblSiparisNo.Text = "---";
            lblCalisanAdim.Text = "---";
        }

        private void ClearBatchSpecificFieldsWithMessage(string message)
        {
            lstAlarmlar.DataSource = new List<string> { message };
            dgvAdimlar.DataSource = null;
            formsPlot1.Plot.Clear();
            formsPlot1.Plot.Title(message);
            formsPlot1.Refresh();
        }

        private void HighlightCurrentStep(int currentStepNumber)
        {
            foreach (DataGridViewRow row in dgvAdimlar.Rows)
            {
                // Önce hücrenin ve değerinin null olup olmadığını kontrol et
                if (row.Cells["Adım"] != null && row.Cells["Adım"].Value != null)
                {
                    // Güvenli çevirme için int.TryParse kullan
                    if (int.TryParse(row.Cells["Adım"].Value.ToString(), out int stepValue))
                    {
                        // Eğer çevirme başarılı olursa, mevcut adımla karşılaştır
                        if (stepValue == currentStepNumber)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                            row.DefaultCellStyle.Font = new Font(dgvAdimlar.Font, FontStyle.Bold);
                        }
                        else
                        {
                            row.DefaultCellStyle.BackColor = Color.White;
                            row.DefaultCellStyle.Font = new Font(dgvAdimlar.Font, FontStyle.Regular);
                        }
                    }
                    else
                    {
                        // Değer bir sayı değilse (örn: "...", "!"), satırı varsayılan renge boya
                        row.DefaultCellStyle.BackColor = Color.White;
                        row.DefaultCellStyle.Font = new Font(dgvAdimlar.Font, FontStyle.Regular);
                    }
                }
            }
        }

        private string GetStepTypeName(ScadaRecipeStep step)
        {
            var stepTypes = new List<string>();
            short controlWord = step.StepDataWords[24];
            if ((controlWord & 1) != 0) stepTypes.Add($"{Resources.sualma}");
            if ((controlWord & 2) != 0) stepTypes.Add($"{Resources.isitma}");
            if ((controlWord & 4) != 0) stepTypes.Add($"{Resources.calisma}");
            if ((controlWord & 8) != 0) stepTypes.Add($"{Resources.dozaj}");
            if ((controlWord & 16) != 0) stepTypes.Add($"{Resources.bosaltma}");
            if ((controlWord & 32) != 0) stepTypes.Add($"{Resources.sikma}");
            return string.Join(" + ", stepTypes);
        }

        private Color GetTemperatureColor(int temp)
        {
            if (temp < 40) return Color.DodgerBlue;
            if (temp < 60) return Color.SeaGreen;
            if (temp < 90) return Color.Orange;
            return Color.Crimson;
        }

        private void progressTemp_Paint(object sender, PaintEventArgs e)
        {
            // Sender'ı bir Panel olarak alıyoruz
            Panel barPanel = sender as Panel;
            if (barPanel == null || barPanel.Tag == null) return; // Tag kontrolü eklendi

            // Tag'den anlık değeri alıyoruz (eğer short atadıysanız short, int atadıysanız int olarak çekin)
            int currentValue = Convert.ToInt32(barPanel.Tag);
            int maximumValue = 1500; // Max değeri burada sabit tuttuk (önceki gibi 150)

            // Değerin ProgressBar aralığında olduğundan emin olalım
            currentValue = Math.Max(0, Math.Min(maximumValue, currentValue));

            int controlWidth = barPanel.Width;
            int controlHeight = barPanel.Height;

            // Tüm arka planı temizle (varsayılan çizim müdahalesi olmayacak)
            e.Graphics.FillRectangle(new SolidBrush(Color.WhiteSmoke), 0, 0, controlWidth, controlHeight);

            // Dolu olması gereken yüksekliği hesapla
            int filledHeight = (int)(controlHeight * ((double)currentValue / maximumValue));

            // Dolu alanı çizeceğimiz dikdörtgeni tanımla
            Rectangle filledRect = new Rectangle(
                0, // X başlangıcı: Kontrolün sol kenarından başla
                controlHeight - filledHeight, // Y başlangıcı: Aşağıdan yukarıya dolum için
                controlWidth, // Genişlik: Kontrolün tam genişliğini kullan
                filledHeight // Yükseklik: Hesaplanan dolu alan yüksekliği
            );

            // Dolu alanı çiz
            e.Graphics.FillRectangle(new SolidBrush(GetTemperatureColor(currentValue)), filledRect);

            // Kenarlık çiz
            using (Pen borderPen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, controlWidth - 1, controlHeight - 1);
            }
        }

        private void lblMakineAdi_Click(object sender, EventArgs e)
        {

        }

        private void SafeInvoke(Action action)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                try { this.BeginInvoke(action); }
                catch (Exception) { /* Form kapatılırken oluşabilecek hataları yoksay */ }
            }
        }
        // Bu iki yeni metodu sınıfın içine ekleyin
        private void OnAlarmStateChanged(int machineId, FullMachineStatus status)
        {
            // Sadece ilgili makinede ve form açıkken çalış
            if (machineId == _machine.Id && this.IsHandleCreated && !this.IsDisposed)
            {
                // Sadece alarm durumu değiştiğinde listeyi güncellemek için UI thread'ine güvenli bir çağrı yap
                this.BeginInvoke(new Action(UpdateAlarmList));
            }
        }

        private void UpdateAlarmList()
        {
            // Sadece canlı izleme modundaysak (geçmiş bir rapora bakmıyorsak) çalış
            if (string.IsNullOrEmpty(_lastLoadedBatchIdForChart))
            {
                var activeAlarms = _pollingService.GetActiveAlarmsForMachine(_machine.Id);
                List<string> newAlarmList;

                if (activeAlarms.Any())
                {
                    newAlarmList = activeAlarms.Select(a => $"#{a.AlarmNumber}: {a.AlarmText}").ToList();
                }
                else
                {
                    newAlarmList = new List<string> { $"{Resources.aktifalarmyok}" };
                }

                // Sadece yeni alarm listesi eskisinden farklıysa arayüzü güncelle
                if (!_currentlyDisplayedAlarms.SequenceEqual(newAlarmList))
                {
                    _currentlyDisplayedAlarms = newAlarmList;
                    lstAlarmlar.DataSource = _currentlyDisplayedAlarms;
                }
            }
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_pollingService != null)
            {
                _pollingService.OnMachineDataRefreshed -= OnDataRefreshed;
                _pollingService.OnMachineConnectionStateChanged -= OnConnectionStateChanged;
                _pollingService.OnActiveAlarmStateChanged -= OnAlarmStateChanged; // BU SATIRI EKLEYİN
            }
            _uiUpdateTimer?.Stop();
            _uiUpdateTimer?.Dispose();
            base.OnHandleDestroyed(e);
        }

        private void pnlAlarmsAndSteps_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}