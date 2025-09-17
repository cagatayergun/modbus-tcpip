using System;
using System.Collections.Generic; // List için eklendi
using System.Drawing; // Color için eklendi
using System.Linq;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Localization;
using TekstilScada.Models;
using TekstilScada.Properties;
using TekstilScada.Repositories;
namespace TekstilScada.UI.Views
{
    public partial class RecipeOptimization_Control : UserControl
    {
        private RecipeRepository _recipeRepository;

        public RecipeOptimization_Control()
        {
            InitializeComponent();
            ApplyLocalization();
            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
        }

        public void InitializeControl(RecipeRepository recipeRepo)
        {
            _recipeRepository = recipeRepo;
        }
        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();

        }
        private void ApplyLocalization()
        {



            label1.Text = Resources.anarecete;
            label7.Text = Resources.karsilastirilacak;
            btnAnalyze.Text = Resources.analizet;
            label3.Text = Resources.ortalamasutuketimi;
            label4.Text = Resources.ortalamacevrimsuresi;
            label5.Text = Resources.ortalamaelektriktuketimi;
            label6.Text = Resources.ortalamabuhartuketimi;
            label14.Text = Resources.ortalamasutuketimi;
            label12.Text = Resources.ortalamacevrimsuresi;
            label10.Text = Resources.ortalamaelektriktuketimi;
            label8.Text = Resources.ortalamabuhartuketimi;
            label2.Text = Resources.gecmisuretimler;
        }
        private void RecipeOptimization_Control_Load(object sender, EventArgs e)
        {
            if (_recipeRepository != null)
            {
                var recipes = _recipeRepository.GetAllRecipes();
                var recipes2 = new List<ScadaRecipe>(recipes); // İkinci ComboBox için kopyasını oluştur

                cmbRecipes.DataSource = recipes;
                cmbRecipes.DisplayMember = "RecipeName";
                cmbRecipes.ValueMember = "Id";

                cmbRecipe2.DataSource = recipes2;
                cmbRecipe2.DisplayMember = "RecipeName";
                cmbRecipe2.ValueMember = "Id";
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (cmbRecipes.SelectedValue == null || cmbRecipe2.SelectedValue == null)
            {
                MessageBox.Show("Please select both recipes to analyze.", "Warning");
                return;
            }

            int recipe1Id = (int)cmbRecipes.SelectedValue;
            int recipe2Id = (int)cmbRecipe2.SelectedValue;

            // İlk reçete için verileri ve ortalamaları hesapla
            var history1 = _recipeRepository.GetRecipeUsageHistory(recipe1Id);
            var averages1 = CalculateAverages(history1);
            DisplayAverages(averages1, 1);

            // İkinci reçete için verileri ve ortalamaları hesapla
            var history2 = _recipeRepository.GetRecipeUsageHistory(recipe2Id);
            var averages2 = CalculateAverages(history2);
            DisplayAverages(averages2, 2);

            // Geçmiş tablosunu birleştirerek göster
            dgvHistory.DataSource = history1.Concat(history2).OrderByDescending(h => h.StartTime).ToList();

            // Sonuçları renklendirerek karşılaştır
            CompareAndHighlight(averages1, averages2);
        }

        // Ortalama hesaplama işini ayrı bir metoda taşıyalım
        private (double Water, double Electricity, double Steam, TimeSpan CycleTime) CalculateAverages(List<ProductionReportItem> history)
        {
            if (!history.Any())
                return (0, 0, 0, TimeSpan.Zero);

            double avgWater = history.Average(h => h.TotalWater);
            double avgElectricity = history.Average(h => h.TotalElectricity);
            double avgSteam = history.Average(h => h.TotalSteam);
            var avgCycleTime = TimeSpan.FromSeconds(history.Average(h => TimeSpan.Parse(h.CycleTime).TotalSeconds));

            return (avgWater, avgElectricity, avgSteam, avgCycleTime);
        }

        // Sonuçları ilgili panellere yazdıran metot
        private void DisplayAverages((double Water, double Electricity, double Steam, TimeSpan CycleTime) averages, int panelIndex)
        {
            if (panelIndex == 1)
            {
                lblAvgWater.Text = $"{averages.Water:F0} L";
                lblAvgElectricity.Text = $"{averages.Electricity:F1} kW";
                lblAvgSteam.Text = $"{averages.Steam:F1} kg";
                lblAvgCycleTime.Text = averages.CycleTime.ToString(@"hh\:mm\:ss");
            }
            else // Panel 2
            {
                lblAvgWater2.Text = $"{averages.Water:F0} L";
                lblAvgElectricity2.Text = $"{averages.Electricity:F1} kW";
                lblAvgSteam2.Text = $"{averages.Steam:F1} kg";
                lblAvgCycleTime2.Text = averages.CycleTime.ToString(@"hh\:mm\:ss");
            }
        }

        // Karşılaştırma ve renklendirme metodu
        private void CompareAndHighlight(
            (double Water, double Electricity, double Steam, TimeSpan CycleTime) avg1,
            (double Water, double Electricity, double Steam, TimeSpan CycleTime) avg2)
        {
            // Su Tüketimi Karşılaştırması
            HighlightLabel(lblAvgWater, lblAvgWater2, avg1.Water, avg2.Water);
            // Elektrik Tüketimi Karşılaştırması
            HighlightLabel(lblAvgElectricity, lblAvgElectricity2, avg1.Electricity, avg2.Electricity);
            // Buhar Tüketimi Karşılaştırması
            HighlightLabel(lblAvgSteam, lblAvgSteam2, avg1.Steam, avg2.Steam);
            // Çevrim Süresi Karşılaştırması
            HighlightLabel(lblAvgCycleTime, lblAvgCycleTime2, avg1.CycleTime.TotalSeconds, avg2.CycleTime.TotalSeconds);
        }

        // Label'ları renklendiren yardımcı metot
        private void HighlightLabel(Label label1, Label label2, double value1, double value2)
        {
            label1.BackColor = SystemColors.Control;
            label2.BackColor = SystemColors.Control;

            if (value1 == 0 || value2 == 0) return; // Veri olmayanları karşılaştırma

            if (value1 < value2)
            {
                label1.BackColor = Color.LightGreen;
                label2.BackColor = Color.LightCoral;
            }
            else if (value2 < value1)
            {
                label2.BackColor = Color.LightGreen;
                label1.BackColor = Color.LightCoral;
            }
        }
    }
}