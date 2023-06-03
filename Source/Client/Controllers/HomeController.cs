using Client.Models;
using Client.News.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public async Task<IActionResult> News()
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            List<NewsVM> newsVMs = new List<NewsVM>();
            var response = await httpClient.GetAsync("GetAllNews").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var newsString = await response.Content.ReadAsStringAsync();
                newsVMs = JsonSerializer.Deserialize<List<NewsVM>>(newsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(newsVMs);
            }

            return View(newsVMs);
        }

        public async Task<IActionResult> View(int Id)
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.GetAsync($"api/GetItemById?Id=${Id}").ConfigureAwait(false);
            NewsVM newVm = new NewsVM();
            if (response.IsSuccessStatusCode)
            {
                var newsString = await response.Content.ReadAsStringAsync();
                newVm = JsonSerializer.Deserialize<NewsVM>(newsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(newVm);
            }
            return View(newVm);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}