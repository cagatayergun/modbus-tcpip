// Bu dosyanın içeriğini tamamen aşağıdakiyle değiştirin
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TekstilScada.Models;
using TekstilScada.Repositories; // <-- BU SATIRI EKLEYİN
namespace TekstilScada.UI.Controls
{
    public partial class DashboardMachineCard_Control : UserControl
    {
        private readonly Machine _machine;
        private List<PointF> _sparklinePoints = new List<PointF>();
        private readonly Color _colorAlarm = Color.FromArgb(231, 76, 60);
        private readonly Color _colorRunning = Color.FromArgb(46, 204, 113);
        private readonly Color _colorIdle = Color.FromArgb(243, 156, 18);
        private readonly Color _colorStopped = Color.SlateGray;

        public DashboardMachineCard_Control(Machine machine)
        {
            InitializeComponent();
            _machine = machine;
            lblMachineName.Text = _machine.MachineName;

            // Daha akıcı çizim için Double Buffering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            // YENİ DÜZENLEME: Tüm etiketlerin metin rengini siyah yapıyoruz.
            lblMachineName.ForeColor = Color.Black;
            lblRecipeName.ForeColor = Color.Black;
            lblBatchId.ForeColor = Color.Black;
            lblTemperature.ForeColor = Color.Black;
            gaugeRpm.ForeColor = Color.Black;
        }

        public void UpdateData(FullMachineStatus status, List<ProcessDataPoint> trendData)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateData(status, trendData)));
                return;
            }

            lblRecipeName.Text = $"Recipe: {status.RecipeName ?? "-"}";
            lblBatchId.Text = $"Party: {status.BatchNumarasi ?? "-"}";
            lblTemperature.Text = $"{status.AnlikSicaklik / 10.0m}°C";
            gaugeRpm.Value = status.AnlikDevirRpm;
            gaugeRpm.Text = status.AnlikDevirRpm.ToString();

            // Durum ve Renk Ayarları
            if (status.HasActiveAlarm)
            {
                pnlStatusIndicator.BackColor = _colorAlarm;
                lblStatus.Text = $"ALARM #{status.ActiveAlarmNumber}";
                lblStatus.ForeColor = _colorAlarm;
            }
            else if (status.IsInRecipeMode)
            {
                pnlStatusIndicator.BackColor = _colorRunning;
                lblStatus.Text = $"Working - Step {status.AktifAdimNo}";
                lblStatus.ForeColor = _colorRunning;
            }
            else
            {
                pnlStatusIndicator.BackColor = _colorStopped;
                lblStatus.Text = "Stops";
                lblStatus.ForeColor = _colorStopped;
            }

            // Sparkline verisini güncelle
            UpdateSparkline(trendData);
        }

        private void UpdateSparkline(List<ProcessDataPoint> trendData)
        {
            _sparklinePoints.Clear();
            if (trendData == null || trendData.Count < 2)
            {
                pnlSparkline.Invalidate();
                return;
            }

            var minTime = trendData.First().Timestamp;
            var maxTime = trendData.Last().Timestamp;
            var timeRange = (maxTime - minTime).TotalSeconds;
            if (timeRange == 0) timeRange = 1;

            var minTemp = trendData.Min(p => p.Temperature / 10.0m);
            var maxTemp = trendData.Max(p => p.Temperature / 10.0m);
            var tempRange = maxTemp - minTemp;
            if (tempRange == 0) tempRange = 1;

            foreach (var p in trendData)
            {
                float x = (float)(((p.Timestamp - minTime).TotalSeconds / timeRange) * pnlSparkline.Width);
                float y = (float)(pnlSparkline.Height - ((p.Temperature / 10.0m - minTemp) / tempRange) * pnlSparkline.Height);
                _sparklinePoints.Add(new PointF(x, y));
            }
            pnlSparkline.Invalidate(); // Paneli yeniden çizdir
        }

        private void pnlSparkline_Paint(object sender, PaintEventArgs e)
        {
            if (_sparklinePoints.Count < 2) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(_colorRunning, 2))
            {
                e.Graphics.DrawLines(pen, _sparklinePoints.ToArray());
            }
        }
    }
}