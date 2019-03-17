using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Renamer.Services.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Renamer.Services {
    public class TVDBRetrieverService {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _baseUri;
        private readonly UriBuilder _uriBuilder;
        public TVDBRetrieverService(IHttpClientFactory clientFactory) {
            _clientFactory = clientFactory;
            _baseUri = "https://api.thetvdb.com";
            _uriBuilder = new UriBuilder("https", "api.thetvdb.com");
        }

        public async Task<string> FetchToken(TVDBAuthenticator authenticator) {
            string myToken = null;
            string jsonAuth = JsonConvert.SerializeObject(authenticator);

            var stringContent = new StringContent(jsonAuth, UnicodeEncoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient();
            try {
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = GetLoginUri();
                var response = await client.PostAsync(GetLoginUri(), stringContent);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                TokenFromTVDBDto TokenObject = JsonConvert.DeserializeObject<TokenFromTVDBDto>(stringResult);
                myToken = TokenObject.Token;
            }
            catch (Exception e) {
                Console.WriteLine($"{ e.Message }");
            }
            return myToken;
        }

        public Uri GetLoginUri() {
            _uriBuilder.Path = "login";
            return _uriBuilder.Uri;
        }

        public async Task<List<int>> FetchUserFavorites(string token) {
            List<int> favoritesFromApi = new List<int>();
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try {
                var response = await client.GetAsync(GetFavoritesUri());
                response.EnsureSuccessStatusCode();
                var stringResponse = await response.Content.ReadAsStringAsync();
                JsonConverter converter = new JsonConverter();
                favoritesFromApi = converter.ConvertFavoritesToDto(stringResponse);

            }
            catch (Exception e) {
                Console.WriteLine($"{ e.Message }");
            }
            return favoritesFromApi;
        }
        public Uri GetFavoritesUri() {
            _uriBuilder.Path = "user/favorites";
            return _uriBuilder.Uri;
        }

    }
}
