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
        public string EncryptedConnectionString { get; set; } // YENİ EKLENEN
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
                string connectionString = DecryptConnectionString(licenseData.EncryptedConnectionString);

                // Lisans verisini ve şifresi çözülmüş bağlantı dizesini döndür
                licenseData.EncryptedConnectionString = connectionString;
                //  return (true, "Lisans başarıyla doğrulandı.", licenseData);

                return (true, "Lisans başarıyla doğrulandı.", licenseData);
            }
            catch (Exception ex)
            {
                return (false, $"Lisans doğrulaması sırasında beklenmedik bir hata oluştu: {ex.Message}", null);
            }

        }
        private static string DecryptConnectionString(string encryptedData)
        {
            // Anahtar (key) uzunluğu 32 byte (256 bit) olmalıdır.
            // Lisans oluşturan programdaki anahtarla aynı olduğundan emin olun.
            byte[] key = Encoding.UTF8.GetBytes("mysupersecretkeythatis32byteslon");

            // Başlangıç vektörü (IV) uzunluğu 16 byte (128 bit) olmalıdır.
            // Lisans oluşturan programdaki IV ile aynı olduğundan emin olun.
            byte[] iv = Encoding.UTF8.GetBytes("16-byte-vector-!");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
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
            catch (Exception ex)
            {
                // Hatanın nedenini loglamak veya göstermek için kod ekle
                LogToFile($"WMI erişim hatası - Sınıf: {wmiClass}, Hata: {ex.Message}");

            }
            return "";
        }
        private static void LogToFile(string logMessage)
        {
            // Uygulamanın çalıştığı dizinde "logs" adında bir klasör oluşturur.
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Dosya yolu: [UygulamaDizini]/logs/hardware_log.txt
            string logFilePath = Path.Combine(logDirectory, "hardware_log.txt");

            // Mesajı zaman damgasıyla birlikte dosyaya ekler.
            string formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {logMessage}{Environment.NewLine}";
            File.AppendAllText(logFilePath, formattedMessage);
        }
    }
}
