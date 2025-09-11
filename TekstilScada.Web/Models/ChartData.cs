using System.Collections.Generic;

namespace TekstilScada.Web.Models
{
    public class ChartData
    {
        public List<string> Labels { get; set; }
        public List<double> Values { get; set; }
        public string Color { get; set; }
    }
}