using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Renamer.Services.Models {

    public class TVSeriesOuter {
        [JsonProperty("data")]
        public TVSeriesDto tvSeries;
    }

    public class TVSeriesDto {
        [JsonProperty("id")]
        public int SeriesId { get; set; }
        [JsonProperty("seriesName")]
        public string SeriesNameTVDB { get; set; }
        public string SeriesNamePreferred { get; set; }

    }
}
