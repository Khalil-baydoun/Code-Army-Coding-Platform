using System;

namespace DataContracts.Submissions
{
    public class Submission
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public int ProblemId { get; set; }

        public int Verdict { get; set; }

        public string SourceCode { get; set; }

        public DateTime SubmittedAt { get; set; }

        public string ProgrammingLanguage { get; set; }

        public bool IsRetried { get; set; }

        public int TestsPassed { get; set; }
        
        public int TotalTests { get; set; }

        public string ActualOutput { get; set; }

        public string ExpectedOutput { get; set; }

        public string WrongTestInput { get; set; }

        public string RuntimeErrorMessage { get; set; }

        public string CompilerErrorMessage { get; set; }
    }
}