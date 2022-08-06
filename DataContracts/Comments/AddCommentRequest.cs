namespace DataContracts.Comments
{
    public class AddCommentRequest
    {
        public string Body { get; set; }

        public string problemId { get; set; }
        
        public string userEmail { get; set; }
    }
}