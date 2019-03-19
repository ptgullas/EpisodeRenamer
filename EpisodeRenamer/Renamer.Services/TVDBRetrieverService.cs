using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Renamer.Services.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Serilog;

namespace Renamer.Services {
    public class TVDBRetrieverService {
        private readonly IHttpClientFactory _clientFactory;
        private readonly UriBuilder _uriBuilder;
        public TVDBRetrieverService(IHttpClientFactory clientFactory) {
            _clientFactory = clientFactory;
            _uriBuilder = new UriBuilder("https", "api.thetvdb.com");
        }

        public async Task<string> FetchNewToken(TVDBAuthenticator authenticator) {
            Log.Information("Fetching new token...");
            string myToken = null;
            string jsonAuth = JsonConvert.SerializeObject(authenticator);

            var stringContent = new StringContent(jsonAuth, UnicodeEncoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient();
            try {
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = GetLoginUri();
                var response = await client.PostAsync(GetLoginUri(), stringContent);
                response.EnsureSuccessStatusCode();
                Log.Information("Successfully fetched new token");
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
        public async Task<string> FetchRefreshToken(string token) {
            Log.Information("Fetching refresh token");
            string myToken = token;
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try {
                var response = await client.GetAsync(GetRefreshTokenUri());
                response.EnsureSuccessStatusCode();
                Log.Information("Successfully fetched refresh token");
                var stringResult = await response.Content.ReadAsStringAsync();
                TokenFromTVDBDto TokenObject = JsonConvert.DeserializeObject<TokenFromTVDBDto>(stringResult);
                myToken = TokenObject.Token;
            }
            catch (Exception e) {
                Log.Error($"{ e.Message }");
            }
            return myToken;
        }

        public Uri GetRefreshTokenUri() {
            _uriBuilder.Path = "refresh_token";
            return _uriBuilder.Uri;
        }

        public async Task<List<int>> FetchUserFavorites(string token) {
            Log.Information("Retrieving user favorites...");
            List<int> favoritesFromApi = new List<int>();
            try {
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Uri favoritesUri = GetFavoritesUri();
                var response = await client.GetAsync(favoritesUri);
                response.EnsureSuccessStatusCode();
                Log.Information("Successfully retrieved favorites");
                var stringResponse = await response.Content.ReadAsStringAsync();
                JsonConverter converter = new JsonConverter();
                favoritesFromApi = converter.ConvertFavoritesToDto(stringResponse);

            }
            catch (Exception e) {
                Log.Error($"{ e.Message }");
            }
            return favoritesFromApi;
        }
        public Uri GetFavoritesUri() {
            Log.Information("In GetFavoritesUri");
            _uriBuilder.Path = "user/favorites";
            return _uriBuilder.Uri;
        }

        public async Task<TVShowFromTVDBDto> FetchTVShow(int seriesId, string token) {
            Log.Information("Fetching TV Series with seriesId {a}", seriesId);
            TVShowFromTVDBDto showDto = new TVShowFromTVDBDto();
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try {
                var response = await client.GetAsync(GetShowUri(seriesId));
                response.EnsureSuccessStatusCode();
                var stringResponse = await response.Content.ReadAsStringAsync();
                JsonConverter converter = new JsonConverter();
                showDto = converter.ConvertTVSeriesToDto(stringResponse);
                Log.Information($"Successfully fetched TV Series {seriesId}: {showDto.SeriesNameTVDB}");
            }
            catch (Exception e) {
                Log.Error($"{ e.Message }");
            }
            return showDto;
        }

        public Uri GetShowUri(int seriesId) {
            _uriBuilder.Path = $"series/{seriesId}/filter";
            _uriBuilder.Query = $"keys=seriesName,id";
            return _uriBuilder.Uri;
        }

        public async Task<EpisodeOuterDto> FetchEpisodes(int seriesId, string token, int page = 1) {
            Log.Information("Fetching Episodes from seriesId {a}", seriesId);
            EpisodeOuterDto episodesOuter = new EpisodeOuterDto();
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try {
                var response = await client.GetAsync(GetEpisodesUri(seriesId, page));
                response.EnsureSuccessStatusCode();
                var stringResponse = await response.Content.ReadAsStringAsync();
                JsonConverter converter = new JsonConverter();
                episodesOuter = converter.ConvertEpisodeOuterObjectToDto(stringResponse);
                Log.Information($"Successfully fetched episodes for TV Series {seriesId}");
            }
            catch (Exception e) {
                Log.Error($"{ e.Message }");
            }
            return episodesOuter;
        }

        public Uri GetEpisodesUri(int seriesId, int page = 1) {
            _uriBuilder.Path = $"series/{seriesId}/episodes";
            if (page != 1) {
                _uriBuilder.Query = $"page={page}";
            }
            return _uriBuilder.Uri;
        }

    }
}
