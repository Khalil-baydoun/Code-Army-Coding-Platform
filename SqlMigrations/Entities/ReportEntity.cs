using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]

    public class ReportEntity
    {
        public const string TableName = "Reports";
        public int Id { get; set; }
        public virtual SubmissionStatisticsEntity SubmissionStatistics {get; set;}

        public virtual WaReportEntity WaReport {get; set;}

        public string StaticCodeAnalysis { get; set; }        
    }
}
