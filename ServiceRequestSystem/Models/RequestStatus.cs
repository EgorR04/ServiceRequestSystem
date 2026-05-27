namespace ServiceRequestSystem.Models
{
    public class RequestStatus
    {
        public int RequestStatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}
