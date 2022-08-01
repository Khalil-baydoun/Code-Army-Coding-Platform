using System.Threading.Tasks;
using DataContracts.Comments;

namespace WebApi.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> AddComment(AddCommentRequest commentRequest);

        //Task<List<Comment>> GetComments(string continuationToken, int limit);
    }
}