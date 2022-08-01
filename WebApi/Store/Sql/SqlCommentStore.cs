using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using DataContracts.Comments;
using DataContracts.Problems;
using DataContracts.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SqlMigrations;
using SqlMigrations.Entities;
using WebApi.Exceptions;
using WebApi.Store.Interfaces;

namespace WebApi.Store.Sql
{
    public class SqlCommentStore : ICommentStore
    {
        private readonly IMapper _mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        private readonly IServiceScopeFactory scopeFactory;

        public SqlCommentStore(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserEntity, User>();
                cfg.CreateMap<User, UserEntity>();
                cfg.CreateMap<ProblemEntity, Problem>();
                cfg.CreateMap<Problem, ProblemEntity>();
                cfg.CreateMap<CommentEntity, Comment>();
                cfg.CreateMap<Comment, CommentEntity>()
                    .ForMember(d => d.AuthorEmail, o => o.MapFrom(s => s.AuthorEmail));
            });
            _mapper = configuration.CreateMapper();
        }

        public async Task<Comment> AddComment(AddCommentRequest commentRequest)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemEntity = await db.Problems.FindAsync(int.Parse(commentRequest.problemId));
                var userEntity = await db.Users.FindAsync(commentRequest.userEmail);
                var CreatedAt = DateTime.Now;

                var commentEntity = new CommentEntity
                {
                    AuthorEmail = commentRequest.userEmail,
                    problemId = int.Parse(commentRequest.problemId),
                    Body = commentRequest.Body,
                    CreatedAt = CreatedAt,
                    problem = problemEntity,
                    Author = userEntity
                };
                if (problemEntity.Comments == null)
                {
                    problemEntity.Comments = new List<CommentEntity>();
                }
                problemEntity.Comments.Add(commentEntity);
                //await db.Comments.AddAsync(commentEntity);

                var success = await db.SaveChangesAsync() > 0;
                if (success)
                {
                    return new Comment
                    {
                        Body = commentEntity.Body,
                        AuthorEmail = userEntity.Email,
                        AuthorName = userEntity.FirstName + " " + userEntity.LastName,
                        CreatedAt = commentEntity.CreatedAt,
                        Id = commentEntity.Id
                    };
                }
                throw new Exception("Could not add comment");
            }
        }

        private CommentEntity ToCommentEntity(Comment comment)
        {
            var entity = _mapper.Map<CommentEntity>(comment);
            return entity;
        }
    }
}