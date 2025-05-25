using Microsoft.AspNetCore.Mvc;
using Services;
using Models;
namespace Controllers;

[ApiController]
[Route("api/evacuation")]
public class DisasterEvacuationController : ControllerBase {
    private readonly IEvacuationService _svc;
    public DisasterEvacuationController(IEvacuationService svc) => _svc = svc;

    [HttpPost("zones")]
    public IActionResult AddZone([FromBody] EvacuationZone zone) {
        if (string.IsNullOrWhiteSpace(zone.ZoneID))
            return BadRequest(new { error = "Zone ID is required" });
        _svc.AddZone(zone);
        return Ok(new { data = zone });
    }

    [HttpPost("vehicles")]
    public IActionResult AddVehicle([FromBody] Vehicle vehicle) {
        if (string.IsNullOrWhiteSpace(vehicle.VehicleID))
            return BadRequest(new { error = "Vehicle ID is required" });
        _svc.AddVehicle(vehicle);
        return Ok(new { data = vehicle });
    }

    [HttpPost("plan")]
    public IActionResult Plan() {
        var result = _svc.Plan();
        if (result.Count == 0)
            return BadRequest(new { error = "No assignments could be made" });
        return Ok(new { data = result });
    }

    [HttpGet("status/{id}")]
    public IActionResult GetStatus(string id) {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "Zone ID is required" });
        return Ok(new { data = _svc.GetStatus(id) });
    }

    [HttpPut("update")]
    public IActionResult Update([FromBody] Status status) {
        if (string.IsNullOrWhiteSpace(status.ZoneId))
            return BadRequest(new { error = "Zone ID is required for update" });
        _svc.UpdateStatus(status.ZoneId, status.Evacuated, status.LastVehicle);
        return Ok(new { data = status });
    }

    [HttpDelete("clear")]
    public IActionResult Clear() {
        _svc.Clear();
        return Ok(new { data = "cleared" });
    }
} 