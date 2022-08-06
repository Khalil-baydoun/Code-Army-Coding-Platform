using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]

    public class SolutionEntity
    {
        public const string TableName = "Solutions";

        [Key, ForeignKey("Problem")]
        public int ProblemId { get; set; }

        public string  SourceCode { get; set; }

        public string ProgLanguage { get; set; }  

        public virtual ProblemEntity Problem { get; set; }   
    }
}