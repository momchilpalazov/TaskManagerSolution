using TaskManager.Client.Models;

namespace TaskManager.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly JwtStorageService _jwtStorage;

        public AuthService(HttpClient http, JwtStorageService jwtStorage)
        {
            _http = http;
            _jwtStorage = jwtStorage;
        }

        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:5001/api/v1/auth/login", loginDto);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (result is not null)
            {
                await _jwtStorage.SaveTokenAsync(result.Token);
                return true;
            }

            return false;
        }
    }
}
