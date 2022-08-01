namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class TestEntity
    {
        public const string TableName = "Tests";

        public int Id { get; set; }

        public ProblemEntity Problem { get; set; }

        public int ProblemId { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }
    }
}