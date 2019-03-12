using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Renamer.Services.Models;

namespace Renamer.Services {



    public class JsonConverter {
        public JsonConverter() {

        }


        public List<string> ConvertFavoritesToDto(string favoritesJson) {
            var myData = JsonConvert.DeserializeObject<UserDataDto>(favoritesJson);
            return myData.faves.favorites;
        }

        public TVSeriesDto ConvertTVSeriesToDto(string seriesJson) {
            TVSeriesOuter outerData = JsonConvert.DeserializeObject<TVSeriesOuter>(seriesJson);
            return outerData.tvSeries;
        }

        public EpisodeDto[] ConvertEpisodesToDto(string episodesJson) {
            EpisodeOuter outerData = JsonConvert.DeserializeObject<EpisodeOuter>(episodesJson);
            return outerData.episodes;
        }
    }
}
