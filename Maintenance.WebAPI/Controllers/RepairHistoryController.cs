using Maintenance.WebAPI.Models;
using Maintenance.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepairHistoryController : ControllerBase
{
    private readonly IRepairHistoryService _repairService;
    private readonly Dictionary<string, int> _usageCounts;

    public RepairHistoryController(IRepairHistoryService repairService, Dictionary<string, int> usageCounts)
    {
        _repairService = repairService;
        _usageCounts = usageCounts;
    }

    // GET api/RepairHistory/vehicles/101/repairs
    [HttpGet("vehicles/{vehicleId}/repairs")]
    public IActionResult GetRepairHistory(int vehicleId)
    {
        var repairs = _repairService.GetByVehicleId(vehicleId);
        return Ok(repairs);
    }

    // POST api/RepairHistory
    [HttpPost]
    public IActionResult AddRepair([FromBody] RepairHistoryDto repair)
    {
        // Validation required by lab
        if (repair.VehicleId <= 0)
        {
            return BadRequest(new { error = "InvalidParameter", message = "VehicleId must be greater than zero." });
        }
        if (string.IsNullOrWhiteSpace(repair.Description))
        {
            return BadRequest(new { error = "InvalidParameter", message = "Description must not be empty." });
        }
        if (repair.Cost < 0)
        {
            return BadRequest(new { error = "InvalidParameter", message = "Cost cannot be negative." });
        }

        var created = _repairService.AddRepair(repair);

        return CreatedAtAction(
            nameof(GetRepairHistory),
            new { vehicleId = created.VehicleId },
            created
        );
    }

    // GET api/RepairHistory/crash
    [HttpGet("crash")]
    public IActionResult Crash()
    {
        int x = 0;
        int y = 5 / x; // will throw
        return Ok();
    }

    // GET api/RepairHistory/usage
    [HttpGet("usage")]
    public IActionResult Usage()
    {
        var key = Request.Headers["X-Api-Key"].ToString();

        if (!_usageCounts.ContainsKey(key))
            _usageCounts[key] = 0;

        _usageCounts[key]++;

        return Ok(new
        {
            clientId = key,
            callCount = _usageCounts[key]
        });
    }
}