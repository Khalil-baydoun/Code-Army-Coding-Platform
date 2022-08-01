using System.Collections.Generic;

namespace DataContracts.ProblemSets
{
    public class AddProblemToProblemSetRequest
    {
        public int ProblemId { get; set; }

        public int ProblemSetId { get; set; }
    }
}