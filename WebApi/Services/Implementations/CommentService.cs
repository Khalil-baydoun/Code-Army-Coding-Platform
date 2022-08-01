using System.Threading.Tasks;
using DataContracts.Comments;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentStore _commentStore;
        public CommentService(ICommentStore commentStore)
        {
            _commentStore = commentStore;
        }

        public async Task<Comment> AddComment(AddCommentRequest commentRequest)
        {
            var comment = await _commentStore.AddComment(commentRequest);
            return comment;
        }
    }
}