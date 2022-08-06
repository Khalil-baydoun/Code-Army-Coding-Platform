using System.Security.Claims;
using DataContracts.Comments;
using Microsoft.AspNetCore.SignalR;
using WebApi.Services.Interfaces;

namespace WebApi.SignalR
{
    public class ChatHub : Hub
    {
        private readonly ICommentService _commentService;
        public ChatHub(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task SendComment(string Body, int problemId)
        {
            var username = Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            var comment = await _commentService.AddComment(new AddCommentRequest
            {
                userEmail = username,
                problemId = problemId.ToString(),
                Body = Body
            });
            await Clients.All.SendAsync("ReceiveComment", comment);
        }
    }
    public class CommentHubRequest
    {
        public string Body { get; set; }
        public string problemId { get; set; }
    }
}