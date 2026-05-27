namespace ServiceRequestSystem.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }

        public string EquipmentName { get; set; } = string.Empty;

        public string EquipmentType { get; set; } = string.Empty;

        public string? SerialNumber { get; set; }

        public string? Location { get; set; }

        public int ServiceObjectId { get; set; }

        public ServiceObject? ServiceObject { get; set; }
    }
}
