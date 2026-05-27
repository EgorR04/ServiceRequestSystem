namespace ServiceRequestSystem.Models
{
    public class ServiceObject
    {
        public int ServiceObjectId { get; set; }

        public string ObjectName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? ContactPhone { get; set; }

        public string? Description { get; set; }

        public ICollection<ServiceRequest>? ServiceRequests { get; set; }

        public ICollection<Equipment>? Equipment { get; set; }
    }
}
