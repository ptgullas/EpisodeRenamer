using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public class TVDBAuthenticator {
        [JsonProperty("apiKey")]
        public string ApiKey { get; private set; }
        [JsonProperty("userKey")]
        public string UserKey { get; private set; }
        [JsonProperty("userName")]
        public string Username { get; private set; }

    }
}
