using DataContracts.Statistics;
using System.Collections.Generic;

namespace DataContracts.Report
{
    public class Report
    {
        public int Id { get; set; }
        public string[] StaticCodeAnalysis { get; set; }

        public WrongAnswerReport WaReport { get; set; }

    }
}