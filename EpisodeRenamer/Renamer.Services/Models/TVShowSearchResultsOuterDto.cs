using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public class TVShowSearchResultsOuterDto {
        [JsonProperty("data")]
        public TVShowFromTVDBDto[] shows;
    }
}
