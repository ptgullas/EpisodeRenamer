using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Data.Entities {
    public class Episode {
        public int Id { get; set; }
        [JsonProperty("id")]
        public int TVDBEpisodeId { get; set; }
        public int Season { get; set; }
        public int AiredSeasonId { get; set; }
        public string EpisodeName { get; set; }
        public DateTime FirstAired { get; set; }
        public int SeriesId { get; set; }
    }
}
