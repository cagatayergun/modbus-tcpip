// UIViews/GenelUretimRaporu_Control.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Models;
using TekstilScada.Properties;
using TekstilScada.Repositories;

namespace TekstilScada.UI.Views
{
    public partial class GenelUretimRaporu_Control : UserControl
    {
        private MachineRepository _machineRepository;
        private ProductionRepository _productionRepository;
        // YENİ: CostRepository alanını ekleyin
        private CostRepository _costRepository;
        private DataTable _reportData;
        private ListBox _activeSourceListBox;

        public GenelUretimRaporu_Control()
        {
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
            InitializeComponent();
        }

        // GÜNCELLENDİ: CostRepository parametresini ekleyin
        public void InitializeControl(MachineRepository machineRepo, ProductionRepository productionRepo, CostRepository costRepo)
        {
            _machineRepository = machineRepo;
            _productionRepository = productionRepo;
            _costRepository = costRepo; // YENİ: Atama işlemi
        }

        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();
        }

        public void ApplyLocalization()
        {
            groupBox1.Text = Resources.tüketimtipi;
            btnRaporOlustur.Text = Resources.Reports;
            radioElektrik.Text = Resources.elk;
            radioBuhar.Text = Resources.buhar;
            radioSu.Text = Resources.su;
        }

        private void GenelUretimRaporu_Control_Load(object sender, EventArgs e)
        {
            dtpStartTime.Value = DateTime.Today;
            dtpEndTime.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            LoadMachineLists();
        }

        private void LoadMachineLists()
        {
            var allMachines = _machineRepository.GetAllMachines();
            var groupedMachines = allMachines
                .Where(m => !string.IsNullOrEmpty(m.MachineSubType))
                .GroupBy(m => m.MachineSubType);
            flpMachineGroups.Controls.Clear();
            foreach (var group in groupedMachines)
            {
                var groupBox = new GroupBox
                {
                    Text = group.Key,
                    Width = 250,
                    Height = 220
                };
                var listBox = new ListBox
                {
                    DataSource = group.ToList(),
                    DisplayMember = "MachineName",
                    Dock = DockStyle.Fill,
                    SelectionMode = SelectionMode.MultiExtended
                };
                listBox.Enter += (s, a) => { _activeSourceListBox = s as ListBox; };
                listBox.DoubleClick += (s, a) => { AddSelectedItems(); };
                groupBox.Controls.Add(listBox);
                flpMachineGroups.Controls.Add(groupBox);
            }
        }

        private void btnRaporOlustur_Click(object sender, EventArgs e)
        {
            var selectedMachines = listBoxSeciliMakineler.Items.Cast<Machine>().Select(m => m.MachineName).ToList();
            if (!selectedMachines.Any())
            {
                MessageBox.Show($"{Resources.lütfenbirmakinesec}", $"{Resources.Warning}");
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                _reportData = _productionRepository.GetGeneralProductionReport(dtpStartTime.Value, dtpEndTime.Value, selectedMachines);

                // YENİ: Maliyet hesaplamalarını ekle
                CalculateAndAddCostToReport();

                dgvReport.DataSource = _reportData;
                FilterGridByConsumptionType();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Resources.raporolusturukenhata}: {ex.Message}", $"{Resources.Error}");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // YENİ: Maliyet hesaplayan ve tabloya ekleyen metot
        private void CalculateAndAddCostToReport()
        {
            if (_reportData == null || _reportData.Rows.Count == 0 || _costRepository == null) return;

            var costParams = _costRepository.GetAllParameters();
            var waterCostParam = costParams.FirstOrDefault(p => p.ParameterName == "Water");
            var electricityCostParam = costParams.FirstOrDefault(p => p.ParameterName == "Electricity");
            var steamCostParam = costParams.FirstOrDefault(p => p.ParameterName == "Steam");

            if (waterCostParam == null || electricityCostParam == null || steamCostParam == null)
            {
                MessageBox.Show("Cost parameters are missing. Please check the Settings -> Cost Parameters screen.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Yeni bir sütun ekle
            _reportData.Columns.Add("Cost", typeof(decimal));

            foreach (DataRow row in _reportData.Rows)
            {
                decimal totalWater = row["TotalWater"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalWater"]);
                decimal totalElectricity = row["TotalElectricity"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalElectricity"]);
                decimal totalSteam = row["TotalSteam"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalSteam"]);

                decimal totalCost = 0;

                // Sadece seçili tüketim tipine göre maliyeti hesapla
                if (radioSu.Checked)
                {
                    totalCost = totalWater * waterCostParam.CostValue * waterCostParam.Multiplier;
                }
                else if (radioElektrik.Checked)
                {
                    totalCost = totalElectricity * electricityCostParam.CostValue * electricityCostParam.Multiplier;
                }
                else if (radioBuhar.Checked)
                {
                    totalCost = totalSteam * steamCostParam.CostValue * steamCostParam.Multiplier;
                }

                row["Cost"] = totalCost;
            }
        }

        private void radioConsumption_CheckedChanged(object sender, EventArgs e)
        {
            FilterGridByConsumptionType();
        }

        private void FilterGridByConsumptionType()
        {
            if (dgvReport.DataSource == null || dgvReport.Columns.Count == 0) return;

            // Önce tüm tüketim ve maliyet kolonlarını gizle
            if (dgvReport.Columns.Contains("TotalWater")) dgvReport.Columns["TotalWater"].Visible = false;
            if (dgvReport.Columns.Contains("TotalElectricity")) dgvReport.Columns["TotalElectricity"].Visible = false;
            if (dgvReport.Columns.Contains("TotalSteam")) dgvReport.Columns["TotalSteam"].Visible = false;
            if (dgvReport.Columns.Contains("Cost")) dgvReport.Columns["Cost"].Visible = false;

            // Sadece seçili olanı göster
            if (radioSu.Checked && dgvReport.Columns.Contains("TotalWater"))
            {
                dgvReport.Columns["TotalWater"].Visible = true;
                if (dgvReport.Columns.Contains("Cost")) dgvReport.Columns["Cost"].Visible = true;
            }
            if (radioElektrik.Checked && dgvReport.Columns.Contains("TotalElectricity"))
            {
                dgvReport.Columns["TotalElectricity"].Visible = true;
                if (dgvReport.Columns.Contains("Cost")) dgvReport.Columns["Cost"].Visible = true;
            }
            if (radioBuhar.Checked && dgvReport.Columns.Contains("TotalSteam"))
            {
                dgvReport.Columns["TotalSteam"].Visible = true;
                if (dgvReport.Columns.Contains("Cost")) dgvReport.Columns["Cost"].Visible = true;
            }
        }

        private void AddSelectedItems()
        {
            if (_activeSourceListBox == null || _activeSourceListBox.SelectedItems.Count == 0) return;
            foreach (var item in _activeSourceListBox.SelectedItems)
            {
                if (!listBoxSeciliMakineler.Items.Contains(item))
                {
                    listBoxSeciliMakineler.Items.Add(item);
                }
            }
            listBoxSeciliMakineler.DisplayMember = "DisplayInfo";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddSelectedItems();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            for (int i = listBoxSeciliMakineler.SelectedIndices.Count - 1; i >= 0; i--)
            {
                listBoxSeciliMakineler.Items.RemoveAt(listBoxSeciliMakineler.SelectedIndices[i]);
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            listBoxSeciliMakineler.Items.Clear();
            foreach (var groupBox in flpMachineGroups.Controls.OfType<GroupBox>())
            {
                var listBox = groupBox.Controls.OfType<ListBox>().FirstOrDefault();
                if (listBox != null)
                {
                    foreach (var item in listBox.Items)
                    {
                        if (!listBoxSeciliMakineler.Items.Contains(item))
                        {
                            listBoxSeciliMakineler.Items.Add(item);
                        }
                    }
                }
            }
            listBoxSeciliMakineler.DisplayMember = "DisplayInfo";
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            listBoxSeciliMakineler.Items.Clear();
        }
    }
}