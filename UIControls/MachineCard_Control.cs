// UI/Controls/MachineCard_Control.cs
using System;
using System.Drawing;
using System.Drawing.Imaging; // ColorMatrix için bu using ifadesi gerekli
using System.Windows.Forms;
using TekstilScada.Models;
using TekstilScada.Services;

namespace TekstilScada.UI.Controls
{
    public partial class MachineCard_Control : UserControl
    {
        public int MachineId { get; private set; }
        private int _lastValidProgress = 0; // BU YENİ ALANI EKLEYİN
        public string MachineUserDefinedId { get; private set; }
        public string MachineName { get; private set; }

        public event EventHandler DetailsRequested;
        public event EventHandler VncRequested;

        private readonly Color _colorConnected = Color.FromArgb(46, 204, 113);
        private readonly Color _colorDisconnected = Color.FromArgb(231, 76, 60);
        private readonly Color _colorConnecting = Color.FromArgb(241, 196, 15);
        private readonly Color _colorAlarm = Color.FromArgb(192, 57, 43);
        private readonly Color _colorPlay = Color.FromArgb(46, 204, 113);
        private readonly Color _colorPause = Color.FromArgb(241, 196, 15);

        private readonly Image _originalPlayIcon;
        private readonly Image _originalPauseIcon;
        private readonly Image _originalAlarmIcon;
        private readonly Image _originalPlay2Icon;
        private readonly Image _originalPause2Icon;
        private readonly Image _originalAlarmvarIcon;
        private readonly Image _originalAlarmyokIcon;
        private readonly Image _originalbaglantivarIcon;
        private readonly Image _originalbaglantiyokIcon;


        public MachineCard_Control(int machineId, string machineUserDefinedId, string machineName, int displayIndex)
        {
            InitializeComponent();
            this.MachineId = machineId;
            this.MachineUserDefinedId = machineUserDefinedId;
            this.MachineName = machineName;
            lblMachineNumber.Text = $"{displayIndex}.";

            // Kaynaklardan orijinal ikonları bir kereliğine yükle
            _originalPlayIcon = Properties.Resource1.play2;
            _originalPauseIcon = Properties.Resource1.pause2;
            _originalAlarmIcon = Properties.Resource1.alarm_var;
            _originalAlarmyokIcon = Properties.Resource1.alarm_yok;
            _originalbaglantivarIcon = Properties.Resource1.yilmak_baglanti_2;
            _originalbaglantiyokIcon = Properties.Resource1.yilmak_baglanti;
            // GÜNCELLENDİ: PictureBox'ların arkaplanını şeffaf yap
            picPlay.BackColor = Color.Transparent;
            picPause.BackColor = Color.Transparent;
            picAlarm.BackColor = Color.Transparent;
            picPlay.Visible = false;
            picPause.Visible = false;
            picAlarm.Visible = false;
            btnVnc.Visible = false;
            btnInfo.Visible = false;

            UpdateView(new FullMachineStatus { ConnectionState = ConnectionStatus.Disconnected, MachineName = this.MachineName });
        }

        // GÜNCELLENDİ: Donmaya neden olmayan, performanslı ColorMatrix yöntemi
        private Image TintImage(Image sourceImage, Color tintColor)
        {
            if (sourceImage == null) return null;

            Bitmap newBitmap = new Bitmap(sourceImage.Width, sourceImage.Height);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                float r = tintColor.R / 255f;
                float gg = tintColor.G / 255f;
                float b = tintColor.B / 255f;

                // Bu matris, resmin orijinal Alpha (şeffaflık) değerini korurken,
                // renkli pikselleri hedef renge boyar.
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
        {
          new float[] {0, 0, 0, 0, 0},
          new float[] {0, 0, 0, 0, 0},
          new float[] {0, 0, 0, 0, 0},
          new float[] {0, 0, 0, 1, 0},
          new float[] {r, gg, b, 0, 1}
        });

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    g.DrawImage(sourceImage, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                          0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }

        private void ApplyPermissions()
        {

            // === ANA MENÜ BUTONLARI İÇİN YETKİLENDİRME ===
            // 5 numaralı role sahip kullanıcılar rapor alabilir
            var fullmachine = new FullMachineStatus();
            var status = fullmachine.ConnectionState;
            if(status==ConnectionStatus.Connected)
            { 
            btnVnc.Visible = PermissionService.HasAnyPermission(new List<int> { 4 });
            btnVnc.Enabled = btnVnc.Visible; // Yetkisi yoksa butonun tıklanmasını engelle
            var master = PermissionService.HasAnyPermission(new List<int> { 1000 });
            if (master == true)
            {
                btnVnc.Visible = PermissionService.HasAnyPermission(new List<int> { 1000 });
                btnVnc.Enabled = btnVnc.Visible; // Yetkisi yoksa butonun tıklanmasını engelle
            }
            }
        }
        public void UpdateView(FullMachineStatus status)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateView(status)));
                return;
            }

            switch (status.ConnectionState)
            {
                case ConnectionStatus.Connected:
                    picConnection.Image = _originalbaglantivarIcon;
                    picPlay.Visible = true;
                    picPause.Visible = true;
                    picAlarm.Visible = true;
                    btnVnc.Visible = true;
                    btnInfo.Visible = true;
                    break;
                case ConnectionStatus.Connecting:
                    picConnection.Image = _originalbaglantiyokIcon;
                    picPlay.Visible = false;
                    picPause.Visible = false;
                    picAlarm.Visible = false;
                    btnVnc.Visible = false;
                    btnInfo.Visible = false;
                    break;
                case ConnectionStatus.ConnectionLost:
                case ConnectionStatus.Disconnected:
                    picConnection.Image = _originalbaglantiyokIcon;
                    picPlay.Visible = false;
                    picPause.Visible = false;
                    picAlarm.Visible = false;
                    btnVnc.Visible = false;
                    btnInfo.Visible = false;
                    ClearData();
                    return;
            }

            lblRecipeNameValue.Text = status.RecipeName;
            lblOperatorValue.Text = status.OperatorIsmi;
            lblStepValue.Text = status.AktifAdimAdi;
            lblMachineNameValue.Text = status.MachineName;
            lblMachineIdValue.Text = this.MachineUserDefinedId;

            // Alarm varsa ilerleme çubuğunu duraklatma mantığı
            if (status.HasActiveAlarm)
            {
                // Alarm varsa, diğer ikonları gizle ve sadece alarm ikonunu göster
               
               
                // Alarm ikonunun kendisini göster, renk tonu değiştirme
               picAlarm.Image = _originalAlarmIcon;
                picPause.Visible = status.IsPaused;
                if (picPause.Visible) picPause.Image = _originalPauseIcon;
                picPlay.Visible = status.IsInRecipeMode && !status.IsPaused;
                if (progressBar.Value > 0)
                {
                    _lastValidProgress = progressBar.Value;
                }

                progressBar.Value = _lastValidProgress;
                lblPercentage.Text = $"{_lastValidProgress} %";
            }
            else
            {
                picAlarm.Image = _originalAlarmyokIcon;

                // Alarm yoksa, mevcut durum ikonlarını ve ilerlemeyi normal olarak işle
                picPlay.Visible = status.IsInRecipeMode && !status.IsPaused;
                if (picPlay.Visible) picPlay.Image = _originalPlayIcon;

                picPause.Visible = status.IsPaused;
                if (picPause.Visible) picPause.Image = _originalPauseIcon;

               

                _lastValidProgress = Math.Max(0, Math.Min(100, (int)status.ProsesYuzdesi));
                progressBar.Value = _lastValidProgress;
                lblPercentage.Text = $"{_lastValidProgress} %";
            }
            ApplyPermissions();
        }

        private void ClearData()
        {
            string noConnectionText = "---";
            lblRecipeNameValue.Text = noConnectionText;
            lblOperatorValue.Text = noConnectionText;
            lblStepValue.Text = noConnectionText;
            lblMachineNameValue.Text = this.MachineName;
            lblMachineIdValue.Text = this.MachineUserDefinedId;
            progressBar.Value = 0;
            lblPercentage.Text = "0 %";

            picPlay.Visible = false;
            picPause.Visible = false;
            picAlarm.Visible = false;
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            DetailsRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnVnc_Click(object sender, EventArgs e)
        {

            VncRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}