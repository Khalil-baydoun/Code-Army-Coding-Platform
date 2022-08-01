using System;

namespace DataContracts.DueDates
{
    public class DueDate
    {
        public int groupId { get; set; }
        public int problemSetId { get; set; }
        public DateTime dueDate { get; set; }
    }
}