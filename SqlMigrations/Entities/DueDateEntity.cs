using System;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class DueDateEntity
    {
        public const string TableName = "DueDates";
        public int groupId { get; set; }
        public GroupEntity group { get; set; }
        public ProblemSetEntity problemSet { get; set; }
        public int problemSetId { get; set; }
        public DateTime dueDate { get; set; }
    }
}