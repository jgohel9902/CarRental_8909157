using Maintenance.WebAPI.Models;

namespace Maintenance.WebAPI.Services
{
    public class FakeRepairHistoryService : IRepairHistoryService
    {
        public List<RepairHistoryDto> GetByVehicleId(int vehicleId)
        {
            return new List<RepairHistoryDto>
            {
                new RepairHistoryDto
                {
                    Id = 1,
                    VehicleId = vehicleId,
                    RepairDate = DateTime.Now.AddDays(-10),
                    Description = "Oil change",
                    Cost = 89.99m,
                    PerformedBy = "Quick Lube"
                },
                new RepairHistoryDto
                {
                    Id = 2,
                    VehicleId = vehicleId,
                    RepairDate = DateTime.Now.AddDays(-40),
                    Description = "Brake pad replacement",
                    Cost = 350.00m,
                    PerformedBy = "Auto Repair Pro"
                }
            };
        }
    }
}