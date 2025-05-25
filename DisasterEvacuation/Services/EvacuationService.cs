using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Helpers;
namespace Services;
public class EvacuationService : IEvacuationService {
    private readonly List<EvacuationZone> _zones = new();
    private readonly List<Vehicle> _vehicles = new();
    private readonly IDistributedCache _cache;

    public EvacuationService(IDistributedCache cache) => _cache = cache;

    public void AddZone(EvacuationZone zone) => _zones.Add(zone);
    public void AddVehicle(Vehicle vehicle) => _vehicles.Add(vehicle);

    public List<Assignment> Plan() {
        var assignments = new List<Assignment>();

        foreach (var zone in _zones.OrderByDescending(z => z.UrgencyLevel)) {
            foreach (var vehicle in _vehicles.OrderBy(v => GeoHelper.Distance(zone.LocationCoordinates.latitude, zone.LocationCoordinates.longitude, v.LocationCoordinates.latitude, v.LocationCoordinates.longitude))) {
                if (zone.NumberOfPeople == 0) break;

                var evac = Math.Min(vehicle.Capacity, zone.NumberOfPeople);
                var distance = GeoHelper.Distance(zone.LocationCoordinates.latitude, zone.LocationCoordinates.longitude, vehicle.LocationCoordinates.latitude, vehicle.LocationCoordinates.longitude);
                var eta = Math.Round(distance / vehicle.Speed * 60); // นาที

                assignments.Add(new Assignment {
                    ZoneID = zone.ZoneID,
                    VehicleID = vehicle.VehicleID,
                    ETA = eta + " minutes",
                    NumberOfPeople = evac
                });

                zone.NumberOfPeople = zone.NumberOfPeople - evac;
                UpdateStatus(zone.ZoneID, evac, vehicle.VehicleID);
            }
        }
        return assignments;
    }

    public Status? GetStatus(string zoneId) {
        var json = _cache.GetString(zoneId);
        return string.IsNullOrEmpty(json) ? new Status { ZoneId = zoneId } : JsonSerializer.Deserialize<Status>(json);
    }

    public void UpdateStatus(string zoneId, int moved, string vehicleId) {
        var status = GetStatus(zoneId);
        status.Evacuated = status.Evacuated + moved;
        status.Remaining = status.Remaining - moved;
        status.LastVehicle = vehicleId;
        _cache.SetString(zoneId, JsonSerializer.Serialize(status));
    }

    public void Clear() {
        _zones.Clear();
        _vehicles.Clear();
    }
} 