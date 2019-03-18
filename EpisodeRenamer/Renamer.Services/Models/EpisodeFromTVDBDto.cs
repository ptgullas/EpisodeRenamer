using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Renamer.Services.Models {

    public class EpisodeOuterDto {
        [JsonProperty("data")]
        public EpisodeFromTVDBDto[] episodes;
        [JsonProperty]
        public EpisodeLinksDto links;
    }

    public class EpisodeLinksDto {
        [JsonProperty]
        public int First { get; set; }
        [JsonProperty]
        public int Last { get; set; }
        [JsonProperty]
        public int? Next { get; set; }
        [JsonProperty]
        public int? Prev { get; set; }
    }

    public class EpisodeFromTVDBDto {
        [JsonProperty("id")]
        public int EpisodeId { get; set; }
        public int SeriesId { get; set; }
        [JsonProperty("airedSeason")]
        public int? Season { get; set; }
        [JsonProperty("airedEpisodeNumber")]
        public int? NumberInSeason { get; set; }
        public string EpisodeName { get; set; }
        public DateTime DateRetrieved { get; set; }
        public long LastUpdated { get; set; }
        public int? AiredSeasonId { get; set; }
        public int? AbsoluteNumber { get; set; }

    }
}
