using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public class UserData {
        [JsonProperty("data")]
        public Faves faves;
    }

    public class Faves {
        public List<string> favorites;
    }
}
