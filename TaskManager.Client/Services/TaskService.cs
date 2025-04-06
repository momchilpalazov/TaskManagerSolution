using System.Net.Http.Headers;
using TaskManager.Client.Models;

namespace TaskManager.Client.Services
{
    public class TaskService
    {
        private readonly HttpClient _http;

        public TaskService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TaskDto>?> GetAllAsync(string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync("https://localhost:5001/api/v1/task");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<TaskDto>>();
        }
    }
}
