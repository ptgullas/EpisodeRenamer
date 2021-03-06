﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Renamer.Services.Models {

    public class TVShowOuterDto {
        [JsonProperty("data")]
        public TVShowFromTVDBDto tvSeries;
    }

    public class TVShowFromTVDBDto {
        [JsonProperty("id")]
        public int SeriesId { get; set; }
        [JsonProperty("seriesName")]
        public string SeriesNameTVDB { get; set; }
        public string SeriesNamePreferred { get; set; }

        public string Network { get; set; }
        public string FirstAired { get; set; }
        public string Status { get; set; }
    }
}
