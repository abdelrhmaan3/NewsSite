using Client.News.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace Client.News.Controllers
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


        public async Task<IActionResult> News(int? cateogryId)
        {
            List<NewsVM> newsVMs = new List<NewsVM>();
            List<CategoryVM> categories = await GetCategoriesAsync(); 
        
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            if (cateogryId.HasValue)
            {
                newsVMs = await GetNewsByCateogryIdAsync(cateogryId);
            }
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.GetAsync("GetAllNews").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var newsString = await response.Content.ReadAsStringAsync();
                newsVMs = JsonSerializer.Deserialize<List<NewsVM>>(newsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(newsVMs);
            }
            return View(newsVMs);
        }
        private async Task<List<CategoryVM>> GetCategoriesAsync()
        {
            List<CategoryVM> categoryVM = new List<CategoryVM>();
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.GetAsync("GetAllCategorys").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var newsString = await response.Content.ReadAsStringAsync();
                categoryVM = JsonSerializer.Deserialize<List<CategoryVM>>(newsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return categoryVM;
            }

            return categoryVM;
        }

        private async Task<List<NewsVM>> GetNewsByCateogryIdAsync(int? cateogryId)
        {
            List<NewsVM> newsVMs = new List<NewsVM>();
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.GetAsync("GetNewsByCategoryId").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var newsString = await response.Content.ReadAsStringAsync();
                newsVMs = JsonSerializer.Deserialize<List<NewsVM>>(newsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return newsVMs;
            }
            return newsVMs;
        }
        public async Task<IActionResult> Show(int Id)
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.GetAsync($"GetItemById?ItemId={Id}").ConfigureAwait(false);
            NewsVM newVm = new NewsVM();
            if (response.IsSuccessStatusCode)
            {
                var newsString = await response.Content.ReadAsStringAsync();
                newVm = JsonSerializer.Deserialize<NewsVM>(newsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(newVm);
            }
            return View(newVm);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}