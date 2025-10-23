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
        private readonly FtpTransferService _ftpTransferService;
        private readonly MachineRepository _machineRepository;
        private readonly RecipeRepository _recipeRepository;
        private readonly ProcessLogRepository _processLogRepository;
        private readonly AlarmRepository _alarmRepository;
        private readonly ProductionRepository _productionRepository;
        private readonly PlcPollingService _pollingService;
        private readonly DashboardRepository _dashboardRepository;
        private readonly CostRepository _costRepository; // YENÝ: Alaný ekleyin
        private readonly UserRepository _userRepository ; // YENÝ: Alaný ekleyin
        // Arayüz Kontrolleri (Views)
        private readonly ProsesÝzleme_Control _prosesIzlemeView;
        private readonly ProsesKontrol_Control _prosesKontrolView;
        private readonly Ayarlar_Control _ayarlarView;
        private readonly MakineDetay_Control _makineDetayView;
        private readonly Raporlar_Control _raporlarView;
        private readonly LiveEventPopup_Form _liveEventPopup;
        private readonly GenelBakis_Control _genelBakisView;
       // private readonly FtpTransferService _ftpTransferService; // YENÝ: FTP transfer servisi eklendi
        private VncViewer_Form _activeVncViewerForm = null;
        private readonly UserSettings_Control _user_setting;

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
            _pollingService = new PlcPollingService(_alarmRepository, _processLogRepository, _productionRepository, _recipeRepository, _machineRepository);
            _dashboardRepository = new DashboardRepository(_recipeRepository);
            _costRepository = new CostRepository(); // YENÝ: Nesneyi oluþturun
                                                 // DÜZELTME: FtpTransferService nesnesini burada oluþturun ve baðýmlýlýðý enjekte edin.
            _ftpTransferService = new FtpTransferService(_pollingService);
            _prosesIzlemeView = new ProsesÝzleme_Control();
           _prosesKontrolView = new ProsesKontrol_Control();
            _ayarlarView = new Ayarlar_Control();
            _makineDetayView = new MakineDetay_Control();
            _raporlarView = new Raporlar_Control();
            _liveEventPopup = new LiveEventPopup_Form();
            _genelBakisView = new GenelBakis_Control();
            _user_setting = new UserSettings_Control();
            _userRepository = new UserRepository();
         //   _ftpTransferService = new FtpTransferService(_pollingService);
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
            // === YENÝ LÝSANS DOÐRULAMA KODU BAÞLANGICI ===
            var (isValid, message, licenseData) = LicenseManager.ValidateLicense();

            if (!isValid)
            {
                MessageBox.Show($"Lisans Hatasý: {message}", "Uygulama Lisansý", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
             // Lisans baþarýlý, makine sayýsýný kontrol et
            var machines = _machineRepository.GetAllMachines();
            if (machines.Count > licenseData.MachineLimit)
            {
                var dialogResult = MessageBox.Show(
                    $"Lisansýnýz {licenseData.MachineLimit} makine ile sýnýrlýdýr. Veritabanýnýzda {machines.Count} makine bulunmaktadýr.\nFazla makineler otomatik olarak silinecektir. Devam etmek istiyor musunuz?",
                    "Makine Sayýsý Limiti Aþýldý",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    // Fazla makineleri silme mantýðý
                    for (int i = licenseData.MachineLimit; i < machines.Count; i++)
                    {
                        _machineRepository.DeleteMachine(machines[i].Id);
                    }
                    MessageBox.Show($"{machines.Count - licenseData.MachineLimit} adet makine baþarýyla silindi.", "Ýþlem Tamamlandý");
                }
                else
                {
                    this.Close();
                    return;
                }
            }
            // 2. ADIM: Form tamamen yüklendikten sonra bu metot çalýþýr.
            // Veritabaný ve PLC iþlemlerini baþlatan metotlar burada çaðrýlýr.
            ApplyLocalization();
            UpdateUserInfoAndPermissions();
            ReloadSystem(_genelBakisView);
            LanguageManager.SetLanguage("en-US");
        }
        private void ApplyPermissions()
        {

            // === ANA MENÜ BUTONLARI ÝÇÝN YETKÝLENDÝRME ===
            // 5 numaralý role sahip kullanýcýlar rapor alabilir
            btnProsesKontrol.Visible = PermissionService.HasAnyPermission(new List<int> { 1 });
            btnProsesKontrol.Enabled = btnProsesKontrol.Visible; // Yetkisi yoksa butonun týklanmasýný engelle

            btnRaporlar.Visible = PermissionService.HasAnyPermission(new List<int> { 2 });
            btnRaporlar.Enabled = btnRaporlar.Visible; // Yetkisi yoksa butonun týklanmasýný engelle

            // 8 numaralý role sahip kullanýcýlar ayarlar ekranýna eriþebilir
            btnAyarlar.Visible = PermissionService.HasAnyPermission(new List<int> { 3 });
            btnAyarlar.Enabled = btnAyarlar.Visible; // Yetkisi yoksa butonun týklanmasýný engelle

            // === ANA MENÜ BUTONLARI ÝÇÝN YETKÝLENDÝRME ===
            var master = PermissionService.HasAnyPermission(new List<int> { 1000 });
            if (master == true)
            {

                // 5 numaralý role sahip kullanýcýlar rapor alabilir
                btnProsesKontrol.Visible = PermissionService.HasAnyPermission(new List<int> { 1000 });
                btnProsesKontrol.Enabled = btnProsesKontrol.Visible; // Yetkisi yoksa butonun týklanmasýný engelle

                btnRaporlar.Visible = PermissionService.HasAnyPermission(new List<int> { 1000 });
                btnRaporlar.Enabled = btnRaporlar.Visible; // Yetkisi yoksa butonun týklanmasýný engelle

                // 8 numaralý role sahip kullanýcýlar ayarlar ekranýna eriþebilir
                btnAyarlar.Visible = PermissionService.HasAnyPermission(new List<int> { 1000 });
                btnAyarlar.Enabled = btnAyarlar.Visible; // Yetkisi yoksa butonun týklanmasýný engelle
            }
            _ayarlarView.ApplyPermissions1();
            _user_setting.LoadAllRoles();

        }
        private void ReloadSystem(Control viewToShow)
        {
            //MessageBox.Show($"{btnRaporlar.Visible}");
            _pollingService.Stop();
            // === YENÝ LÝSANS KONTROLÜ BAÞLANGICI ===
            var (isValid, message, licenseData) = LicenseManager.ValidateLicense();
            if (!isValid)
            {
                MessageBox.Show($"Lisans Hatasý: {message}", "Uygulama Lisansý", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Makine sayýsýný kontrol et ve lisans limitini aþanlarý sil
            var machines1 = _machineRepository.GetAllMachines();
            if (machines1.Count > licenseData.MachineLimit)
            {
                MessageBox.Show(
                    $"Lisansýnýz {licenseData.MachineLimit} makine ile sýnýrlýdýr. Veritabanýnýzda {machines1.Count} makine bulunmaktadýr.\nFazla makineler otomatik olarak silinecektir.",
                    "Makine Sayýsý Limiti Aþýldý",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                // Sondan baþlayarak fazla makineleri sil
                for (int i = machines1.Count - 1; i >= licenseData.MachineLimit; i--)
                {
                    _machineRepository.DeleteMachine(machines1[i].Id);
                }

                // Makine listesini yeniden oku
                machines1 = _machineRepository.GetAllMachines();
            }
            // === YENÝ LÝSANS KONTROLÜ BÝTÝÞÝ ===
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
            _prosesKontrolView.InitializeControl(_recipeRepository, _machineRepository, plcManagers, _pollingService, _ftpTransferService);

            _ayarlarView.InitializeControl(_machineRepository, plcManagers);
            // GÜNCELLENDÝ: CostRepository parametresini ekleyin
            _raporlarView.InitializeControl(_machineRepository, _alarmRepository, _productionRepository, _dashboardRepository, _processLogRepository, _recipeRepository, _costRepository);
            _genelBakisView.InitializeControl(_pollingService, _machineRepository, _dashboardRepository, _alarmRepository, _processLogRepository, _productionRepository);
            _ayarlarView.RefreshMachineSettingsView();
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
                try
                {
                    if (CurrentUser.IsLoggedIn && CurrentUser.User != null)
                    {
                        _userRepository.LogAction(CurrentUser.User.Id, "Log", $"Session Login");
                    }
                }
                catch (Exception logEx)
                {
                    // Loglama sýrasýnda oluþan hatayý kullanýcýya göster
                    MessageBox.Show($" 'Session Login' is Log error : {logEx.Message}", Resources.Error);
                }
            }
            else
            {
                lblStatusCurrentUser.Text = $"{Resources.Loggedin}: -";
                try
                {
                    if (CurrentUser.IsLoggedIn && CurrentUser.User != null)
                    {
                        _userRepository.LogAction(CurrentUser.User.Id, "Log", $"Session Logout");
                    }
                }
                catch (Exception logEx)
                {
                    // Loglama sýrasýnda oluþan hatayý kullanýcýya göster
                    MessageBox.Show($" 'Session Logout' is Log error : {logEx.Message}", Resources.Error);
                }
            }
            // Ayarlar butonunu sadece "Admin" rolüne sahip kullanýcýlar için etkinleþtir.
            _user_setting.LoadAllRoles();
            _ayarlarView.RefreshUserRoles();
            ApplyPermissions(); // YENÝ: Yetkileri uygula
                                // LogAction çaðrýsýný try-catch bloðuna taþýyoruz

        }

        private void ShowView(UserControl view)
        {
          
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
            // Kullanýcý çýkýþ yapýyorsa, CurrentUser'ý sýfýrla
            CurrentUser.User = null;
            UpdateUserInfoAndPermissions();
            ShowView(_genelBakisView); // Çýkýþ yaptýktan sonra genel bakýþ ekranýna dön

            // Yeni bir LoginForm açarak kullanýcý giriþi yapmasýný iste
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    UpdateUserInfoAndPermissions();
                    ReloadSystem(_genelBakisView);
                }
                else
                {
                    // Giriþ yapmayý iptal ederse, programý kapat
                    //Application.Exit();
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
               

                // LogAction çaðrýsýný try-catch bloðuna taþýyoruz
                try
                {
                    if (CurrentUser.IsLoggedIn && CurrentUser.User != null)
                    {
                        _userRepository.LogAction(CurrentUser.User.Id, "VNC Baðlantýsý", $"{machine.MachineName} makinesine VNC ile baðlandý.");
                    }
                }
                catch (Exception logEx)
                {
                    // Loglama sýrasýnda oluþan hatayý kullanýcýya göster
                    MessageBox.Show($"VNC baðlantýsý loglanýrken bir hata oluþtu: {logEx.Message}", Resources.Error);
                }

                
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