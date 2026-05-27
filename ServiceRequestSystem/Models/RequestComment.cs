namespace ServiceRequestSystem.Models
{
    public class RequestComment
    {
        public int RequestCommentId { get; set; }

        public int ServiceRequestId { get; set; }

        public ServiceRequest? ServiceRequest { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public string CommentText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
