using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
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

        public async Task<string> GetToken(TVDBAuthenticator authenticator) {
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
    }
}
