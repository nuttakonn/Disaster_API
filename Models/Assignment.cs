namespace Disaster_API.Models
{
    public class Assignment
    {
        public string AreaId { get; set; }
        public string? TruckId { get; set; }
        public Dictionary<string, int>? ResourcesDelivered { get; set; }
        public string? Message { get; set; }
    }
}