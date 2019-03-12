using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public class UserDataDto {
        [JsonProperty("data")]
        public FavesDto faves;
    }

    public class FavesDto {
        public List<string> favorites;
    }
}
