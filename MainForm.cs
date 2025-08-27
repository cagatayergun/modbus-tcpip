// MainForm.cs
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Localization;
using TekstilScada.Models;
using TekstilScada.Repositories;
using TekstilScada.Services;
using TekstilScada.UI;
using TekstilScada.UI.Controls;
using TekstilScada.UI.Views;
using TekstilScada.Properties;

namespace TekstilScada
{
    public partial class MainForm : Form
    {
        // Repository ve Servisler
        private readonly MachineRepository _machineRepository;
        private readonly RecipeRepository _recipeRepository;
        private readonly ProcessLogRepository _processLogRepository;
        private readonly AlarmRepository _alarmRepository;
        private readonly ProductionRepository _productionRepository;
        private readonly PlcPollingService _pollingService;
        private readonly DashboardRepository _dashboardRepository;
        private readonly CostRepository _costRepository; // YENÝ: Alaný ekleyin
        // Arayüz Kontrolleri (Views)
        private readonly ProsesÝzleme_Control _prosesIzlemeView;
        private readonly ProsesKontrol_Control _prosesKontrolView;
        private readonly Ayarlar_Control _ayarlarView;
        private readonly MakineDetay_Control _makineDetayView;
        private readonly Raporlar_Control _raporlarView;
        private readonly LiveEventPopup_Form _liveEventPopup;
        private readonly GenelBakis_Control _genelBakisView;
        private readonly FtpTransferService _ftpTransferService; // YENÝ: FTP transfer servisi eklendi
        private VncViewer_Form _activeVncViewerForm = null;

        public MainForm()
        {
            InitializeComponent();

            // 1. ADIM: Tüm nesneler burada oluþturulur.
            // Bu, NullReferenceException hatasýný önlemek için kritiktir.
            _machineRepository = new MachineRepository();
            _recipeRepository = new RecipeRepository();
            _processLogRepository = new ProcessLogRepository();
            _alarmRepository = new AlarmRepository();
            _productionRepository = new ProductionRepository();
            _pollingService = new PlcPollingService(_alarmRepository, _processLogRepository, _productionRepository, _recipeRepository);
            _dashboardRepository = new DashboardRepository(_recipeRepository);
            _costRepository = new CostRepository(); // YENÝ: Nesneyi oluþturun
            _prosesIzlemeView = new ProsesÝzleme_Control();
           _prosesKontrolView = new ProsesKontrol_Control();
            _ayarlarView = new Ayarlar_Control();
            _makineDetayView = new MakineDetay_Control();
            _raporlarView = new Raporlar_Control();
            _liveEventPopup = new LiveEventPopup_Form();
            _genelBakisView = new GenelBakis_Control();
            _ftpTransferService = new FtpTransferService(_pollingService);
            //_prosesKontrolView = new ProsesKontrol_Control(_ftpTransferService);
            // Olay abonelikleri (Events)
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
            _ayarlarView.MachineListChanged += OnMachineListChanged;
            _pollingService.OnActiveAlarmStateChanged += OnActiveAlarmStateChanged;
            _prosesIzlemeView.MachineDetailsRequested += OnMachineDetailsRequested;
            _prosesIzlemeView.MachineVncRequested += OnMachineVncRequested;
            _makineDetayView.BackRequested += OnBackRequested;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 2. ADIM: Form tamamen yüklendikten sonra bu metot çalýþýr.
            // Veritabaný ve PLC iþlemlerini baþlatan metotlar burada çaðrýlýr.
            ApplyLocalization();
            UpdateUserInfoAndPermissions();
            ReloadSystem(_genelBakisView);
            ApplyPermissions(); // YENÝ: Yetkileri uygula
        }
        private void ApplyPermissions()
        {
            // Raporlar butonunu sadece yetkisi olanlar görebilir/kullanabilir.
            btnRaporlar.Enabled = PermissionService.CanViewReports;

            // Proses Kontrol (Reçete) butonunu yetkisi olanlar kullanabilir.
            btnProsesKontrol.Enabled = PermissionService.CanEditRecipes;

            // Ayarlar butonunu sadece Admin görebilir/kullanabilir.
            btnAyarlar.Enabled = PermissionService.CanViewSettings;
        }
        private void ReloadSystem(Control viewToShow)
        {
            _pollingService.Stop();
            List<Machine> machines = _machineRepository.GetAllEnabledMachines();
            if (machines == null)
            {
                MessageBox.Show(Resources.DatabaseConnectionFailed, Resources.CriticalError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _pollingService.Start(machines);
            var plcManagers = _pollingService.GetPlcManagers();

            // Kontrolleri en güncel verilerle baþlat
            _prosesIzlemeView.InitializeView(machines, _pollingService);
            _prosesKontrolView.InitializeControl(_recipeRepository, _machineRepository, plcManagers, _pollingService);

            _ayarlarView.InitializeControl(_machineRepository, plcManagers);
            // GÜNCELLENDÝ: CostRepository parametresini ekleyin
            _raporlarView.InitializeControl(_machineRepository, _alarmRepository, _productionRepository, _dashboardRepository, _processLogRepository, _recipeRepository, _costRepository);
            _genelBakisView.InitializeControl(_pollingService, _machineRepository, _dashboardRepository, _alarmRepository, _processLogRepository, _productionRepository);

            // HANGÝ SAYFANIN GÖSTERÝLECEÐÝNÝ KONTROL ET
            if (viewToShow != _genelBakisView)
            {
                // Eðer bir sayfa belirtilmiþse onu göster
                ShowView(_ayarlarView);
            }
            else
            {
                // Eðer bir sayfa belirtilmemiþse (ilk açýlýþ gibi), Genel Bakýþ'ý göster
                ShowView(_genelBakisView);
            }
        }

        #region Arayüz ve Dil Yönetimi

        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();
            UpdateUserInfoAndPermissions();
        }

        private void ApplyLocalization()
        {
            // Ana form baþlýðý ve ana menü butonlarý "Strings" sýnýfýndan geliyor.
            this.Text = TekstilScada.Localization.Strings.ApplicationTitle;
            btnGenelBakis.Text = TekstilScada.Localization.Strings.MainMenu_GeneralOverview;
            btnProsesIzleme.Text = TekstilScada.Localization.Strings.MainMenu_ProcessMonitoring;
            btnProsesKontrol.Text = TekstilScada.Localization.Strings.MainMenu_ProcessControl;
            btnRaporlar.Text = TekstilScada.Localization.Strings.MainMenu_Reports;
            btnAyarlar.Text = TekstilScada.Localization.Strings.MainMenu_Settings;

            // Menü ve durum çubuðu gibi diðer elemanlar "Resources" sýnýfýndan geliyor.
            dilToolStripMenuItem.Text = Resources.Language;
            oturumToolStripMenuItem.Text = Resources.Session;
            çýkýþYapToolStripMenuItem.Text = Resources.Logout;
            lblStatusLiveEvents.Text = Resources.Livelogsee;
        }

        private void UpdateUserInfoAndPermissions()
        {
            if (CurrentUser.IsLoggedIn)
            {
                lblStatusCurrentUser.Text = $"{Resources.Loggedin}: {CurrentUser.User.FullName}";
            }
            else
            {
                lblStatusCurrentUser.Text = $"{Resources.Loggedin}: -";
            }
            // Ayarlar butonunu sadece "Admin" rolüne sahip kullanýcýlar için etkinleþtir.
            btnAyarlar.Enabled = CurrentUser.HasRole("Admin");
        }

        private void ShowView(UserControl view)
        {
            if (view is Ayarlar_Control && !PermissionService.CanViewSettings)

            {
                MessageBox.Show(Resources.NoAccess, Resources.AccessDenied);
                return;
            }
            if (view is Raporlar_Control && !PermissionService.CanViewReports)
            {
                MessageBox.Show(Resources.NoAccess, Resources.AccessDenied);
                return;
            }
            pnlContent.Controls.Clear();
            view.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(view);
        }

        #endregion

        #region Olay Yöneticileri (Event Handlers)

        private void OnMachineListChanged(object sender, EventArgs e) => ReloadSystem(_ayarlarView);
        private void OnBackRequested(object sender, EventArgs e) => ShowView(_prosesIzlemeView);
        private void btnGenelBakis_Click(object sender, EventArgs e) => ShowView(_genelBakisView);
        private void btnProsesIzleme_Click(object sender, EventArgs e) => ShowView(_prosesIzlemeView);
        private void btnProsesKontrol_Click(object sender, EventArgs e) => ShowView(_prosesKontrolView);
        private void btnRaporlar_Click(object sender, EventArgs e) => ShowView(_raporlarView);
        private void btnAyarlar_Click(object sender, EventArgs e) => ShowView(_ayarlarView);
        private void türkçeToolStripMenuItem_Click(object sender, EventArgs e) => LanguageManager.SetLanguage("tr-TR");
        private void englishToolStripMenuItem_Click(object sender, EventArgs e) => LanguageManager.SetLanguage("en-US");

        private void çýkýþYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Resources.Cikiseminmisin, Resources.Confirim, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                _pollingService.Stop();

                using (var loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        UpdateUserInfoAndPermissions();
                        ReloadSystem(_genelBakisView);
                        this.Show();
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
        }

        private void OnMachineDetailsRequested(object sender, int machineId)
        {
            var machine = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == machineId);
            if (machine != null)
            {
                // YENÝ: _productionRepository parametresini ekleyin
                _makineDetayView.InitializeControl(machine, _pollingService, _processLogRepository, _alarmRepository, _recipeRepository, _productionRepository);
                ShowView(_makineDetayView);
            }
        }
        private void OnMachineVncRequested(object sender, int machineId)
        {
            if (_activeVncViewerForm != null && !_activeVncViewerForm.IsDisposed)
            {
                _activeVncViewerForm.Activate();
                MessageBox.Show(Resources.Vnccurrentclose, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var machine = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == machineId);
            if (machine != null && !string.IsNullOrEmpty(machine.VncAddress))
            {
                try
                {
                    var vncForm = new VncViewer_Form(machine.VncAddress, machine.VncPassword);
                    vncForm.Text = $"{machine.MachineName} - {Resources.VncConnectionTo}";
                    vncForm.FormClosed += (s, args) => { _activeVncViewerForm = null; };
                    _activeVncViewerForm = vncForm;
                    vncForm.Show();
                }
                catch (Exception ex)
                {
                    _activeVncViewerForm = null;
                    MessageBox.Show($"{Resources.Vncconnecterror} {ex.Message}", Resources.Error);
                }
            }
            else
            {
                MessageBox.Show(Resources.Vncnomachine, Resources.Information);
            }
        }

        private void OnActiveAlarmStateChanged(int machineId, FullMachineStatus status)
        {
            if (!this.IsHandleCreated || this.IsDisposed) return;
            this.Invoke(new Action(() =>
            {
                if (this.IsDisposed) return;

                var activeAlarms = _pollingService.MachineDataCache.Values.Where(s => s.HasActiveAlarm).ToList();

                if (activeAlarms.Any())
                {
                    var alarmToShow = activeAlarms
                        .Select(s => new { Status = s, Definition = _alarmRepository.GetAlarmDefinitionByNumber(s.ActiveAlarmNumber) })
                        .Where(ad => ad.Definition != null)
                        .OrderByDescending(ad => ad.Definition.Severity)
                        .FirstOrDefault();

                    if (alarmToShow != null)
                    {
                        lblStatusLiveEvents.Text = $"[{alarmToShow.Status.MachineName}] - ALARM: {alarmToShow.Definition.AlarmText}";
                        lblStatusLiveEvents.BackColor = Color.FromArgb(231, 76, 60); // Kýrmýzý
                        lblStatusLiveEvents.ForeColor = Color.White;
                    }
                }
                else
                {
                   
                    lblStatusLiveEvents.BackColor = System.Drawing.SystemColors.Control;
                    lblStatusLiveEvents.ForeColor = System.Drawing.SystemColors.ControlText;
                }
            }));
        }

        private void lblStatusLiveEvents_Click(object sender, EventArgs e)
        {
            if (_liveEventPopup.Visible)
            {
                _liveEventPopup.Hide();
            }
            else
            {
                _liveEventPopup.Show(this);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LanguageManager.LanguageChanged -= LanguageManager_LanguageChanged;
            _pollingService.Stop();
            if (_activeVncViewerForm != null && !_activeVncViewerForm.IsDisposed)
            {
                try
                {
                    _activeVncViewerForm.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{Resources.Closeandvnc} {ex.Message}");
                }
            }
        }

        #endregion
    }
}