// UI/Controls/KurutmaReçete_Control.cs
using System;
using System.Windows.Forms;
using TekstilScada.Models;

namespace TekstilScada.UI.Controls
{
    public partial class KurutmaReçete_Control : UserControl
    {
        private ScadaRecipeStep _recipeStep;
        public event EventHandler ValueChanged;

        public KurutmaReçete_Control()
        {
            InitializeComponent();
            numSicaklik.ValueChanged += OnValueChanged;
            numNem.ValueChanged += OnValueChanged;
            numZaman.ValueChanged += OnValueChanged;
            numCalismaDevri.ValueChanged += OnValueChanged;
            numSogutmaZamani.ValueChanged += OnValueChanged;
            chkNemAktif.CheckedChanged += OnValueChanged;
            chkZamanAktif.CheckedChanged += OnValueChanged;
        }

        public void LoadRecipe(ScadaRecipe recipe)
        {
            if (recipe != null && recipe.Steps.Count > 0)
            {
                _recipeStep = recipe.Steps[0];
                var kurutmaParams = new KurutmaParams(_recipeStep.StepDataWords);
                // Değerleri PLC hafıza haritasına göre kontrollerden oku
                // Word0 = Sıcaklık, Word1 = Nem, Word2 = Zaman
                // Word3 = Çalışma Devri, Word4 = Soğutma Zamanı
                numSicaklik.Value = kurutmaParams.Temperature / 10.0m;
                numNem.Value = kurutmaParams.Humidity;
                numZaman.Value = kurutmaParams.DurationMinutes;
                numCalismaDevri.Value = kurutmaParams.Rpm;
                numSogutmaZamani.Value = kurutmaParams.CoolingTimeMinutes;

                // Kontrol bitlerini oku (Word 5)
                chkNemAktif.Checked = kurutmaParams.HumidityControlActive;
                chkZamanAktif.Checked = kurutmaParams.TimeControlActive;
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            if (_recipeStep == null) return;

            var kurutmaParams = new KurutmaParams(_recipeStep.StepDataWords);

            // Değişiklikleri anında _recipeStep nesnesine kaydet
            kurutmaParams.Temperature = (short)(numSicaklik.Value * 10);
            kurutmaParams.Humidity = (short)numNem.Value;
            kurutmaParams.DurationMinutes = (short)numZaman.Value;
            kurutmaParams.Rpm = (short)numCalismaDevri.Value;
            kurutmaParams.CoolingTimeMinutes = (short)numSogutmaZamani.Value;

            // Kontrol bitlerini yaz (Word 5)
            kurutmaParams.HumidityControlActive = chkNemAktif.Checked;
            kurutmaParams.TimeControlActive = chkZamanAktif.Checked;

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}