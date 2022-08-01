using System;
using System.Collections.Generic;
using DataContracts.DueDates;

namespace DataContracts.ProblemSets
{
    public class UpdateProblemSetRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Prerequisites { get; set; }
        public List<DueDate> dueDates { get; set; }
    };
}