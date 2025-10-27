using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Models;
using TekstilScada.Repositories;
using TekstilScada.Services;
using TekstilScada.UI.Controls;
using TekstilScada.UI.Controls.RecipeStepEditors;
using TekstilScada.UI.Views;
using TekstilScada.Core.Models;
namespace TekstilScada.UI
{
    public partial class FtpSync_Form : Form
    {
        private readonly MachineRepository _machineRepository;
        private readonly RecipeRepository _recipeRepository;
        private readonly FtpTransferService _transferService;

        // Ön izleme editörü için gerekli değişkenler
        private SplitContainer _byMakinesiEditor;
        private DataGridView dgvRecipeSteps;
        private Panel pnlStepDetails;
        private ScadaRecipe _previewRecipe;
        private readonly string _targetMachineType;
        private readonly PlcPollingService _plcPollingService; // <-- YENİ DEĞİŞKENİ EKLEYİN
        public FtpSync_Form(MachineRepository machineRepo, RecipeRepository recipeRepo, PlcPollingService plcPollingService, string targetMachineType, FtpTransferService transferService)
        {
            InitializeComponent();
            _machineRepository = machineRepo;
            _recipeRepository = recipeRepo;
            _targetMachineType = targetMachineType;
            _transferService = transferService; // DÜZELTME: Dışarıdan gelen nesneyi kullanın.
            _transferService.SetSyncContext(SynchronizationContext.Current);
            _plcPollingService = plcPollingService;
        }

        private void FtpSync_Form_Load(object sender, EventArgs e)
        {
            LoadMachines();
            LoadLocalRecipes();
            SetupTransfersGrid();
            dgvTransfers.DataSource = _transferService.Jobs;
            _transferService.Jobs.ListChanged += Jobs_ListChanged;
        }

        private void LoadMachines()
        {
            // 1. Servisin anlık veri önbelleğini al.
            var machineCache = _plcPollingService.MachineDataCache;

            // 2. Makineleri veritabanından çekerken bu önbelleğe göre filtrele.
            var machines = _machineRepository.GetAllEnabledMachines()
                .Where(m =>
                    // Mevcut Filtrelerin
                    !string.IsNullOrEmpty(m.FtpUsername) &&
                    m.MachineType != "Kurutma Makinesi" &&
                    (!string.IsNullOrEmpty(m.MachineSubType) ? m.MachineSubType : m.MachineType) == _targetMachineType &&

                    // NİHAİ VE KAYNAK KODUNA UYGUN KONTROL:
                    // Bu makinenin kaydı Cache'de var mı? VE
                    // Bu kaydın içindeki anlık bağlantı durumu "Connected" mı?
                    machineCache.TryGetValue(m.Id, out FullMachineStatus status) && status.ConnectionState == ConnectionStatus.Connected
                )
                .ToList();

            ((ListBox)clbMachines).DataSource = machines;
            ((ListBox)clbMachines).DisplayMember = "DisplayInfo";
            ((ListBox)clbMachines).ValueMember = "Id";
        }

        private void LoadLocalRecipes()
        {
            // KRİTİK ADIM 1: Yeni veri yüklenmeden önce mevcut seçimi ve indeksi temizle
            if (lstLocalRecipes.Items.Count > 0)
            {
                lstLocalRecipes.SelectedIndex = -1;
                lstLocalRecipes.ClearSelected();
            }

            // Reçeteleri seçilen makine tipine göre filtrele
            lstLocalRecipes.DataSource = _recipeRepository.GetAllRecipes()
                .Where(r => r.TargetMachineType == _targetMachineType)
                .ToList();
            lstLocalRecipes.DisplayMember = "RecipeName";
            lstLocalRecipes.ValueMember = "Id";

            // KRİTİK ADIM 2: DataSource atandıktan sonra seçimi tekrar sıfırla
            lstLocalRecipes.SelectedIndex = -1;
        }

        // FtpSync_Form.cs
        // FtpSync_Form.cs
        private async void LoadHmiRecipes()
        {
            var selectedMachine = clbMachines.CheckedItems.Count == 1
                ? clbMachines.CheckedItems.Cast<Machine>().FirstOrDefault()
                : null;

            if (selectedMachine == null)
            {
                lstHmiRecipes.DataSource = null;
                btnReceive.Enabled = false;
                ClearPreview();
                return;
            }

            btnReceive.Enabled = true;

            btnRefreshHmi.Enabled = false;
            lstHmiRecipes.DataSource = new List<string> { "Prescription names are read from the PLC..." };
            ClearPreview();

            try
            {
                if (!_plcPollingService.GetPlcManagers().TryGetValue(selectedMachine.Id, out var plcManager))
                {
                    throw new Exception("PLC manager for the machine could not be found.");
                }

                var readResult = await plcManager.ReadRecipeNamesFromPlcAsync();
                if (readResult.IsSuccess)
                {
                    var recipeNames = readResult.Content;
                    var displayList = new List<string>();
                    foreach (var kvp in recipeNames)
                    {
                        displayList.Add($"{kvp.Key} - {kvp.Value}");
                    }

                    if (!displayList.Any())
                    {
                        displayList.Add("No prescription name registered in PLC found.");
                    }

                    lstHmiRecipes.DataSource = displayList;
                    lstHmiRecipes.ClearSelected(); // Clear selection after loading new data
                }
                else
                {
                    throw new Exception(readResult.Message);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"An unexpected error occurred: {ex.Message}";
                MessageBox.Show(errorMessage, "General Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lstHmiRecipes.DataSource = null;
            }
            finally
            {
                btnRefreshHmi.Enabled = true;
            }
        }

        private void SetupTransfersGrid()
        {
            dgvTransfers.AutoGenerateColumns = false;
            dgvTransfers.Columns.Clear();
            dgvTransfers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MachineName", HeaderText = "Machine", FillWeight = 150 });
            dgvTransfers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RecipeName", HeaderText = "Recipe/File", FillWeight = 200 });
            dgvTransfers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OperationType", HeaderText = "Operation" });
            dgvTransfers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status" });
            dgvTransfers.Columns.Add(new DataGridViewProgressBarColumn { DataPropertyName = "Progress", HeaderText = "Progress" });
            dgvTransfers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ErrorMessage", HeaderText = "Error", FillWeight = 250 });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRecipes = lstLocalRecipes.SelectedItems.OfType<ScadaRecipe>().ToList();
                var selectedMachines = clbMachines.CheckedItems.Cast<Machine>().ToList();

                if (!selectedRecipes.Any() || !selectedMachines.Any())
                {
                    MessageBox.Show("Please select at least one recipe and one target machine.", "Warning");
                    return;
                }

                // Kullanıcıdan başlangıç numarasını al
                string startNumberStr = ProsesKontrol_Control.ShowInputDialog("Enter the first prescription number to be sent (1-98):", true);
                if (string.IsNullOrEmpty(startNumberStr) || !int.TryParse(startNumberStr, out int startNumber))
                {
                    return; // Kullanıcı iptal etti veya geçersiz giriş yaptı
                }

                // Seçilen reçete sayısı, kalan numaralara sığıyor mu kontrol et
                if (startNumber + selectedRecipes.Count - 1 > 98)
                {
                    MessageBox.Show($"The {selectedRecipes.Count} number of recipes you selected exceeds the limit of 98 with a starting number of {startNumber}. Please select a lower starting number.", "Error");
                    return;
                }

                // Yeni servis metodunu çağır
                _transferService.QueueSequentiallyNamedSendJobs(selectedRecipes, selectedMachines, startNumber);
            }
            catch (Exception ex)
            {
                // Gönderim veya UI işleme sırasında oluşan diğer hataları yakalar
                MessageBox.Show($"Gönderim sırasında beklenmeyen bir hata oluştu: {ex.Message}", "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            var selectedMachines = clbMachines.CheckedItems.Cast<Machine>().ToList();

            if (!selectedMachines.Any() || selectedMachines.Count > 1)
            {
                MessageBox.Show("Please select ONLY ONE source machine from the list.", "Warning");
                return;
            }

            var selectedMachine = selectedMachines.First();
            var selectedIndices = lstHmiRecipes.SelectedIndices;

            if (selectedIndices.Count == 0)
            {
                MessageBox.Show("Please select at least one HMI recipe to download.", "Warning");
                return;
            }

            // Seçilen her bir öğenin indeksine göre doğru dosya adlarını oluştur.
            var filesToReceive = new List<string>();
            foreach (int index in selectedIndices)
            {
                // ListBox indeksi 0'dan başladığı için, reçete numarası için 1 ekliyoruz.
                int recipeNumber = index + 1;
                string remoteFileName = $"XPR{recipeNumber:D5}.csv";
                filesToReceive.Add(remoteFileName);
            }

            // FtpTransferService'e doğru dosya adlarıyla kuyruğa ekleme emrini ver.
            _transferService.QueueReceiveJobs(filesToReceive, selectedMachine);
        }

        private void btnRefreshHmi_Click(object sender, EventArgs e)
        {
            LoadHmiRecipes();
        }

        private void clbMachines_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate { LoadHmiRecipes(); });
        }

        private void Jobs_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Hata oluşma potansiyeli olan kısmı ana UI thread'ine yönlendiriyoruz
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => Jobs_ListChanged(sender, e)));
                return;
            }

            // --- Bu noktadan sonrası artık UI Thread'de çalışır ---

            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                var job = _transferService.Jobs[e.NewIndex] as TransferJob;
                if (job != null && job.OperationType == TransferType.Send && job.Status == TransferStatus.Successful)
                {
                    // LoadLocalRecipes() UI kontrolünü güncellediği için artık güvenli
                    LoadLocalRecipes();
                }
            }

            if (this.IsDisposed || !this.IsHandleCreated) return;

            // dgvTransfers.Refresh() işlemi de UI Thread'de olmalıdır, ancak Jobs_ListChanged
            // metodunun başında Invoke kontrolü eklendiği için aşağıdaki özel Invoke kontrolü gereksiz hale gelir.

            // DÜZELTME: Güvenli olması için InvokeRequired kontrolü kaldırıldı, en yukarıdaki Invoke kontrolü yeterlidir.
            dgvTransfers.Refresh();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _transferService.Jobs.ListChanged -= Jobs_ListChanged;
            base.OnFormClosing(e);
        }

        #region Ön İzleme Metotları

        // FtpSync_Form.cs
        // ...
        // FtpSync_Form.cs

        // ... Diğer metotlarınız ...

        // FtpSync_Form.cs
        // ...

        private async void lstHmiRecipes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstHmiRecipes.SelectedItems.Count != 1 || lstHmiRecipes.SelectedIndex < 0)
            {
                ClearPreview();
                return;
            }

            var selectedMachine = clbMachines.CheckedItems.Cast<Machine>().FirstOrDefault();
            if (selectedMachine == null)
            {
                ClearPreview();
                return;
            }

            // Reçete numarasını ListBox'taki sırasına göre alıyoruz.
            // İndeks 0'dan başladığı için 1 ekliyoruz.
            int recipeNumber = lstHmiRecipes.SelectedIndex + 1;

            // FTP dosya adını bu numaraya göre oluşturuyoruz.
            string remoteFileName = $"XPR{recipeNumber:D5}.csv";

            tabControlMain.SelectedTab = tabPagePreview;
            pnlPreviewArea.Controls.Clear();
            lblPreviewStatus.Visible = true;
            lblPreviewStatus.Text = $"'{remoteFileName}' yükleniyor...";

            try
            {
                var ftpService = new FtpService(selectedMachine.VncAddress, selectedMachine.FtpUsername, selectedMachine.FtpPassword);
                string csvContent = await ftpService.DownloadFileAsync($"/{remoteFileName}");

                string previewName = lstHmiRecipes.SelectedItem.ToString();
                _previewRecipe = RecipeCsvConverter.ToRecipe(csvContent, previewName);

                lblPreviewStatus.Visible = false;
                InitializeBYMakinesiEditor(previewName);
                PopulateStepsGridView();
                pnlPreviewArea.Controls.Add(_byMakinesiEditor);
            }
            catch (Exception ex)
            {
                lblPreviewStatus.Text = $"Ön izleme yüklenemedi: {ex.Message}";
            }
        }
        // ...

        // ... Diğer metotlarınız ...
        // ...

        private void ClearPreview()
        {
            pnlPreviewArea.Controls.Clear();
            pnlPreviewArea.Controls.Add(lblPreviewStatus);
            lblPreviewStatus.Visible = true;
            lblPreviewStatus.Text = "Ön izleme için HMI listesinden bir reçete seçin.";
            _previewRecipe = null;
        }

        private string GeneratePreviewRecipeName(Machine machine, string fileName, ScadaRecipe recipe)
        {
            string machineName = machine.MachineName;
            string recipeNumberPart = "0";
            try
            {
                string fName = Path.GetFileNameWithoutExtension(fileName);
                Match match = Regex.Match(fName, @"\d+$");
                if (match.Success)
                {
                    recipeNumberPart = int.Parse(match.Value).ToString();
                }
            }
            catch { recipeNumberPart = "NO_HATA"; }

            string asciiPart = "BILGI_YOK";
            try
            {
                var step99 = recipe.Steps.FirstOrDefault(s => s.StepNumber == 99);
                if (step99 != null && step99.StepDataWords.Length >= 5)
                {
                    byte[] asciiBytes = new byte[10];
                    for (int i = 0; i < 5; i++)
                    {
                        short word = step99.StepDataWords[i];
                        byte[] wordBytes = BitConverter.GetBytes(word);
                        asciiBytes[i * 2] = wordBytes[0];
                        asciiBytes[i * 2 + 1] = wordBytes[1];
                    }
                    asciiPart = Encoding.ASCII.GetString(asciiBytes).Replace("\0", "").Trim();
                    if (string.IsNullOrEmpty(asciiPart)) asciiPart = "BOS";
                }
                else { asciiPart = "ADIM99_YOK"; }
            }
            catch { asciiPart = "HATA"; }

            return $"{machineName}-{recipeNumberPart}-{asciiPart}";
        }

        private void InitializeBYMakinesiEditor(string recipeName)
        {
            _byMakinesiEditor = new SplitContainer();
            dgvRecipeSteps = new DataGridView();
            pnlStepDetails = new Panel();

            // Üst başlık çubuğunu oluşturun ve reçete adını atayın
            var pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 30, BackColor = Color.LightSteelBlue };
            var lblRecipeName = new Label { Dock = DockStyle.Fill, Text = recipeName, Font = new Font("Segoe UI", 10F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            pnlTopBar.Controls.Add(lblRecipeName);

            _byMakinesiEditor.Dock = DockStyle.Fill;
            _byMakinesiEditor.SplitterDistance = 450;
            _byMakinesiEditor.Panel1.Controls.Add(dgvRecipeSteps);

            // Panel2'ye ilk olarak başlık panelini ekle
            _byMakinesiEditor.Panel2.Controls.Add(pnlTopBar);

            // Ardından, adım detayları panelini (Fill) olarak ekle
            _byMakinesiEditor.Panel2.Controls.Add(pnlStepDetails);

            dgvRecipeSteps.Dock = DockStyle.Fill;
            dgvRecipeSteps.AllowUserToAddRows = false;
            dgvRecipeSteps.AllowUserToDeleteRows = false;
            dgvRecipeSteps.ReadOnly = true;
            dgvRecipeSteps.MultiSelect = false;
            dgvRecipeSteps.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecipeSteps.CellClick += DgvRecipeSteps_CellClick;

            pnlStepDetails.Dock = DockStyle.Fill;
            pnlStepDetails.AutoScroll = true;
            pnlStepDetails.BorderStyle = BorderStyle.FixedSingle;

            SetupStepsGridView();
        }


        private void SetupStepsGridView()
        {
            if (dgvRecipeSteps == null) return;
            dgvRecipeSteps.DataSource = null;
            dgvRecipeSteps.Rows.Clear();
            dgvRecipeSteps.Columns.Clear();
            dgvRecipeSteps.AutoGenerateColumns = false;

            dgvRecipeSteps.Columns.Add(new DataGridViewTextBoxColumn { Name = "StepNumber", HeaderText = "Adım No", DataPropertyName = "StepNumber", Width = 60 });
            dgvRecipeSteps.Columns.Add(new DataGridViewTextBoxColumn { Name = "StepType", HeaderText = "Adım Tipi", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        }

        private void PopulateStepsGridView()
        {
            if (_previewRecipe == null || _previewRecipe.Steps == null || dgvRecipeSteps == null) return;
            dgvRecipeSteps.Rows.Clear();
            foreach (var step in _previewRecipe.Steps)
            {
                string stepTypeName = GetStepTypeName(step);
                dgvRecipeSteps.Rows.Add(step.StepNumber, stepTypeName);
            }
        }

        private string GetStepTypeName(ScadaRecipeStep step)
        {
            var stepTypes = new List<string>();
            if (step.StepDataWords.Length > 24)
            {
                short controlWord = step.StepDataWords[24];
                if ((controlWord & 1) != 0) stepTypes.Add("Su Alma");
                if ((controlWord & 2) != 0) stepTypes.Add("Isıtma");
                if ((controlWord & 4) != 0) stepTypes.Add("Çalışma");
                if ((controlWord & 8) != 0) stepTypes.Add("Dozaj");
                if ((controlWord & 16) != 0) stepTypes.Add("Boşaltma");
                if ((controlWord & 32) != 0) stepTypes.Add("Sıkma");
            }
            return stepTypes.Any() ? string.Join(" + ", stepTypes) : "Tanımsız Adım";
        }

        private void DgvRecipeSteps_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _previewRecipe == null || pnlStepDetails == null) return;

            var selectedMachine = clbMachines.CheckedItems.Cast<Machine>().FirstOrDefault();
            if (selectedMachine == null) return;

            pnlStepDetails.Controls.Clear();

            int selectedIndex = e.RowIndex;
            if (selectedIndex < _previewRecipe.Steps.Count)
            {
                var selectedStep = _previewRecipe.Steps[selectedIndex];
                var mainEditor = new StepEditor_Control();
                mainEditor.LoadStep(selectedStep, selectedMachine);
                mainEditor.SetReadOnly(true);
                mainEditor.Dock = DockStyle.Top;
                mainEditor.AutoSize = true;
                pnlPreviewArea.Controls.Add(mainEditor);
            }
        }

        #endregion
        // BU KODU FtpSync_Form.cs DOSYASININ SONUNA EKLEYİN

        // DataGridView için özel ProgressBar kolonu
        public class DataGridViewProgressBarColumn : DataGridViewTextBoxColumn
        {
            public DataGridViewProgressBarColumn()
            {
                this.CellTemplate = new DataGridViewProgressBarCell();
            }
        }

        public class DataGridViewProgressBarCell : DataGridViewTextBoxCell
        {
            protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts & ~DataGridViewPaintParts.ContentForeground);

                int progressVal = (value == null) ? 0 : (int)value;
                float percentage = ((float)progressVal / 100.0f);

                if (percentage > 0.0)
                {
                    // İlerleme çubuğunun rengini belirle
                    Brush progressBarBrush = new SolidBrush(Color.FromArgb(180, 220, 180));
                    g.FillRectangle(progressBarBrush, cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32((percentage * cellBounds.Width - 4)), cellBounds.Height - 4);
                    progressBarBrush.Dispose();
                }

                // Yüzde metnini yazdır
                string text = progressVal.ToString() + "%";
                SizeF textSize = g.MeasureString(text, cellStyle.Font);
                float textX = cellBounds.X + (cellBounds.Width - textSize.Width) / 2;
                float textY = cellBounds.Y + (cellBounds.Height - textSize.Height) / 2;
                g.DrawString(text, cellStyle.Font, Brushes.Black, textX, textY);
            }
        }
    }
}