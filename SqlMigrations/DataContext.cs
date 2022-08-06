using Microsoft.EntityFrameworkCore;
using SqlMigrations.Entities;

namespace SqlMigrations
{
    /// <summary>
    /// * To add a migration, navigate to the migration project and run:
    ///     dotnet ef migrations add MyMigration
    /// 
    /// * To view resulting SQL script, run:
    ///     dotnet ef migrations script
    /// 
    /// * To apply a migration, run:
    ///     dotnet ef database update
    /// 
    /// To change a migration before applying it, simply delete the corresponding files from visual studio and run (you can also create another migration to apply the changes):
    ///     dotnet ef migration add MyMigration
    /// </summary>
    public class DataContext : DbContext
    {
        public DbSet<ProblemEntity> Problems { get; set; }

        public DbSet<SolutionEntity> Solutions { get; set; }

        public DbSet<CourseEntity> Courses { get; set; }

        public DbSet<ProblemSetEntity> ProblemSets { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<TestEntity> Tests { get; set; }
        
        public DbSet<CourseUserEntity> CourseUsers { get; set; }
 
        public DbSet<SubmissionStatisticsEntity> SubmissionStatistics { get; set; }
        
        public DbSet<WaReportEntity> WaReports {get; set; }

        public DbSet<ReportEntity> Reports {get; set; }
 

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Setup Composite Keys
            modelBuilder.Entity<CourseUserEntity>()
                .HasKey(ent => new { ent.CourseId, ent.UserEmail });
            
            modelBuilder.Entity<ProblemEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<ProblemEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<ProblemEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<CourseEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<TestEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<UserEntity>().Property(e => e.FirstName).IsRequired();

            modelBuilder.Entity<UserEntity>().Property(e => e.LastName).IsRequired();

            modelBuilder.Entity<UserEntity>().Property(e => e.Password).IsRequired();

            modelBuilder.Entity<UserEntity>().Property(e => e.Role).HasDefaultValue(0).IsRequired();

            modelBuilder.Entity<UserEntity>()
                .HasMany<CourseUserEntity>(s => s.CourseUser)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserEmail)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserEntity>()
                .HasMany<ProblemSetEntity>()
                .WithOne(s => s.Author)
                .HasForeignKey(s => s.AuthorEmail)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserEntity>()
                .HasMany<ProblemEntity>()
                .WithOne(s => s.Author)
                .HasForeignKey(s => s.AuthorEmail)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserEntity>()
                .HasMany<SubmissionStatisticsEntity>()
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserEmail)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
