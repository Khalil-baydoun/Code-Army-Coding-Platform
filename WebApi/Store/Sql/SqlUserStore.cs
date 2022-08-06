using System.Data;
using DataContracts.Users;
using Microsoft.EntityFrameworkCore;
using SqlMigrations;
using SqlMigrations.Entities;
using Utilities;
using WebApi.Exceptions;
using WebApi.Store.Interfaces;

namespace WebApi.Store.Sql
{
    public class SqlUserStore : IUserStore
    {
        private readonly GlobalMapper _mapper;

        private readonly IServiceScopeFactory scopeFactory;

        public SqlUserStore(GlobalMapper _mapper, IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            this._mapper = _mapper;
        }

        public async Task AddUser(AddUserRequest userReq)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToUserEntity(userReq);
                    db.Users.Add(entity);
                    await db.SaveChangesAsync();

                    if (userReq.CourseIds != null)
                    {
                        foreach (var courseId in userReq.CourseIds)
                        {
                            db.CourseUsers.Add(new CourseUserEntity { CourseId = Int32.Parse(courseId), UserEmail = userReq.Email });
                        }
                        await db.SaveChangesAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }
            }
        }

        public User GetUser(string email)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var userEntity = db.Users
                    .Where(ps => ps.Email == email)
                    .Include(ps => ps.CourseUser)
                    .ThenInclude(ps => ps.Course)
                    .ToList().FirstOrDefault();
                var ownedCourses = db.Courses
                    .Where(ps => ps.AuthorEmail.Equals(email))
                    .ToList();
                if (userEntity == null)
                {
                    throw new NotFoundException($"User with email {email} was not found");
                }
                return _mapper.ToUser(userEntity, ownedCourses);
            }
        }

        public async Task DeleteUser(string email)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Users.Remove(db.Users.Where(x => x.Email.Equals(email)).FirstOrDefault());
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not delete user");
                }
            }
        }

        public async Task UpdateUser(AddUserRequest user)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToUserEntity(user);
                var target = db.Users
                    .Where(ps => ps.Email == user.Email)
                    .ToList().FirstOrDefault();
                if (target != null)
                {
                    entity.CopyProperties(target);
                    var success = await db.SaveChangesAsync() > 0;
                    if (!success)
                    {
                        throw new Exception("Could not update user");
                    }
                }
                else
                {
                    throw new NotFoundException($"User with email {user.Email} was not found");
                }
            }
        }

        public async Task<bool> UserExists(string email)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return await db.Users.FindAsync(email) != null;
            }
        }
    }
}