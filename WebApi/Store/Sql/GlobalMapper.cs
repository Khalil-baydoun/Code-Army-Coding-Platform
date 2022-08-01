using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataContracts.Comments;
using DataContracts.Courses;
using DataContracts.DueDates;
using DataContracts.Groups;
using DataContracts.Problems;
using DataContracts.ProblemSets;
using DataContracts.Statistics;
using DataContracts.Submissions;
using DataContracts.Report;
using DataContracts.Tests;
using DataContracts.Users;
using SqlMigrations.Entities;
using DataContracts.Forbiddens;

public class GlobalMapper
{
    IMapper _mapper;
    public GlobalMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProblemSetEntity, ProblemSet>();
            cfg.CreateMap<ProblemSet, ProblemSetEntity>();
            cfg.CreateMap<AddProblemSetRequest, ProblemSetEntity>();
            cfg.CreateMap<ProblemEntity, Problem>();
            cfg.CreateMap<SubmissionStatistics, SubmissionStatisticsEntity>();
            cfg.CreateMap<SubmissionStatisticsEntity, SubmissionStatistics>();
            cfg.CreateMap<UserEntity, User>();
            cfg.CreateMap<User, UserEntity>();
            cfg.CreateMap<CreateProblemRequest, Problem>();
            cfg.CreateMap<AddUserRequest, UserEntity>();
            cfg.CreateMap<TestEntity, TestUnit>();
            cfg.CreateMap<TestUnit, TestEntity>();
            cfg.CreateMap<CourseEntity, Course>().ForMember(x => x.ProblemSets, y => y.Ignore()); ;
            cfg.CreateMap<Course, CourseEntity>();
            cfg.CreateMap<ProblemSetEntity, ProblemSet>();
            cfg.CreateMap<ProblemEntity, Problem>();
            cfg.CreateMap<Problem, ProblemEntity>();
            cfg.CreateMap<UpdateProblemRequest, Problem>();
            cfg.CreateMap<UpdateCourseRequest, Course>();
            cfg.CreateMap<UpdateProblemSetRequest, ProblemSet>();
            cfg.CreateMap<SolutionRequest, SubmissionRequest>();
            cfg.CreateMap<SubmissionRequest, SolutionEntity>();
            cfg.CreateMap<WrongAnswerReport, WaReportEntity>();
            cfg.CreateMap<WaReportEntity, WrongAnswerReport>();
            cfg.CreateMap<ReportEntity, Report>();
            cfg.CreateMap<Report, ReportEntity>();
            cfg.CreateMap<CommentEntity, Comment>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.FirstName + " " + s.Author.LastName));
            cfg.CreateMap<GroupEntity, Group>();
            cfg.CreateMap<Group, GroupEntity>();
            cfg.CreateMap<DueDateEntity, DueDate>();
            cfg.CreateMap<DueDate, DueDateEntity>();
        });
        _mapper = configuration.CreateMapper();
    }

    public UserEntity ToUserEntity(User user)
    {
        return _mapper.Map<UserEntity>(user);
    }

    public UserEntity ToUserEntity(AddUserRequest req)
    {
        return _mapper.Map<UserEntity>(req);
    }

    public Problem ToProblem(UpdateProblemRequest req)
    {
        return _mapper.Map<Problem>(req);
    }

    public Course ToCourse(UpdateCourseRequest req)
    {
        return _mapper.Map<Course>(req);
    }

    public ProblemSet ToProblemSet(UpdateProblemSetRequest req)
    {
        return _mapper.Map<ProblemSet>(req);
    }

    public Problem ToProblem(CreateProblemRequest req)
    {
        return _mapper.Map<Problem>(req);
    }

    public User ToUser(UserEntity entity, List<CourseEntity> ownedCourses)
    {
        if (entity == null)
        {
            return null;
        }

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CourseEntity, Course>();
        });
        var _courseMapper = configuration.CreateMapper();

        var user = _mapper.Map<User>(entity);
        user.Courses = entity.CourseUser.Select(x => _courseMapper.Map<Course>(x.Course)).ToList();
        user.Courses.AddRange(ownedCourses.Select(x => _courseMapper.Map<Course>(x)).ToList());
        return user;
    }

    public CourseEntity ToCourseEntity(Course Course)
    {
        var entity = _mapper.Map<CourseEntity>(Course);
        return entity;
    }

    public SubmissionStatisticsEntity ToSubmissionStatisticsEntity(SubmissionStatistics subStat)
    {
        var entity = _mapper.Map<SubmissionStatisticsEntity>(subStat);
        return entity;
    }

    public SubmissionStatistics ToSubmissionStatistics(SubmissionStatisticsEntity subStat)
    {
        var entity = _mapper.Map<SubmissionStatistics>(subStat);
        return entity;
    }

    public Course ToCourse(CourseEntity entity)
    {
        if (entity == null)
        {
            return null;
        }
        var course = _mapper.Map<Course>(entity);
        course.Id = entity.Id;
        course.UsersEmails = entity.CourseUser.Select(x => x.UserEmail).ToList();
        course.ProblemSets = entity.ProblemSets.Select(x => ToProblemSet(x)).ToList();
        return course;
    }

    public ProblemSet ToProblemSet(ProblemSetEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var problemSet = _mapper.Map<ProblemSet>(entity);
        problemSet.Id = entity.Id;
        if (entity.Prerequisites != null) problemSet.Prerequisites = entity.Prerequisites.Split(',');
        problemSet.Problems = entity.Problems.Select(x => ToProblem(x)).ToList();
        return problemSet;
    }

    public Problem ToProblem(ProblemEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var problem = _mapper.Map<Problem>(entity);
        problem.Id = entity.Id;
        problem.Tags = entity.Tags.Split(',');
        problem.Hints = entity.Hints.Split(',');
        return problem;
    }

    public ProblemSetEntity ToProblemSetEntity(ProblemSet problemSet)
    {
        var entity = _mapper.Map<ProblemSetEntity>(problemSet);
        if (problemSet.Prerequisites != null)
            entity.Prerequisites = String.Join(',', problemSet.Prerequisites);
        return entity;
    }

    public ProblemSetEntity ToProblemSetEntity(AddProblemSetRequest problemSet)
    {
        var entity = _mapper.Map<ProblemSetEntity>(problemSet);
        if (problemSet.Prerequisites != null)
            entity.Prerequisites = String.Join(',', problemSet.Prerequisites);
        return entity;
    }

    public ProblemEntity ToProblemEntity(Problem problem)
    {
        var entity = _mapper.Map<ProblemEntity>(problem);
        entity.Tags = String.Join(',', problem.Tags);
        entity.Hints = String.Join(',', problem.Hints);
        return entity;
    }

    public ReportEntity ToReportEntity(Report report)
    {
        var entity = _mapper.Map<ReportEntity>(report);
        if (report.StaticCodeAnalysis != null)
        {
            entity.StaticCodeAnalysis = String.Join(',', report.StaticCodeAnalysis);
        }
        return entity;
    }

    public Report ToReport(ReportEntity entity)
    {
        var report = _mapper.Map<Report>(entity);
        report.Id = report.Id;
        if (entity.StaticCodeAnalysis != null)
        {
            report.StaticCodeAnalysis = entity.StaticCodeAnalysis.Split(',');
        }
        return report;
    }

    public TestEntity ToTestEntity(TestUnit test)
    {
        var entity = _mapper.Map<TestEntity>(test);
        return entity;
    }

    public TestUnit ToTest(TestEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var test = _mapper.Map<TestUnit>(entity);
        return test;
    }

    public SubmissionRequest ToSubmissionRequest(SolutionRequest solutionResquest)
    {

        var submission = _mapper.Map<SubmissionRequest>(solutionResquest);

        return submission;
    }

    public SolutionEntity ToSolutionEntity(SubmissionRequest submissionRequest)
    {

        var solution = _mapper.Map<SolutionEntity>(submissionRequest);

        return solution;
    }

    public Group ToGroup(GroupEntity groupEntity)
    {
        var group = _mapper.Map<Group>(groupEntity);
        return group;
    }

    public DueDate ToDueDate(DueDateEntity dueDateEntity)
    {
        var dueDate = _mapper.Map<DueDate>(dueDateEntity);
        return dueDate;
    }
    public WaReportEntity ToWaReportEntity(WrongAnswerReport wrongAnswerReport)
    {
        var report = _mapper.Map<WaReportEntity>(wrongAnswerReport);

        return report;
    }

    public WrongAnswerReport ToWrongAnswerReport(WaReportEntity waReportEntity)
    {
        var report = _mapper.Map<WrongAnswerReport>(waReportEntity);

        return report;
    }

    private Comment ToComment(CommentEntity entity)
    {
        var comment = _mapper.Map<Comment>(entity);
        comment.AuthorName = entity.Author.FirstName + " " + entity.Author.LastName;
        return comment;
    }

    public ForbiddensEntity ToForbiddensEntity(Forbiddens forbiddnes)
    {
        var entity = _mapper.Map<ForbiddensEntity>(forbiddnes);
        return entity;
    }
    public Forbiddens ToForbiddens(ForbiddensEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var forbiddens = _mapper.Map<Forbiddens>(entity);
        return forbiddens;
    }
}