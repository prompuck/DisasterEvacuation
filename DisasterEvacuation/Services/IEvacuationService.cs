using Models;
namespace Services;
public interface IEvacuationService {
    void AddZone(EvacuationZone zone);
    void AddVehicle(Vehicle vehicle);
    List<Assignment> Plan();
    Status GetStatus(string zoneId);
    void UpdateStatus(string zoneId, int moved, string vehicleId);
    void Clear();
} 