using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using CarRentalApp.Models;

namespace CarRentalApp.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Customers
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            var customers = await client.GetFromJsonAsync<List<CustomerDto>>("/api/customer");

            return View(customers ?? new List<CustomerDto>());
        }
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");
            var customer = await client.GetFromJsonAsync<CustomerDto>($"/api/customer/{id}");

            if (customer == null) return NotFound();
            return View(customer);
        }
    }
}