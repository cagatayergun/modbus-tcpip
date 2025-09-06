// Program.cs
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.UI;
using TekstilScada.Properties;

namespace TekstilScada
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // === LÝSANS DOÐRULAMA VE KONTROL KODU BAÞLANGICI ===
            var (isValid, message, licenseData) = LicenseManager.ValidateLicense();
            string currentHardwareKey = LicenseManager.GenerateHardwareKey() ?? "Donaným bilgileri alýnamadý.";
            string licenseFileHardwareKey = licenseData?.HardwareKey;

            string debugMessage = $"Lisans Doðrulama Durumu: {(isValid ? "BAÞARILI" : "HATALI")}\n" +
                                  $"Hata Mesajý: {message}\n\n" +
                                  $"Bu Bilgisayarýn Anahtarý: {currentHardwareKey}\n" +
                                  $"Lisans Dosyasýndaki Anahtar: {licenseFileHardwareKey}";

            // Mesaj kutusunu, programýn sonlanmasýndan önce gösteriyoruz.
            MessageBox.Show(debugMessage, "Lisans Durum Kontrolü");

            if (!isValid)
            {
                // Mesaj kutusu gösterildikten sonra programdan çýkýþ yap.
                Application.Exit();
                return;
            }

            AppConfig.SetConnectionString(licenseData.EncryptedConnectionString);
            // === LÝSANS DOÐRULAMA VE KONTROL KODU BÝTÝÞÝ ===

            Application.Run(new MainForm());
        }
    }
}