using DataContracts.Problems;
using DataContracts.Users;
using System;

namespace DataContracts.Comments
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Body { get; set; }

        public string AuthorName { get; set; } 

        public string AuthorEmail { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}