// using Disaster_API.Interfaces;
// using Disaster_API.Models;
// using Microsoft.Extensions.Caching.Memory;
// using StackExchange.Redis;

// namespace Disaster_API.Services
// {
//     public class AssignmentService : IAssignmentService
//     {
//         private readonly List<Area> _areas = new();
//         private readonly List<Truck> _trucks = new();
//         private const string CacheKey = "assignments";
//         private readonly IMemoryCache _cache;

//         // public AssignmentService(IMemoryCache cache)
//         // {
//         //     _cache = cache;
//         // }
        
        
//         public async Task<List<Assignment>> GetAssignmentsAsync()
//         {
//             if (_cache.TryGetValue(CacheKey, out List<Assignment> cached))
//             {
//                 return cached;
//             }

//             var assignments = await ProcessAssignmentsAsync();
//             _cache.Set(CacheKey, assignments, TimeSpan.FromMinutes(30));
//             return assignments;
//         }
//         public bool AddArea(Area area)
//         {
//             if (_areas.Any(a => a.AreaId == area.AreaId)) return false;
//             _areas.Add(area);
//             return true;
//         }


//         public bool AddTruck(Truck truck)
//         {
//             if (_trucks.Any(a => a.TruckId == truck.TruckId)) return false;
//             _trucks.Add(truck);
//             return true;
//         }


//         public async Task<List<Assignment>> ProcessAssignmentsAsync()
//         {
//             var sortedAreas = _areas.OrderByDescending(a => a.UrgencyLevel).ToList();
//             var assignments = new List<Assignment>();

//             foreach (var area in sortedAreas)
//             {
//                 var suitableTruck = _trucks.FirstOrDefault(truck =>
//                     truck.TravelTimeToArea.TryGetValue(area.AreaId, out var travelTime) &&
//                     travelTime <= area.TimeConstraintHours &&
//                     area.RequiredResources.All(r =>
//                         truck.AvailableResources.TryGetValue(r.Key, out var available) && available >= r.Value)
//                 );

//                 if (suitableTruck != null)
//                 {
//                     foreach (var res in area.RequiredResources)
//                     {
//                         suitableTruck.AvailableResources[res.Key] -= res.Value;
//                     }

//                     assignments.Add(new Assignment
//                     {
//                         AreaId = area.AreaId,
//                         TruckId = suitableTruck.TruckId,
//                         ResourcesDelivered = new Dictionary<string, int>(area.RequiredResources)
//                     });
//                 }
//                 else
//                 {
//                     assignments.Add(new Assignment
//                     {
//                         AreaId = area.AreaId,
//                         Message = "No available truck can fulfill the request within time/resource constraints."
//                     });
//                 }
//             }
    
//             _cache.Set(CacheKey, assignments, TimeSpan.FromMinutes(30));
//             return assignments;
//         }
//         public Task ClearAssignmentsCacheAsync()
//         {
//             _cache.Remove(CacheKey);
//             _areas.Clear();
//             _trucks.Clear();
//             return Task.CompletedTask;
//         }
//     }
// }

using Disaster_API.Interfaces;
using Disaster_API.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Disaster_API.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly List<Area> _areas = new();
        private readonly List<Truck> _trucks = new();
        private const string CacheKey = "assignments";

        private readonly IDatabase _redisDb;

        public AssignmentService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task<List<Assignment>> GetAssignmentsAsync()
        {
            var cachedData = await _redisDb.StringGetAsync(CacheKey);
            if (!cachedData.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<List<Assignment>>(cachedData)!;
            }

            var assignments = await ProcessAssignmentsAsync();
            var json = JsonConvert.SerializeObject(assignments);
            await _redisDb.StringSetAsync(CacheKey, json, TimeSpan.FromMinutes(30));
            return assignments;
        }

        public bool AddArea(List<Area> areas)
        {
            var newAreas = areas.Where(a => !_areas.Any(e => e.AreaId == a.AreaId)).ToList();
            _areas.AddRange(newAreas);
            return newAreas.Any();
        }

        public bool AddTruck(List<Truck> trucks)
        {
            var newTrucks = trucks.Where(t => !_trucks.Any(e => e.TruckId == t.TruckId)).ToList();
            _trucks.AddRange(newTrucks);
            return newTrucks.Any();
        }


        public async Task<List<Assignment>> ProcessAssignmentsAsync()
        {
            var sortedAreas = _areas.OrderByDescending(a => a.UrgencyLevel).ToList();
            var assignments = new List<Assignment>();

            foreach (var area in sortedAreas)
            {
                var suitableTruck = _trucks.FirstOrDefault(truck =>
                    truck.TravelTimeToArea.TryGetValue(area.AreaId, out var travelTime) &&
                    travelTime <= area.TimeConstraintHours &&
                    area.RequiredResources.All(r =>
                        truck.AvailableResources.TryGetValue(r.Key, out var available) && available >= r.Value)
                );

                if (suitableTruck != null)
                {
                    foreach (var res in area.RequiredResources)
                    {
                        suitableTruck.AvailableResources[res.Key] -= res.Value;
                    }

                    assignments.Add(new Assignment
                    {
                        AreaId = area.AreaId,
                        TruckId = suitableTruck.TruckId,
                        ResourcesDelivered = new Dictionary<string, int>(area.RequiredResources)
                    });
                }
                else
                {
                    assignments.Add(new Assignment
                    {
                        AreaId = area.AreaId,
                        Message = "No available truck can fulfill the request within time/resource constraints."
                    });
                }
            }

            var json = JsonConvert.SerializeObject(assignments);
            await _redisDb.StringSetAsync(CacheKey, json, TimeSpan.FromMinutes(30));
            return assignments;
        }

        public async Task ClearAssignmentsCacheAsync()
        {
            await _redisDb.KeyDeleteAsync(CacheKey);
            _areas.Clear();
            _trucks.Clear();
        }
    }
}
