namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class ForbiddensEntity
    {
        public const string TableName = "Forbiddens";

        public int Id { get; set; }

        public ProblemEntity Problem { get; set; }

        public int ProblemId { get; set; }

        public string Keywords { get; set; }
    }
}