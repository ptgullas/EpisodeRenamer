﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Data.Entities {
    public class Episode {
        public int Id { get; set; }
        [JsonProperty("id")]
        public int TVDBEpisodeId { get; set; }
        public int? Season { get; set; }
        public int? AiredSeasonId { get; set; }
        public int? AiredEpisodeNumber { get; set; }
        public string EpisodeName { get; set; }
        public int? AbsoluteNumber { get; set; }
        public DateTime LastUpdated { get; set; }

        public int SeriesId { get; set; }
        public TVShow TVShow { get; set; }
    }
}
