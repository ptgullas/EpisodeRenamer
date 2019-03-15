using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Renamer.Services.Models {

    public class EpisodeOuterDto {
        [JsonProperty("data")]
        public EpisodeDto[] episodes;
    }

    public class EpisodeDto {
        public int SeriesId { get; set; }
        [JsonProperty("airedSeason")]
        public int Season { get; set; }
        [JsonProperty("airedEpisodeNumber")]
        public int NumberInSeason { get; set; }
        public string EpisodeName { get; set; }
        public bool IsInList { get; set; }
        [JsonProperty("firstAired")]
        public DateTime DateAired { get; set; }
        public DateTime DateRetrieved { get; set; }

    }
}
