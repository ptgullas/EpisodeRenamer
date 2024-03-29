﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Data.Entities {
    public class TVShow {
        public int Id { get; set; }
        [JsonProperty("id")]
        public int SeriesId { get; set; }
        [JsonProperty]
        public string SeriesName { get; set; }
        public string SeriesNamePreferred { get; set; }
        public ICollection<Episode> Episodes { get; set; }
        public bool IsActive { get; set; }

    }
}
