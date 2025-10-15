// UI/Views/MachineSettings_Control.cs
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Models;
using TekstilScada.Properties;
using TekstilScada.Repositories;
using TekstilScada.Services;

namespace TekstilScada.UI.Views
{
    public partial class MachineSettings_Control : UserControl
    {
        public event EventHandler MachineListChanged;
        private readonly UserRepository _userRepository;
        private readonly MachineRepository _repository;
        private List<TekstilScada.Models.Machine> _machines;
        private TekstilScada.Models.Machine _selectedMachine;
        private List<object> _machineTypeOptions;
        public MachineSettings_Control()
        {
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
            InitializeComponent();
            _repository = new MachineRepository();
            _userRepository = new UserRepository();
        }
        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();
            PopulateMachineTypeComboBox();

        }
        public void ApplyLocalization()
        {
            groupBox1.Text = Resources.makinebilgileri;
            label1.Text = Resources.MachineID;
            label2.Text = Resources.MachineName;
            label3.Text = Resources.ipadres;
            label5.Text = Resources.vncadres;
            chkIsEnabled.Text = Resources.izlemeaktif;
            label6.Text = Resources.makinegrup;
            label9.Text = Resources.makinetip;
            label7.Text = Resources.ftpuser;
            label8.Text = Resources.ftppass;
            btnDelete.Text = Resources.Delete;
            btnNew.Text = Resources.New;
            btnSave.Text = Resources.Save;
            //btnSave.Text = Resources.Save;


        }
        private void MachineSettings_Control_Load(object sender, EventArgs e)
        {

            // ComboBox'ı ilk defa doldur ve makineleri listele.
            PopulateMachineTypeComboBox();
            RefreshMachineList();
            ApplyLocalization(); // Metinleri ilk yüklemede de uygula
        }
        private void PopulateMachineTypeComboBox()
        {
            // 1. Dilden bağımsız anahtarları ("Value") ve çevrilmiş metinleri ("Display") içeren listeyi oluştur.
            _machineTypeOptions = new List<object>
            {
                new { Display = Resources.bymakinesi,       Value = "BYMakinesi" },
                new { Display = Resources.kurutmamakinesi,  Value = "Kurutma Makinesi" }
            };

            // 2. ComboBox'ı bu listeye bağla.
            cmbMachineType.DataSource = null; // Önceki bağlantıyı temizle
            cmbMachineType.DataSource = _machineTypeOptions;
            cmbMachineType.DisplayMember = "Display"; // Kullanıcı çevrilmiş metni görecek
            cmbMachineType.ValueMember = "Value";     // Arka planda dilden bağımsız anahtar tutulacak
        }
        public void RefreshMachineList()
        {
            try
            {
                _machines = _repository.GetAllMachines();
                dgvMachines.DataSource = null;
                dgvMachines.DataSource = _machines;
                if (dgvMachines.Columns["Id"] != null) dgvMachines.Columns["Id"].Visible = false;
                // GEREKSİZ SÜTUNLARI GİZLE
                if (dgvMachines.Columns["VncPassword"] != null) dgvMachines.Columns["VncPassword"].Visible = false;
                if (dgvMachines.Columns["FtpPassword"] != null) dgvMachines.Columns["FtpPassword"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Resources.makineyüklemehatası} {ex.Message}", $"{Resources.DatabaseError}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvMachines_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMachines.SelectedRows.Count > 0)
            {
                _selectedMachine = dgvMachines.SelectedRows[0].DataBoundItem as TekstilScada.Models.Machine;
                if (_selectedMachine != null)
                {
                    PopulateFields(_selectedMachine);
                }
            }
        }

        private void PopulateFields(TekstilScada.Models.Machine machine)
        {
            txtMachineId.Text = machine.MachineUserDefinedId;
            txtMachineName.Text = machine.MachineName;
            txtIpAddress.Text = machine.IpAddress;
            txtPort.Text = machine.Port.ToString();
            txtVncAddress.Text = machine.VncAddress;
            chkIsEnabled.Checked = machine.IsEnabled;
            cmbMachineType.SelectedValue = machine.MachineType;
            // YENİ: FTP alanlarını doldur
            txtFtpUsername.Text = machine.FtpUsername;
            txtFtpPassword.Text = machine.FtpPassword;
            txtMachineSubType.Text = machine.MachineSubType;
        }

        private void ClearFields()
        {
            _selectedMachine = null;
            dgvMachines.ClearSelection();
            txtMachineId.Text = "";
            txtMachineName.Text = "";
            txtIpAddress.Text = "";
            txtPort.Text = "502";
            txtVncAddress.Text = "";
            chkIsEnabled.Checked = true;
            cmbMachineType.SelectedIndex = 0;
            // YENİ: FTP alanlarını temizle
            txtFtpUsername.Text = "";
            txtFtpPassword.Text = "";
            txtMachineSubType.Text = "";
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            // Olayı geçici olarak devre dışı bırak
            dgvMachines.SelectionChanged -= dgvMachines_SelectionChanged;

            ClearFields();

            // Olayı tekrar etkinleştir
            dgvMachines.SelectionChanged += dgvMachines_SelectionChanged;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMachineId.Text) || string.IsNullOrWhiteSpace(txtIpAddress.Text) || string.IsNullOrWhiteSpace(txtMachineSubType.Text))
            {
                MessageBox.Show($"{Resources.makineidveipzorunlu}]", $"{Resources.EksikBilgi}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_selectedMachine == null) // Yeni Kayıt
                {
                    var newMachine = new TekstilScada.Models.Machine
                    {
                        MachineUserDefinedId = txtMachineId.Text,
                        MachineName = txtMachineName.Text,
                        IpAddress = txtIpAddress.Text,
                        Port = int.Parse(txtPort.Text),
                        VncAddress = txtVncAddress.Text,
                        IsEnabled = chkIsEnabled.Checked,
                        MachineType = cmbMachineType.SelectedValue.ToString(),
                        // YENİ: FTP alanlarını oku
                        FtpUsername = txtFtpUsername.Text,
                        FtpPassword = txtFtpPassword.Text,
                        MachineSubType = txtMachineSubType.Text
                    }; 
                    _repository.AddMachine(newMachine);
                    MessageBox.Show($"{Resources.yenimakinebasarili}", $"{Resources.Confirim}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (CurrentUser.IsLoggedIn)
                    {
                        _userRepository.LogAction(CurrentUser.User.Id, "Machine Settings", $"'{newMachine.MachineName}' Added new machine called.");
                    }
                }
                else // Güncelleme
                {
                    _selectedMachine.MachineUserDefinedId = txtMachineId.Text;
                    _selectedMachine.MachineName = txtMachineName.Text;
                    _selectedMachine.IpAddress = txtIpAddress.Text;
                    _selectedMachine.Port = int.Parse(txtPort.Text);
                    _selectedMachine.VncAddress = txtVncAddress.Text;
                    _selectedMachine.IsEnabled = chkIsEnabled.Checked;
                    _selectedMachine.MachineType = cmbMachineType.SelectedValue.ToString();
                    // YENİ: FTP alanlarını oku
                    _selectedMachine.FtpUsername = txtFtpUsername.Text;
                    _selectedMachine.FtpPassword = txtFtpPassword.Text;
                    _selectedMachine.MachineSubType = txtMachineSubType.Text;
                    _repository.UpdateMachine(_selectedMachine);
                  
                    MessageBox.Show($"{Resources.makinebilgilerigüncellendi}", $"{Resources.Confirim}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (CurrentUser.IsLoggedIn)
                    {
                        _userRepository.LogAction(CurrentUser.User.Id, "Machine Settings", $"The settings for the machine '{_selectedMachine.MachineName}' have been updated.");
                    }
                }
                dgvMachines.SelectionChanged -= dgvMachines_SelectionChanged;
                RefreshMachineList();
                ClearFields();
                dgvMachines.SelectionChanged += dgvMachines_SelectionChanged;
                MachineListChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Resources.kayitsirasihatasi} {ex.Message}", $"{Resources.Error}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedMachine == null)
            {
                MessageBox.Show($"{Resources.lütfensilmekicinmakinesec}", $"{Resources.Warning}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"'{_selectedMachine.MachineName}' ${Resources.makinesilmeeminmisin}", $"{Resources.silmeonayı}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _repository.DeleteMachine(_selectedMachine.Id);
                    RefreshMachineList();
                    ClearFields();
                    MachineListChanged?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Resources.Silmesırasındahata} {ex.Message}", $"{Resources.Error}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
