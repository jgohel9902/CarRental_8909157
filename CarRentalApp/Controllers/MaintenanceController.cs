using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using CarRentalApp.Models;

namespace CarRentalApp.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MaintenanceController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult History()
        {
            return View(new List<RepairHistoryViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> History(int vehicleId)
        {
            var client = _httpClientFactory.CreateClient("MaintenanceApi");

            var repairs = await client.GetFromJsonAsync<List<RepairHistoryViewModel>>(
                $"api/maintenance/vehicles/{vehicleId}/repairs");

            return View(repairs ?? new List<RepairHistoryViewModel>());
        }
    }
}