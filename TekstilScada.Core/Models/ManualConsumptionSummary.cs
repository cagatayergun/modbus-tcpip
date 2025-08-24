namespace TekstilScada.Models
{
    public class ManualConsumptionSummary
    {
        public string Makine { get; set; }
        public string RaporAraligi { get; set; }
        public string ToplamManuelSure { get; set; }
        public double OrtalamaSicaklik { get; set; }
        public double OrtalamaDevir { get; set; }
        public decimal ToplamSuTuketimi_Litre { get; set; }
        public decimal ToplamElektrikTuketimi_kW { get; set; }
        public decimal ToplamBuharTuketimi_kg { get; set; }
    }
}