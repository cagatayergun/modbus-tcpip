// Models/KpiViewModel.cs
namespace TekstilScada.Web.Models
{
    public class KpiViewModel
    {
        public int TotalMachineCount { get; set; }
        public int RunningMachineCount { get; set; }
        public int StoppedMachineCount { get; set; }
        public int AlarmedMachineCount { get; set; }
    }
}
