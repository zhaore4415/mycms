using Cssao.Shared.Models.Admin;
using System.Net.Http.Json;

namespace Cssao.Admin.Client.Services
{
    // Services/NewsService.cs
    public class NewsService
    {
        private readonly HttpClient _http;

        public NewsService(HttpClient http)
        {
            _http = http;
        }

        // 获取新闻列表
        public async Task<List<NewsDto>> GetNewsListAsync()
        {
            return await _http.GetFromJsonAsync<List<NewsDto>>("api/admin/news");
        }

        // 获取单个新闻（用于编辑）
        public async Task<NewsDto> GetNewsByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<NewsDto>($"api/admin/news/{id}");
        }

        // 新增新闻
        public async Task<bool> CreateNewsAsync(CreateNewsRequest command)
        {
            var result = await _http.PostAsJsonAsync("api/admin/news/publish", command);
            return result.IsSuccessStatusCode;
        }

        // 更新新闻
        public async Task<bool> UpdateNewsAsync(int id, UpdateNewsRequest command)
        {
            var result = await _http.PutAsJsonAsync($"api/admin/news/{id}", command);
            return result.IsSuccessStatusCode;
        }

        // 删除新闻
        public async Task<bool> DeleteNewsAsync(int id)
        {
            var result = await _http.DeleteAsync($"api/admin/news/{id}");
            return result.IsSuccessStatusCode;
        }

        // 获取新闻分类
        public async Task<List<CategoryDto>> GetNewsCategoriesAsync()
        {
            return await _http.GetFromJsonAsync<List<CategoryDto>>($"api/admin/news/Categories");
        }
    }
}
