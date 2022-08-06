using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]

    public class ReportEntity
    {
        public const string TableName = "Reports";
        
        [Key]
        public int Id { get; set; }

        [ForeignKey("SubmissionStatistics")]
        public int SubmissionStatisticsId { get; set; }

        [ForeignKey("WaReportId")]
        public int WaReportId { get; set; }

        public virtual SubmissionStatisticsEntity SubmissionStatistics {get; set;}

        public virtual WaReportEntity WaReport {get; set;}
    }
}
