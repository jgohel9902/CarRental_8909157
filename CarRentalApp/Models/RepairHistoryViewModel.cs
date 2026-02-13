namespace CarRentalApp.Models
{
    public class RepairHistoryViewModel
    {
        public int VehicleId { get; set; }
        public DateTime RepairDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
    }
}