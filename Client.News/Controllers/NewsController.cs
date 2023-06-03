using Client.News.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Client.News.Controllers
{
        public class NewsController : Controller
        {

          
            private readonly IHttpClientFactory _httpClientFactory;
            public NewsController(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }


        /// <summary>
        /// ////
        ///  List<NewsVM> newsVMs = new List<NewsVM>();
        //        List<CategoryVM> categories = await GetCategoriesAsync();

        //        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
        //            if (cateogryId.HasValue)
        //            {
        //                newsVMs = await GetNewsByCateogryIdAsync(cateogryId);
        //    }
        //    var httpClient = _httpClientFactory.CreateClient("APIClient");
        //    var response = await httpClient.GetAsync("GetAllNews").ConfigureAwait(false);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var newsString = await response.Content.ReadAsStringAsync();
        //    newsVMs = JsonSerializer.Deserialize<List<NewsVM>>(newsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //return View(newsVMs);
        //            }
        //            return View(newsVMs);
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
            {
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                var response = await httpClient.GetAsync("GetAllNews").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var newsJson = await response.Content.ReadAsStringAsync();
                    var news = JsonConvert.DeserializeObject<IEnumerable<NewsVM>>(newsJson);
                    return View(news);
                }
                else
                {
                    return View("Error");
                }
            }

            public async Task<IActionResult> CreateAsync()
            {
                List<CategoryVM> categories = await GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
                return View();
            }



        [HttpPost]
        public async Task<IActionResult> Create(NewsVM news)
        {
         

            var newsJson = JsonConvert.SerializeObject(news);
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var content = new StringContent(newsJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"AddNews", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.GetAsync($"GetItemById?ItemId={id}").ConfigureAwait(false);
            List<CategoryVM> categories = await GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            if (response.IsSuccessStatusCode)
            {
                var newsJson = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<NewsVM>(newsJson);
                return View(news);
            }
            else
            {
                return View("Error");
            }
        }
        private async Task<List<CategoryVM>> GetCategoriesAsync()
        {
            List<CategoryVM> categoryVM = new List<CategoryVM>();
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.GetAsync("GetAllCategorys").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var newsString = await response.Content.ReadAsStringAsync();
                categoryVM = System.Text.Json.JsonSerializer.Deserialize<List<CategoryVM>>(newsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return categoryVM;
            }

            return categoryVM;
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewsVM news)
        {
            var newsJson = JsonConvert.SerializeObject(news);
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var content = new StringContent(newsJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"EditNews", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.GetAsync($"GetItemById?ItemId={id}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
           {
             var newsJson = await response.Content.ReadAsStringAsync();
              var news = JsonConvert.DeserializeObject<NewsVM>(newsJson);
              return View(news);
           }
           else
           { 
               return View("Error");
           }
       }

      

        public async Task<IActionResult> DeleteNews(int id)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(id.ToString()), "id");
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            var response = await httpClient.PostAsync($"/DeleteNews?NewsId={id}", null);

            // Handle the response as needed
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                Console.WriteLine("News deleted successfully.");
            }
            else
            {
                // Request failed
                Console.WriteLine("Failed to delete news. Status code: " + response.StatusCode);
            }
            return RedirectToAction("Index");
        }


       
    }
}
