// ScottPlot 5 için doğru using ifadeleri
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Localization;
using TekstilScada.Models;
using TekstilScada.Properties;
using TekstilScada.Repositories;

namespace TekstilScada.UI.Views
{
    public partial class TrendAnaliz_Control : UserControl
    {
        private MachineRepository _machineRepository;
        private ProcessLogRepository _processLogRepository;

        public TrendAnaliz_Control()
        {
            InitializeComponent();
            ApplyLocalization();
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
            // Load event'ini manuel olarak bağlayalım
            this.Load += TrendAnaliz_Control_Load;
            btnGenerateChart.Click += btnGenerateChart_Click;
        }
        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();

        }
        public void ApplyLocalization()
        {


         
            label1.Text = Resources.Baslangic_tarihi;
            label2.Text = Resources.Bitis_tarihi;
            groupBox1.Text = Resources.makineler;
            groupBox2.Text = Resources.görüntülenecek_veriler;
            chkTemperature.Text = Resources.Temperature;
            chkWaterLevel.Text = Resources.suseviyesi;
            chkRpm.Text = Resources.devir;
            btnGenerateChart.Text = Resources.grafigiolustur;
        }
        // HATA GİDERİLDİ: Eksik olan InitializeControl metodu eklendi.
        public void InitializeControl(MachineRepository machineRepo, ProcessLogRepository processLogRepo)
        {
            _machineRepository = machineRepo;
            _processLogRepository = processLogRepo;
        }

        private void TrendAnaliz_Control_Load(object sender, EventArgs e)
        {
            dtpStartTime.Value = DateTime.Now.AddHours(-1);
            dtpEndTime.Value = DateTime.Now;

            var machines = _machineRepository.GetAllMachines();
            clbMachines.DataSource = machines;
            clbMachines.DisplayMember = "DisplayInfo";
            clbMachines.ValueMember = "Id";
        }

        private void btnGenerateChart_Click(object sender, EventArgs e)
        {
            var selectedMachineIds = clbMachines.CheckedItems.OfType<Machine>().Select(m => m.Id).ToList();
            if (!selectedMachineIds.Any())
            {
                MessageBox.Show("Please select at least one machine.", "Warning");
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            try
            {
                var dataPoints = _processLogRepository.GetLogsForDateRange(dtpStartTime.Value, dtpEndTime.Value, selectedMachineIds);

                formsPlot1.Plot.Clear();

                if (dataPoints.Any())
                {
                    var groupedData = dataPoints.GroupBy(d => d.MachineId);
                    bool anyChecked = chkTemperature.Checked || chkWaterLevel.Checked || chkRpm.Checked;

                    if (!anyChecked)
                    {
                        MessageBox.Show("Please select at least one data type (Temperature, Water Level or RPM).", "Warning");
                        return;
                    }

                    foreach (var group in groupedData)
                    {
                        var machineName = (clbMachines.DataSource as List<Machine>)?.FirstOrDefault(m => m.Id == group.Key)?.MachineName ?? $"Makine {group.Key}";

                        double[] timeData = group.Select(p => p.Timestamp.ToOADate()).ToArray();

                        if (chkTemperature.Checked)
                        {
                            double[] tempData = group.Select(p => (double)p.Temperature/10).ToArray();
                            var scatter = formsPlot1.Plot.Add.Scatter(timeData, tempData);
                            scatter.LegendText = $"{machineName} - Temperature";
                            scatter.LineWidth = 2;
                        }

                        if (chkWaterLevel.Checked)
                        {
                            double[] waterData = group.Select(p => (double)p.WaterLevel).ToArray();
                            var scatter = formsPlot1.Plot.Add.Scatter(timeData, waterData);
                            scatter.LegendText = $"{machineName} - Water level";
                            scatter.LineWidth = 2;
                        }

                        if (chkRpm.Checked)
                        {
                            double[] rpmData = group.Select(p => (double)p.Rpm).ToArray();
                            var scatter = formsPlot1.Plot.Add.Scatter(timeData, rpmData);
                            scatter.LegendText = $"{machineName} - RPM";
                            scatter.LineWidth = 2;
                        }
                    }

                    formsPlot1.Plot.Axes.DateTimeTicksBottom();
                    formsPlot1.Plot.Title("Process Variables Trend Chart");
                    formsPlot1.Plot.ShowLegend();
                    formsPlot1.Plot.Axes.AutoScale();
                }
                else
                {
                    formsPlot1.Plot.Title("No data found in the selected range.");
                }

                formsPlot1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while creating the chart: {ex.Message}", "Error");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}