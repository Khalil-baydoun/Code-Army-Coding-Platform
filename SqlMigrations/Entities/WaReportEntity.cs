using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]

    public class WaReportEntity
    {
        public const string TableName = "WaReports";

        [Key, ForeignKey("SubmissionStatistics")]
        public int SubmissionStatisticsId { get; set; }

        public string ActualOutput { get; set; }

        public string ExpectedOutput { get; set; }

        public string Input { get; set; }
    }
}