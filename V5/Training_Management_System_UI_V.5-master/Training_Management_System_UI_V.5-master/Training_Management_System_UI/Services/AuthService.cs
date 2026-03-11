using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Training_Management_System_UI.Models.Auth;

namespace Training_Management_System_UI.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private string? _csrfToken;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<AuthResponse> LoginAsync(AuthRequest request)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/auth/login");
                requestMessage.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                requestMessage.Content = JsonContent.Create(request);

                var response = await _http.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    _csrfToken = result.GetProperty("csrfToken").GetString();

                    _http.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
                    _http.DefaultRequestHeaders.Add("X-CSRF-TOKEN", _csrfToken);

                    return new AuthResponse { IsSuccess = true, Message = "Login Successful", CsrfToken = _csrfToken };
                }

                var error = await response.Content.ReadAsStringAsync();
                return new AuthResponse { IsSuccess = false, Message = error };
            }
            catch (Exception ex)
            {
                return new AuthResponse { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<AuthResponse> LogoutAsync()
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
                requestMessage.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

                var response = await _http.SendAsync(requestMessage);

                _http.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
                _csrfToken = null;

                return new AuthResponse
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    Message = await response.Content.ReadAsStringAsync()
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}