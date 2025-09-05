// UI/Views/Ayarlar_Control.cs
using System.Collections.Generic;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Properties;
using TekstilScada.Repositories;
using TekstilScada.Services;
namespace TekstilScada.UI.Views
{
    public partial class Ayarlar_Control : UserControl
    {
        public event System.EventHandler MachineListChanged;

        private readonly MachineSettings_Control _machineSettings;
        private readonly UserSettings_Control _userSettings;
        private readonly AlarmSettings_Control _alarmSettings;
        private readonly PlcOperatorSettings_Control _plcOperatorSettings;
        private readonly CostSettings_Control _costSettings; // YENİ
        private readonly RecipeStepDesigner_Control _recipeStepDesigner;
        public Ayarlar_Control()
        {
            InitializeComponent();
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
            _machineSettings = new MachineSettings_Control();
            _userSettings = new UserSettings_Control();
            _alarmSettings = new AlarmSettings_Control();
            _plcOperatorSettings = new PlcOperatorSettings_Control();
            _costSettings = new CostSettings_Control(); // YENİ
            // YENİ: Tasarımcı kontrolünü oluştur
            _recipeStepDesigner = new RecipeStepDesigner_Control();
            _machineSettings.MachineListChanged += (sender, args) => { MachineListChanged?.Invoke(this, args); };

            _machineSettings.Dock = DockStyle.Fill;
            tabPageMachineSettings.Controls.Add(_machineSettings);

            _userSettings.Dock = DockStyle.Fill;
            tabPageUserSettings.Controls.Add(_userSettings);

            _alarmSettings.Dock = DockStyle.Fill;
            tabPageAlarmSettings.Controls.Add(_alarmSettings);

            _plcOperatorSettings.Dock = DockStyle.Fill;
            tabPagePlcOperators.Controls.Add(_plcOperatorSettings);
            _costSettings.Dock = DockStyle.Fill; // YENİ
            tabPageCostSettings.Controls.Add(_costSettings); // YENİ
                                                             // YENİ: Tasarımcı kontrolünü yeni sekmeye ekle
            _recipeStepDesigner.Dock = DockStyle.Fill;
            tabPageRecipeDesigner.Controls.Add(_recipeStepDesigner);
            ApplyPermissions();
        }
        public void RefreshUserRoles()
        {
            _userSettings.LoadAllRoles();
        }
        public void ApplyPermissions1()
        {
            ApplyPermissions();
        }
        private void ApplyPermissions()
        {

            // === ANA MENÜ BUTONLARI İÇİN YETKİLENDİRME ===
            // 5 numaralı role sahip kullanıcılar rapor alabilir
            _machineSettings.Visible = PermissionService.HasAnyPermission(new List<int> { 6 });
            _userSettings.Visible = PermissionService.HasAnyPermission(new List<int> { 7 });
            _alarmSettings.Enabled = PermissionService.HasAnyPermission(new List<int> { 8 });
            _costSettings.Enabled = PermissionService.HasAnyPermission(new List<int> { 9 });
            _plcOperatorSettings.Enabled = PermissionService.HasAnyPermission(new List<int> { 10 });
            _recipeStepDesigner.Visible = PermissionService.HasAnyPermission(new List<int> { 11 });
            // btnVnc.Enabled = btnVnc.Visible; // Yetkisi yoksa butonun tıklanmasını engelle


            var master = PermissionService.HasAnyPermission(new List<int> { 1000 });
            if (master == true)
            {
                _machineSettings.Visible = PermissionService.HasAnyPermission(new List<int> { 1000 });
                _userSettings.Visible = PermissionService.HasAnyPermission(new List<int> { 1000 });
                _alarmSettings.Enabled = PermissionService.HasAnyPermission(new List<int> { 1000 });
                _costSettings.Enabled = PermissionService.HasAnyPermission(new List<int> { 1000 });
                _plcOperatorSettings.Enabled = PermissionService.HasAnyPermission(new List<int> { 1000 });
                _recipeStepDesigner.Visible = PermissionService.HasAnyPermission(new List<int> { 1000 });
            }

        }
        public void RefreshMachineSettingsView()
        {
            _machineSettings.RefreshMachineList();
        }
        // DEĞİŞİKLİK: LsPlcManager -> IPlcManager
        public void InitializeControl(MachineRepository machineRepo, Dictionary<int, IPlcManager> plcManagers)
        {
            _plcOperatorSettings.InitializeControl(machineRepo, plcManagers);
        }
        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();

        }
        public void ApplyLocalization()
        {
            tabPageMachineSettings.Text = Resources.MachineManagement;
            tabPageUserSettings.Text = Resources.UserManagement;
            tabPageAlarmSettings.Text = Resources.AlarmSettings;
            tabPageCostSettings.Text = Resources.cost;
            tabPagePlcOperators.Text = Resources.PlcOperatorManagement;
           tabPageRecipeDesigner.Text = Resources.recipedesigner;
            //btnSave.Text = Resources.Save;


        }
    }
}
