using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Renamer.Services.Models;
using Renamer.Data.Entities;

namespace Renamer.Services {



    public class JsonConverter {
        public JsonConverter() {

        }


        public List<string> ConvertFavoritesToDto(string favoritesJson) {
            var myData = JsonConvert.DeserializeObject<UserDataDto>(favoritesJson);
            return myData.faves.favorites;
        }

        public TVSeriesDto ConvertTVSeriesToDto(string seriesJson) {
            TVSeriesOuterDto outerData = JsonConvert.DeserializeObject<TVSeriesOuterDto>(seriesJson);
            return outerData.tvSeries;
        }

        public EpisodeDto[] ConvertEpisodesToDto(string episodesJson) {
            EpisodeOuterDto outerData = JsonConvert.DeserializeObject<EpisodeOuterDto>(episodesJson);
            return outerData.episodes;
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
