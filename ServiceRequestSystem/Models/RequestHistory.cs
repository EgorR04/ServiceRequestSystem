namespace ServiceRequestSystem.Models
{
    public class RequestHistory
    {
        public int RequestHistoryId { get; set; }

        public int ServiceRequestId { get; set; }

        public ServiceRequest? ServiceRequest { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public string ActionType { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
