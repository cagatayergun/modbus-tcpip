using System;
using System.Drawing;
using System.Windows.Forms;
using TekstilScada.Models;

namespace TekstilScada.UI.Controls
{
    public partial class MachineStatusCard_Control : UserControl
    {
        public int MachineId { get; private set; }

        public MachineStatusCard_Control(Machine machine)
        {
            InitializeComponent();
            this.MachineId = machine.Id;
            lblMachineName.Text = machine.MachineName;
        }

        public void UpdateStatus(FullMachineStatus status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateStatus(status)));
                return;
            }

            // Temel verileri her zaman güncelle
            lblTemp.Text = $"{status.AnlikSicaklik}°C";
            lblRpm.Text = $"{status.AnlikDevirRpm} rpm";
            progressBar.Value = Math.Max(0, Math.Min(100, (int)status.ProsesYuzdesi));

            // Duruma göre renkleri ve metinleri ayarla
            if (status.HasActiveAlarm)
            {
                pnlStatus.BackColor = Color.Crimson;
                lblStatus.Text = $"ALARM: {status.ActiveAlarmNumber}";
                ClearProductionData();
            }
            else if (status.IsInRecipeMode)
            {
                pnlStatus.BackColor = Color.SeaGreen;
                lblStatus.Text = $"WORKING - Step: {status.AktifAdimNo}";
                lblWater.Text = $"{status.SuMiktari} L";
                lblRunTime.Text = $"{status.CalismaSuresiDakika} min";
            }
            else
            {
                pnlStatus.BackColor = Color.SlateGray;
                lblStatus.Text = "STANDS";
                ClearProductionData();
            }
        }

        private void ClearProductionData()
        {
            lblWater.Text = "---";
            lblRunTime.Text = "---";
        }
    }
}