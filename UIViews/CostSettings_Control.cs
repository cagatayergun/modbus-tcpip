using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Models;
using TekstilScada.Properties;
using TekstilScada.Repositories;

namespace TekstilScada.UI.Views
{
    public partial class CostSettings_Control : UserControl
    {
        private readonly CostRepository _repository;
        private List<CostParameter> _parameters;

        public CostSettings_Control()
        {
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
            InitializeComponent();
            _repository = new CostRepository();
        }

        private void CostSettings_Control_Load(object sender, EventArgs e)
        {
            LoadParameters();
        }
        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();

        }
        public void ApplyLocalization()
        {
            btnSave.Text = Resources.Save;
           
            //btnSave.Text = Resources.Save;


        }
        private void LoadParameters()
        {
            try
            {
                _parameters = _repository.GetAllParameters();
                dgvCostParameters.DataSource = _parameters;
                if (dgvCostParameters.Columns["Id"] != null) dgvCostParameters.Columns["Id"].Visible = false;

                // YENİ: Kolon başlıklarını ve formatlarını düzenle
                if (dgvCostParameters.Columns["ParameterName"] != null) dgvCostParameters.Columns["ParameterName"].HeaderText = "Parameter";
                if (dgvCostParameters.Columns["CostValue"] != null) dgvCostParameters.Columns["CostValue"].HeaderText = "Unit Cost";
                if (dgvCostParameters.Columns["Unit"] != null) dgvCostParameters.Columns["Unit"].HeaderText = "Unit";
                if (dgvCostParameters.Columns["Multiplier"] != null) dgvCostParameters.Columns["Multiplier"].HeaderText = "Multiplier";
                if (dgvCostParameters.Columns["CurrencySymbol"] != null) dgvCostParameters.Columns["CurrencySymbol"].HeaderText = "Currency";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cost parameters: {ex.Message}", "Database Error");
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Değişiklikleri DataGridView'den al
                var updatedParameters = (List<CostParameter>)dgvCostParameters.DataSource;
                _repository.UpdateParameters(updatedParameters);
                MessageBox.Show("Cost parameters updated successfully.", "Successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving parameters: {ex.Message}", "Error");
            }
        }
    }
}