using Cssao.Shared.Models.Admin;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Cssao.Admin.Client.Services
{
    // Services/NewsService.cs
    public class NewsService
    {
        private readonly HttpClient _http;
        private readonly ILogger<NewsService> _logger;

        public NewsService(HttpClient http)
        {
            _http = http;
        }

        //// 获取新闻列表
        //public async Task<List<NewsDto>> GetNewsListAsync()
        //{
        //    return await _http.GetFromJsonAsync<List<NewsDto>>("api/admin/news");
        //}

        // 获取新闻列表
        public async Task<PagedResultDto<NewsDto>> GetNewsListAsync(int pageIndex = 1, int pageSize = 10)
        {
            var url = $"api/admin/news?pageIndex={pageIndex}&pageSize={pageSize}";
            var result = await _http.GetFromJsonAsync<PagedResultDto<NewsDto>>(url);
            return result ?? new PagedResultDto<NewsDto>();
        }

        // 获取单个新闻（用于编辑）
        public async Task<NewsDto> GetNewsByIdAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/admin/news/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var news = JsonSerializer.Deserialize<NewsDto>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return news;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    _logger.LogError("获取新闻失败: {Status}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "反序列化新闻数据失败");
                return null;
            }
        }

        // 新增新闻
        public async Task<bool> CreateNewsAsync(CreateNewsRequest command)
        {
            command.PublishDate = DateTime.Now;
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
