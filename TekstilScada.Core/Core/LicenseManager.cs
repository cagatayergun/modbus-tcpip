// TekstilScada.Core/Core/LicenseManager.cs
using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TekstilScada.Core
{
    public class LicenseData
    {
        public string HardwareKey { get; set; }
        public int MachineLimit { get; set; }
        public string Signature { get; set; }
    }

    public static class LicenseManager
    {
        // GÜVENLİK NOTU: Buradaki açık anahtarı kendi ürettiğiniz anahtarla değiştirin.
        private const string PublicKeyXml = "<RSAKeyValue><Modulus>yck6I5qC/8sWOzOOiJx985LZwUCX+MIcYN5ymdsfCq8SjHhZleV7ZSN6LmChihhDQNLHZjqV7rhY/n+509NYI8aWILtDAI8j2RJNJFZcSMLEsFovEj+ZXqCVqOk/djDAbHSK/Ty3hbCpG4mIAooSqr4NF2qlNwTu1hDCj/gjX8Y2xZp9J1T3VnuKrU/U32XteZLcB2FH9kU+AeM8hkFqK7SaShaxahCFFXr3DJU6OF7ULMed1Efq0vOyp1WDurfOKH0zlbSnZ4GnhfXBN9+WXVdtzBpyYv0AUuwGm6umEnIvaeBEDgPrTSTeJGVLv3G5QMc2E13YkMMTOUMXVCSwgQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static (bool IsValid, string Message, LicenseData Data) ValidateLicense()
        {
            try
            {
                // Kendi makinemizin donanım key'ini al
                string currentHardwareKey = GenerateHardwareKey();
                if (string.IsNullOrEmpty(currentHardwareKey))
                {
                    return (false, "Donanım bilgileri alınamadı.", null);
                }

                // Lisans dosyasını oku
                if (!File.Exists("license.lic"))
                {
                    return (false, "Lisans dosyası bulunamadı (license.lic).", null);
                }
                string licenseJson = File.ReadAllText("license.lic");
                var licenseData = JsonSerializer.Deserialize<LicenseData>(licenseJson);

                if (licenseData == null || string.IsNullOrEmpty(licenseData.Signature))
                {
                    return (false, "Lisans dosyası geçersiz.", null);
                }

                // İmza doğrulama
                string originalSignature = licenseData.Signature;
                licenseData.Signature = null;
                string unsignedDataJson = JsonSerializer.Serialize(licenseData);

                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(PublicKeyXml);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(unsignedDataJson);
                    byte[] signatureBytes = Convert.FromBase64String(originalSignature);

                    if (!rsa.VerifyData(dataBytes, new SHA256CryptoServiceProvider(), signatureBytes))
                    {
                        return (false, "Lisans imzası geçersiz. Dosya kurcalanmış olabilir.", null);
                    }
                }

                // Donanım key kontrolü
                if (licenseData.HardwareKey != currentHardwareKey)
                {
                    return (false, "Lisans, bu bilgisayar için geçerli değil.", null);
                }

                return (true, "Lisans başarıyla doğrulandı.", licenseData);
            }
            catch (Exception ex)
            {
                return (false, $"Lisans doğrulaması sırasında beklenmedik bir hata oluştu: {ex.Message}", null);
            }
        }

        // Donanım key'ini oluşturan metot
        public static string GenerateHardwareKey()
        {
            try
            {
                string motherboardId = GetHardwareInfo("Win32_BaseBoard", "SerialNumber");
                string biosId = GetHardwareInfo("Win32_BIOS", "SerialNumber");
                string diskId = GetHardwareInfo("Win32_DiskDrive", "SerialNumber");
                string combinedString = $"{motherboardId}|{biosId}|{diskId}".Trim();
                if (string.IsNullOrEmpty(combinedString)) return null;

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                    return builder.ToString().ToUpper();
                }
            }
            catch { return null; }
        }

        private static string GetHardwareInfo(string wmiClass, string wmiProperty)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM {wmiClass}");
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj[wmiProperty] != null) return obj[wmiProperty].ToString().Trim();
                }
            }
            catch { }
            return "";
        }
    }
}