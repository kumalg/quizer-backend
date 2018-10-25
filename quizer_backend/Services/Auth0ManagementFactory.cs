using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace quizer_backend.Services {
    public class Auth0ManagementFactory {
        private readonly IConfiguration _externalAuthOptions;
        private readonly HttpClient _client;

        private DateTimeOffset _tokenLastRetrieved;
        private TimeSpan _tokenExpiration;
        private string _accessToken;

        private ManagementApiClient _managementApiClient;

        public Auth0ManagementFactory(IConfiguration externalAuthOptions) {
            _externalAuthOptions = externalAuthOptions;
            _tokenExpiration = TimeSpan.FromSeconds(double.Parse(_externalAuthOptions["Auth0:JwtExpirationInSeconds"]));
            _client = new HttpClient();
        }

        public async Task<ManagementApiClient> GetManagementApiClientAsync() {
            if (_externalAuthOptions == null)
                return null;

            if (!string.IsNullOrEmpty(_accessToken) && _tokenLastRetrieved + _tokenExpiration > DateTimeOffset.UtcNow)
                return _managementApiClient ?? (_managementApiClient = new ManagementApiClient(_accessToken, _externalAuthOptions["Auth0:Domain"]));

            _managementApiClient = new ManagementApiClient(await GetManagementApiTokenAsync(), _externalAuthOptions["Auth0:Domain"]);

            return _managementApiClient;
        }

        public async Task<string> GetManagementApiTokenAsync() {
            if (_externalAuthOptions == null)
                return null;

            if (!string.IsNullOrEmpty(_accessToken) && _tokenLastRetrieved + _tokenExpiration > DateTimeOffset.UtcNow)
                return _accessToken;

            var obj = new Auth0TokenRequest {
                grant_type = "client_credentials",
                client_id = _externalAuthOptions["Auth0:ManagementClientId"],
                client_secret = _externalAuthOptions["Auth0:ManagementClientSecret"],
                audience = $"https://{_externalAuthOptions["Auth0:Domain"]}/api/v2/"
            };
            var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"https://{_externalAuthOptions["Auth0:Domain"]}/oauth/token", content);
            response.EnsureSuccessStatusCode();
            var accessTokenJson = await response.Content.ReadAsStringAsync();
            var accessTokenResponse = JsonConvert.DeserializeObject<Auth0TokenResponse>(accessTokenJson);
            _accessToken = accessTokenResponse.access_token;
            _tokenExpiration = TimeSpan.FromSeconds(accessTokenResponse.expires_in);
            _tokenLastRetrieved = DateTimeOffset.UtcNow;
            return _accessToken;
        }

        private class Auth0TokenRequest {
            public string grant_type { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string audience { get; set; }
            public string scope { get; set; }
        }

        private class Auth0TokenResponse {
            public string access_token { get; set; }
            public int expires_in { get; set; } // in seconds
            public string scope { get; set; }
            public string token_type { get; set; }
        }
    }
}