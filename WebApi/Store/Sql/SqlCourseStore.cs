using System.Data;
using System.Linq;
using AutoMapper;
using DataContracts.Courses;
using System;
using SqlMigrations.Entities;
using webapi.Store.Interfaces;
using SqlMigrations;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using DataContracts.ProblemSets;
using DataContracts.Problems;
using WebApi.Exceptions;
using System.Reflection;
using DataContracts.Groups;

namespace WebApi.Store.Sql
{
    public class SqlCourseStore : ICourseStore
    {
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory scopeFactory;

        public SqlCourseStore(GlobalMapper _mapper, IServiceScopeFactory scopeFactory)
        {
            this._mapper = _mapper;
            this.scopeFactory = scopeFactory;
        }

        public Course GetCourse(string courseId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var courseEntity = db.Courses
                    .Where(ps => ps.Id == Int32.Parse(courseId))
                    .Include(ps => ps.CourseUser)
                    .Include(ps => ps.ProblemSets)
                    .ThenInclude(ps => ps.DueDates)
                    .Include(ps => ps.ProblemSets)
                    .ThenInclude(ps => ps.Problems)
                    .ThenInclude(ps => ps.Comments)
                    .ThenInclude(ps => ps.Author)
                    .ToList().FirstOrDefault();
                if (courseEntity == null)
                {
                    throw new NotFoundException($"Course with id {courseId} was not found");
                }
                return _mapper.ToCourse(courseEntity);
            }
        }

        public async Task<string> AddCourse(Course course)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToCourseEntity(course);
                    db.Courses.Add(entity);
                    await db.SaveChangesAsync();
                    if (course.UsersEmails != null)
                    {
                        foreach (var userEmail in course.UsersEmails)
                        {
                            db.CourseUsers.Add(new CourseUserEntity { CourseId = entity.Id, UserEmail = userEmail });
                        }
                        await db.SaveChangesAsync();
                    }
                    transaction.Commit();
                    return entity.Id.ToString();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }
            }
        }

        public async Task UpdateCourse(Course course)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToCourseEntity(course);
                var target = db.Courses
                    .Where(ps => ps.Id == course.Id)
                    .ToList().FirstOrDefault();
                if (target != null)
                {
                    PropertyInfo[] destinationProperties = target.GetType().GetProperties();
                    foreach (PropertyInfo destinationPi in destinationProperties)
                    {
                        PropertyInfo sourcePi = entity.GetType().GetProperty(destinationPi.Name);
                        destinationPi.SetValue(target, sourcePi.GetValue(entity, null), null);
                    }
                    var success = await db.SaveChangesAsync() > 0;
                    if (!success)
                    {
                        throw new Exception("Could not update course");
                    }
                }
                else
                {
                    throw new NotFoundException($"Course with id {course.Id} was not found");
                }
            }
        }

        public async Task AddUserToCourse(int courseId, string userEmail)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.CourseUsers.Add(new CourseUserEntity { CourseId = courseId, UserEmail = userEmail });
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not add user to course");
                }
            }
        }

        public async Task AddUsersToCourse(int courseId, List<string> usersEmails)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    if (usersEmails != null)
                    {
                        foreach (var userEmail in usersEmails)
                        {
                            db.CourseUsers.Add(new CourseUserEntity { CourseId = courseId, UserEmail = userEmail.Trim() });
                        }
                    }
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }
            }
        }

        public List<Course> GetCourses(List<string> coursesIds)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var CourseEntities = db.Courses
                    .Where(ps => coursesIds.Contains(ps.Id.ToString()))
                    .Include(ps => ps.CourseUser)
                    .Include(ps => ps.ProblemSets)
                    .ThenInclude(ps => ps.Problems)
                    .ToList();
                return CourseEntities.Select(x => _mapper.ToCourse(x)).ToList();
            }
        }

        public async Task DeleteCourse(string courseId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var courseEntity = db.Courses
                    .First(ps => ps.Id == Int32.Parse(courseId));
                db.Courses.Remove(courseEntity);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not delete course");
                }
            }
        }

        public bool IsOwner(string courseId, string userEmail)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var CourseEntities = db.Courses.Where(ps => Int32.Parse(courseId) == ps.Id && userEmail.Equals(ps.AuthorEmail));
                return CourseEntities.Count() > 0;
            }
        }

        public bool IsMember(string courseId, string userEmail)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var courseEntities = db.Courses
                    .Where(ps => ps.Id == Int32.Parse(courseId))
                    .Include(ps => ps.CourseUser)
                    .ToList();
                if (courseEntities.Count != 1)
                {
                    throw new NotFoundException($"Course with id {courseId} was not found");
                }
                return courseEntities
                    .First()
                    .CourseUser.Select(x => x.UserEmail)
                    .Contains(userEmail);
            }
        }

        public List<Group> GetGroups(string courseId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var courseEntity = db.Courses
                    .Where(ps => ps.Id == Int32.Parse(courseId))
                    .Include(ps => ps.CourseUser)
                    .ThenInclude(cu => cu.User)
                    .ThenInclude(u => u.Group)
                    .ToList().FirstOrDefault();
                var groups = courseEntity.CourseUser
                    .GroupBy(x => x.User.Group)
                    .Select(x => _mapper.ToGroup(x.Key))
                    .ToList();
                return groups;
            }
        }
    }
}