using Newtonsoft.Json;
using System.Text;

namespace Cosu_MVC.Models
{
    public class AuthRepository: IAuthRepository
    {
        private readonly HttpClient _httpClient;

        public AuthRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GetTokenAsync(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7029/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseData);
                return tokenResponse.Token;
            }

            throw new Exception("Failed to retrieve token.");
        }
    }
}
