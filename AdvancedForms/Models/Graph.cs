using System.Collections.Generic;

namespace AdvancedForms.Models
{
    public class Graph
    {
        public List<FrequencyData> FreqData { get; set; }
        public Dictionary<string, string> FreqStatusColor { get; set; }
    }

    public class FrequencyData
    {
        public string State { get; set; }
        public Dictionary<string, int> freq { get; set; }
    }
}
