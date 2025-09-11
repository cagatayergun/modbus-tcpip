using System.Collections.Generic;

namespace TekstilScada.Web.Models
{
    public class MachineGroupViewModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public List<MachineViewModel> Machines { get; set; }
    }

    public class MachineViewModel
    {
        public string MachineName { get; set; }
        public string Status { get; set; }
        public string RecipeName { get; set; }
        public string BatchId { get; set; }
        public string Temperature { get; set; }
        public string RpmValue { get; set; }
    }
}