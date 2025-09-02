// VncViewer_Form.cs
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using VncSharpCore; // RemoteDesktop sınıfı için

namespace TekstilScada.UI
{
    public partial class VncViewer_Form : Form
    {
        private readonly string _address;
        private readonly string _password;
        private int _port = 5900; // Varsayılan port
        private bool _isClosingInitiated = false; // Form kapatma işleminin başlatılıp başlatılmadığını izle

        public VncViewer_Form(string address, string password)
        {
            InitializeComponent();

            // Adres ve Port ayrıştırma işlemi
            if (address.Contains(":"))
            {
                var parts = address.Split(':');
                _address = parts[0];
                // Port ayrıştırma başarılı olmazsa _port varsayılan değeri (5900) korur
                if (parts.Length > 1 && !int.TryParse(parts[1], out _port))
                {
                    System.Diagnostics.Debug.WriteLine($"Uyarı: Geçersiz port numarası algılandı: '{parts[1]}'. Varsayılan port (5900) kullanılacak.");
                    MessageBox.Show("Geçersiz port numarası. Varsayılan port (5900) kullanılacak.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _port = 5900;
                }
            }
            else
            {
                _address = address;
            }

            _password = password;

            // Olayları (events) bağlayalım
            remoteDesktop1.ConnectComplete += VncControl_ConnectComplete;
            remoteDesktop1.ConnectionLost += VncControl_ConnectionLost;
            remoteDesktop1.GetPassword = () => _password; // Şifre delegate'ini burada ayarla
        }

        private async void VncViewer_Form_Load(object sender, EventArgs e)
        {
            this.Text = $"{_address}:{_port} - Bağlanılıyor...";
            try
            {
                // Bağlantıyı arka planda başlatıyoruz.
                await Task.Run(() => remoteDesktop1.Connect(_address));
            }
            catch (Exception ex)
            {
                // Bağlantı başlatılırken bir hata oluşursa:
                System.Diagnostics.Debug.WriteLine($"VNC bağlantı başlatma hatası: {ex.Message}");
                MessageBox.Show($"VNC bağlantısı başlatılırken hata oluştu: {ex.Message}", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Hata durumunda formu kapatma işlemine başla
                _isClosingInitiated = true; // Kapanma işleminin bu hata nedeniyle başladığını işaretle
                this.Close();
            }
        }

        private void VncControl_ConnectComplete(object sender, ConnectEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VncControl_ConnectComplete(sender, e)));
                return;
            }
            this.Text = $"{_address}:{_port} - Bağlandı: {e.DesktopName}";
        }

        private void VncControl_ConnectionLost(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VncControl_ConnectionLost(sender, e)));
                return;
            }

            System.Diagnostics.Debug.WriteLine("VNC bağlantısı kesildi veya kaybedildi.");
            MessageBox.Show("VNC bağlantısı kesildi veya kaybedildi.", "Bağlantı Kesildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!_isClosingInitiated)
            {
                _isClosingInitiated = true;
                this.Close();
            }
        }

        private void VncViewer_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isClosingInitiated && e.CloseReason != CloseReason.None)
            {
                return;
            }

            _isClosingInitiated = true;

            if (remoteDesktop1.IsConnected)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("VncViewer_Form_FormClosing: Bağlantı kesiliyor...");
                    remoteDesktop1.Disconnect();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"VncViewer_Form_FormClosing: Bağlantı kesilirken hata: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("VncViewer_Form_FormClosing: Bağlantı zaten kesik.");
            }

            remoteDesktop1.ConnectComplete -= VncControl_ConnectComplete;
            remoteDesktop1.ConnectionLost -= VncControl_ConnectionLost;
            System.Diagnostics.Debug.WriteLine("VncViewer_Form_FormClosing: Event abonelikleri kaldırıldı.");
        }
    }
}