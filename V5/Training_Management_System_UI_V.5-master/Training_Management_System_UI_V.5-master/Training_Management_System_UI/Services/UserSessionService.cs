using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Training_Management_System_UI.Services
{
    public class UserSessionService
    {
        private readonly HttpClient _http;

        public Guid UserId { get; private set; }
        public string Role { get; private set; } = string.Empty;
        public bool IsLoggedIn => UserId != Guid.Empty;

        public UserSessionService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> LoadSessionAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    Clear(); // ← Add this so IsLoggedIn resets properly
                    return false;
                }

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                UserId = Guid.Parse(json.GetProperty("userId").GetString()!);
                Role = json.GetProperty("role").GetString()!;
                return true;
            }
            catch
            {
                Clear(); // ← Add this so IsLoggedIn resets properly
                return false;
            }
        }

        public void Clear()
        {
            UserId = Guid.Empty;
            Role = string.Empty;
        }


    }
}