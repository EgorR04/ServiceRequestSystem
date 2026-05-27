namespace ServiceRequestSystem.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public string? Result { get; set; }

        public int ServiceObjectId { get; set; }

        public ServiceObject? ServiceObject { get; set; }

        public int CreatedByUserId { get; set; }

        public User? CreatedByUser { get; set; }

        public int? AssignedEngineerId { get; set; }

        public User? AssignedEngineer { get; set; }

        public int RequestStatusId { get; set; }

        public RequestStatus? RequestStatus { get; set; }

        public int RequestPriorityId { get; set; }

        public RequestPriority? RequestPriority { get; set; }

        public ICollection<RequestComment>? Comments { get; set; }

        public ICollection<RequestHistory>? History { get; set; }
    }
}
