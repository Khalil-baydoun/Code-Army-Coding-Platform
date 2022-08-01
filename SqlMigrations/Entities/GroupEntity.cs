namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class GroupEntity
    {
        public const string TableName = "Groups";
        public int Id { get; set; }
        public string Name { get; set; }
    }
}