using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TaskManager.Client.Services
{
    public class JwtStorageService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private const string Key = "jwt_token";

        public JwtStorageService(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public async Task SaveTokenAsync(string token)
        {
            await _sessionStorage.SetAsync(Key, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            var result = await _sessionStorage.GetAsync<string>(Key);
            return result.Success ? result.Value : null;
        }

        public async Task ClearTokenAsync()
        {
            await _sessionStorage.DeleteAsync(Key);
        }
    }
}
