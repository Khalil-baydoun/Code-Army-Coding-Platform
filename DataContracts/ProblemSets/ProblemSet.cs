using System;
using System.Collections.Generic;
using DataContracts.DueDates;
using DataContracts.Problems;

namespace DataContracts.ProblemSets
{
    public class ProblemSet
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AuthorEmail { get; set; }

        public string Description { get; set; }

        public string[] Prerequisites { get; set; }

        public List<Problem> Problems { get; set; }

        public List<DueDate> dueDates { get; set; }

        public int CourseId { get; set; }
    };
}