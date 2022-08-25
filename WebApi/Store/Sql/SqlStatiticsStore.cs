using SqlMigrations;
using WebApi.Store.Interfaces;
using DataContracts.Submissions;
using DataContracts.Statistics;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Store.Sql
{
    public class SqlStatisticsStore : IStatisticsStore
    {
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory scopeFactory;

        public SqlStatisticsStore(GlobalMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _mapper = mapper;
            this.scopeFactory = scopeFactory;
        }

        public UserStatistics GetUserStatistics(string userEmail)
        {
            using (var scope = scopeFactory.CreateScope())
            {

                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var submissions = db.SubmissionStatistics
                    .Where(stat => userEmail.Equals(stat.UserEmail))
                    .ToList();

                return new UserStatistics()
                {
                    UserEmail = userEmail,
                    NumberOfAcceptedSubmissions = submissions.Count(sub => sub.Verdict == (int)Verdict.Accepted),
                    NumberOfProblemsAttempted = submissions.Select(sub => sub.ProblemId).Distinct().Count(),
                    NumberOfSubmissions = submissions.Count(),
                    NumberOfSolvedProblems = submissions.Where(sub => sub.Verdict == (int)Verdict.Accepted).Select(sub => sub.ProblemId).Distinct().Count(),
                    VerdictCounts = submissions.Select(sub => sub.Verdict).GroupBy(x => x).Select(x => new KeyValuePair<int, int>(x.Key, x.Count())).ToList()
                };
            }
        }

        public CourseStatistics GetCourseStatistics(int courseId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var course = db.Courses.Where(course => course.Id == courseId)
                    .Include(course => course.CourseUser)
                    .Include(course => course.ProblemSets)
                    .ThenInclude(ps => ps.ProblemSetProblems)
                    .ThenInclude(ps => ps.Problem)
                    .ToList();

                var problemAndProblemSetsIds = course
                    .SelectMany(p => p.ProblemSets)
                    .SelectMany(p => p.ProblemSetProblems)
                    .Select(p => new
                    {
                        p.ProblemId,
                        p.ProblemSetId,
                    })
                    .ToList();

                var userEmails = course
                    .SelectMany(p => p.CourseUser)
                    .Select(cu => cu.UserEmail)
                    .ToList();

                var problemSetIds = course
                    .SelectMany(p => p.ProblemSets)
                    .Select(cu => cu.Id)
                    .ToList();

                var submissions = db.SubmissionStatistics
                    .Where(sub => problemAndProblemSetsIds.Select(p => p.ProblemId).Contains(sub.ProblemId))
                    .ToList();

                return new CourseStatistics()
                {
                    CourseId = courseId,
                    UserStatistics = userEmails.Select(userEmail => new UserStatistics()
                    {
                        UserEmail = userEmail,
                        NumberOfAcceptedSubmissions = submissions.Where(sub => sub.UserEmail.Equals(userEmail)).Count(sub => sub.Verdict == (int)Verdict.Accepted),
                        NumberOfProblemsAttempted = submissions.Where(sub => sub.UserEmail.Equals(userEmail)).Select(sub => sub.ProblemId).Distinct().Count(),
                        NumberOfSubmissions = submissions.Where(sub => sub.UserEmail.Equals(userEmail)).Count(),
                        NumberOfSolvedProblems = submissions.Where(sub => sub.UserEmail.Equals(userEmail) && sub.Verdict == (int)Verdict.Accepted).Select(sub => sub.ProblemId).Distinct().Count(),
                        VerdictCounts = submissions.Where(sub => sub.UserEmail.Equals(userEmail)).Select(sub => sub.Verdict).GroupBy(x => x).Select(x => new KeyValuePair<int, int>(x.Key, x.Count())).ToList()
                    }).ToList(),

                    ProblemSetStatistics = problemSetIds.Select(ps => new ProblemSetStatistics()
                    {
                        ProblemSetId = ps,
                        UserStatistics = userEmails.Select(userEmail =>
                        {
                            var problemIdsOfProblemSet = problemAndProblemSetsIds.Where(p => p.ProblemSetId == ps).Select(p => p.ProblemId).ToList();
                            return new UserProblemSetStatistics()
                            {
                                UserEmail = userEmail,
                                ProblemIdsSolved = submissions
                                .Where(sub => problemIdsOfProblemSet
                                .Contains(sub.ProblemId) && sub.Verdict == (int)Verdict.Accepted && sub.UserEmail == userEmail)
                                .Select(sub => sub.ProblemId)
                                .Distinct()
                                .ToList()
                            };
                        }).ToList(),
                    }).ToList()
                };
            }
        }

        public ProblemStatistics GetProblemStatistics(int problemId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var submissions = db.SubmissionStatistics
                    .Where(stat => problemId == stat.ProblemId)
                    .ToList();

                return new ProblemStatistics
                {
                    ProblemId = problemId,
                    NumberOfTimesAttempted = submissions.Count(),
                    NumberOfTimesSolved = submissions.Where(sub => sub.Verdict == (int)Verdict.Accepted).Count(),
                    VerdictCounts = submissions.Select(sub => sub.Verdict).GroupBy(x => x).Select(x => new KeyValuePair<int, int>(x.Key, x.Count())).ToList()
                };
            }
        }

        public async Task AddSubmission(SubmissionStatistics sub)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToSubmissionStatisticsEntity(sub);
                db.SubmissionStatistics.Add(entity);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not add submission");
                }
                sub.Id = entity.Id;
            }
        }

        public async Task UpdateSubmission(SubmissionStatistics sub)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = db.SubmissionStatistics.Where(ps => ps.Id == sub.Id).FirstOrDefault();
                entity.Verdict = sub.Verdict;
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not update submission");
                }
            }
        }

        public List<SubmissionStatistics> GetSubmissionsPaged(string userEmail, int offset, int limit)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return db.SubmissionStatistics
                    .Where(x => x.UserEmail.Equals(userEmail))
                    .OrderByDescending(x => x.Id)
                    .Skip(offset)
                    .Take(limit)
                    .Select(x => _mapper.ToSubmissionStatistics(x))
                    .ToList();
            }
        }
    }
}