using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]

    public class WaReportEntity
    {
        public const string TableName = "WaReport";
        public int Id { get; set; }

        public string ActualOutput { get; set; }

        public string ExpectedOutput { get; set; }

        public string Input { get; set; }  

        public virtual ReportEntity Report {get; set;}     
    }
}