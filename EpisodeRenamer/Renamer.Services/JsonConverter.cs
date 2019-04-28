using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Renamer.Services.Models;
using Renamer.Data.Entities;
using System.Linq;

namespace Renamer.Services {



    public class JsonConverter {
        public JsonConverter() {

        }

        public List<int> ConvertFavoritesToDto(string favoritesJson) {
            var myData = JsonConvert.DeserializeObject<UserDataDto>(favoritesJson);
            var integerFavorites = myData.faves.favorites.Select(s => s.ToInt());
            return integerFavorites.ToList();
        }

        public TVShowFromTVDBDto ConvertTVSeriesToDto(string seriesJson) {
            TVShowOuterDto outerData = JsonConvert.DeserializeObject<TVShowOuterDto>(seriesJson);
            return outerData.tvSeries;
        }

        public TVShowSearchResultsOuterDto ConvertTVShowSearchResultsToDto(string showsJson) {
            TVShowSearchResultsOuterDto outerData = JsonConvert.DeserializeObject<TVShowSearchResultsOuterDto>(showsJson);
            return outerData;

        }

        public EpisodeOuterDto ConvertEpisodeOuterObjectToDto(string episodesJson) {
            return JsonConvert.DeserializeObject<EpisodeOuterDto>(episodesJson);
        }

        public EpisodeFromTVDBDto[] ConvertEpisodesToDto(string episodesJson) {
            EpisodeOuterDto outerData = JsonConvert.DeserializeObject<EpisodeOuterDto>(episodesJson);
            return outerData.episodes;
        }

        public EpisodeLinksDto ConvertEpisodeLinksToDto(string episodesJson) {
            EpisodeOuterDto outerData = JsonConvert.DeserializeObject<EpisodeOuterDto>(episodesJson);
            return outerData.links;
        }

        /*
        public List<int> ConvertFavorites(string favoritesJson) {
            UserData outerData = JsonConvert.DeserializeObject<UserData>(favoritesJson);
            return outerData.userFavorites;
        }

        public Episode[] ConvertEpisodes(string episodesJson) {

        }
        */
    }
}
