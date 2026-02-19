using Maintenance.WebAPI.Models;

namespace Maintenance.WebAPI.Services;

public class FakeRepairHistoryService : IRepairHistoryService
{
    // Simple in-memory storage (stateful while the app is running)
    private static readonly List<RepairHistoryDto> _repairs = new();
    private static int _nextId = 1;

    public FakeRepairHistoryService()
    {
        // Seed once
        if (_repairs.Count == 0)
        {
            _repairs.Add(new RepairHistoryDto
            {
                Id = _nextId++,
                VehicleId = 101,
                RepairDate = DateTime.Now.AddDays(-10),
                Description = "Oil change",
                Cost = 89.99m,
                PerformedBy = "Quick Lube"
            });

            _repairs.Add(new RepairHistoryDto
            {
                Id = _nextId++,
                VehicleId = 101,
                RepairDate = DateTime.Now.AddDays(-3),
                Description = "Brake inspection",
                Cost = 59.99m,
                PerformedBy = "City Garage"
            });
        }
    }

    public List<RepairHistoryDto> GetByVehicleId(int vehicleId)
    {
        return _repairs
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.RepairDate)
            .ToList();
    }

    public RepairHistoryDto AddRepair(RepairHistoryDto repair)
    {
        repair.Id = _nextId++;
        if (repair.RepairDate == default) repair.RepairDate = DateTime.Now;

        _repairs.Add(repair);
        return repair;
    }
}