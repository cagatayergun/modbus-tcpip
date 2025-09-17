// Dosya: TekstilScada.Core/Core/LanguageManager.cs
using System;
using System.Globalization;
using System.Threading;
// using TekstilScada.Properties; // BU SATIRI SİLİN VEYA YORUMA ALIN

namespace TekstilScada.Core
{
    public static class LanguageManager
    {
        public static event EventHandler LanguageChanged;

        public static void SetLanguage(string cultureName)
        {
            try
            {
                CultureInfo culture = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                // Ayarları kaydetme kısmı şimdilik kaldırıldı.
                // Settings.Default.UserLanguage = cultureName;
                // Settings.Default.Save();
                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}