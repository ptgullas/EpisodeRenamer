using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Renamer.Services.Models {

    public class TVShowOuterDto {
        [JsonProperty("data")]
        public TVShowDto tvSeries;
    }

    public class TVShowDto {
        [JsonProperty("id")]
        public int SeriesId { get; set; }
        [JsonProperty("seriesName")]
        public string SeriesNameTVDB { get; set; }
        public string SeriesNamePreferred { get; set; }

    }
}
