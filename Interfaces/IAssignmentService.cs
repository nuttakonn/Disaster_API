using Disaster_API.Models;

namespace Disaster_API.Interfaces
{
    public interface IAssignmentService
    {
        Task<List<Assignment>> ProcessAssignmentsAsync();
        Task<List<Assignment>> GetAssignmentsAsync();

        Task ClearAssignmentsCacheAsync();
        bool AddArea(List<Area> area);
        bool AddTruck(List<Truck> truck);
    }
}