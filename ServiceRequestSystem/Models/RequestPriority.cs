namespace ServiceRequestSystem.Models
{
    public class RequestPriority
    {
        public int RequestPriorityId { get; set; }

        public string PriorityName { get; set; } = string.Empty;

        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}
