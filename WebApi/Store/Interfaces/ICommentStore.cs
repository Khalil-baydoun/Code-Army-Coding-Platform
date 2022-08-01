using System.Threading.Tasks;
using DataContracts.Comments;

namespace WebApi.Store.Interfaces
{
    public interface ICommentStore
    {   
        Task<Comment> AddComment(AddCommentRequest commentRequest);

        //Task<List<Comment>> GetComments(string continuationToken, int limit);

    }
}