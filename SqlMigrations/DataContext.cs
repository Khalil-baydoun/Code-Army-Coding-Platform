using System;
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
        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<SubmissionStatisticsEntity> SubmissionStatistics { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<DueDateEntity> DueDates { get; set; }

        public DbSet<WaReportEntity> WaReports {get; set; }

        public DbSet<ReportEntity> Reports {get; set; }
        public DbSet<ForbiddensEntity> Forbiddens { get; set; }
 

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubmissionStatisticsEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<ProblemEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<ProblemEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<ProblemEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<ProblemSetEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<ProblemEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<CourseEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<CourseEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<CommentEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<CommentEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<SolutionEntity>().HasKey(e => e.ProblemId);
            //  modelBuilder.Entity<SolutionEntity>().Property(e => e.Id).ValueGeneratedOnAdd(); //add foreignKey
           
            modelBuilder.Entity<ReportEntity>().HasKey(e => e.Id);

            modelBuilder.Entity<WaReportEntity>().HasKey(e => e.Id);
           // modelBuilder.Entity<WaReportEntity>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ForbiddensEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<ForbiddensEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<ReportEntity>()
                .HasOne(s => s.SubmissionStatistics)
                .WithOne(g => g.Report)
                .HasForeignKey<ReportEntity>(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WaReportEntity>()
                .HasOne(s => s.Report)
                .WithOne(g => g.WaReport)
                .HasForeignKey<WaReportEntity>(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentEntity>()
                .HasOne(c => c.problem)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.problemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentEntity>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorEmail)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DueDateEntity>()
                .HasKey(d => new { d.groupId, d.problemSetId });
            
            modelBuilder.Entity<DueDateEntity>()
                .HasOne(d => d.group)
                .WithMany()
                .HasForeignKey(d => d.groupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DueDateEntity>()
                .HasOne(d => d.problemSet)
                .WithMany(ps => ps.DueDates)
                .HasForeignKey(d => d.problemSetId)
                .OnDelete(DeleteBehavior.Cascade);

            // modelBuilder.Entity<ProblemProblemSetEntity>()
            //     .HasOne(pps => pps.Problem)
            //     .WithMany(pps => pps.ProblemProblemSets)
            //     .HasForeignKey(pps => pps.ProblemId);

            // modelBuilder.Entity<ProblemProblemSetEntity>()
            //     .HasOne(pps => pps.ProblemSet)
            //     .WithMany(pps => pps.ProblemProblemSets)
            //     .HasForeignKey(pps => pps.ProblemSetId);

            // Deleting a user will not delete course_user entries
            // Deleting a user will not delete related problems/problem sets/course
            // This is to avoid cascade cycles, and may be needed 

            modelBuilder.Entity<CourseUserEntity>()
                .HasKey(pps => new { pps.CourseId, pps.UserEmail });

            modelBuilder.Entity<CourseUserEntity>()
                .HasOne(pps => pps.User)
                .WithMany(pps => pps.CourseUser)
                .HasForeignKey(pps => pps.UserEmail)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CourseUserEntity>()
                .HasOne(pps => pps.Course)
                .WithMany(pps => pps.CourseUser)
                .HasForeignKey(pps => pps.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseEntity>()
                .HasMany(c => c.ProblemSets)
                .WithOne(e => e.Course)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProblemSetEntity>()
                .HasMany(c => c.Problems)
                .WithOne(e => e.ProblemSet)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProblemEntity>()
                .HasMany(c => c.Tests)
                .WithOne(e => e.Problem)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestEntity>()
                .HasOne<ProblemEntity>(s => s.Problem)
                .WithMany(g => g.Tests)
                .HasForeignKey(s => s.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProblemSetEntity>()
                .HasOne<CourseEntity>(s => s.Course)
                .WithMany(g => g.ProblemSets)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseEntity>()
                .HasOne<UserEntity>(s => s.Author)
                .WithMany()
                .HasForeignKey(s => s.AuthorEmail)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SubmissionStatisticsEntity>()
                .HasOne<UserEntity>(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserEmail)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubmissionStatisticsEntity>()
                .HasOne<ProblemEntity>(s => s.Problem)
                .WithMany()
                .HasForeignKey(s => s.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProblemSetEntity>()
                .HasOne<UserEntity>(s => s.Author)
                .WithMany()
                .HasForeignKey(s => s.AuthorEmail)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserEntity>()
                .HasOne<GroupEntity>(u => u.Group)
                .WithMany()
                .HasForeignKey(u => u.GroupId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<ProblemEntity>()
                .HasOne<UserEntity>(s => s.Author)
                .WithMany()
                .HasForeignKey(s => s.AuthorEmail)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProblemEntity>()
            .HasOne<ProblemSetEntity>(s => s.ProblemSet)
            .WithMany(g => g.Problems)
            .HasForeignKey(s => s.ProblemSetId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<TestEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<UserEntity>().HasKey(e => e.Email);
            modelBuilder.Entity<UserEntity>().Property(e => e.Email);
            modelBuilder.Entity<UserEntity>().Property(e => e.FirstName).IsRequired();
            modelBuilder.Entity<UserEntity>().Property(e => e.LastName).IsRequired();
            modelBuilder.Entity<UserEntity>().Property(e => e.Password).IsRequired();
            modelBuilder.Entity<UserEntity>().Property(e => e.Role).HasDefaultValue(0).IsRequired();
        }
    }
}
