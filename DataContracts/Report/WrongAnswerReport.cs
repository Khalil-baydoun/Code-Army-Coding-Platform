using DataContracts.Tests;

namespace DataContracts.Report
{
    public class WrongAnswerReport
    {
        public int Id {get; set;}

        public string ActualOutput { get; set; }

        public string ExpectedOutput { get; set; }

        public string Input { get; set; }
    }
}