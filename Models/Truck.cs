namespace Disaster_API.Models
{
    public class Truck
    {
        public string TruckId { get; set; }
        public Dictionary<string, int> AvailableResources { get; set; } = new();
        public Dictionary<string, int> TravelTimeToArea { get; set; } = new();
    }
}