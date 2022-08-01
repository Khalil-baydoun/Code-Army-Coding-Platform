using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]

    public class SolutionEntity
    {
        [Key, ForeignKey("Problem")]
        public const string TableName = "Solution";

        public int ProblemId { get; set; }

        public string  SourceCode { get; set; }

        public int ProgLanguage { get; set; }  

        public virtual ProblemEntity Problem { get; set; }   
    }
}