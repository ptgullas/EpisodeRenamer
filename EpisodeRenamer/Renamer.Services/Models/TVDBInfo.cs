using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Renamer.Services.Models {
    public class TVDBInfo {
        [JsonProperty("apiKey")]
        public string ApiKey { get; private set; }
        [JsonProperty("userKey")]
        public string UserKey { get; private set; }
        [JsonProperty("userName")]
        public string Username { get; private set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("tokenRetrieved")]
        public DateTime TokenRetrieved { get; set; }

        public bool TokenIsExpired {
            get {
                return (HoursSinceLastRefresh >= 24);
            }
        }

        public bool TokenIsAlmostExpired {
            get {
                return (HoursSinceLastRefresh >= 23) && (HoursSinceLastRefresh < 24);
            }
        }

        public TVDBInfo() {

        }

        // use it like this:
        // TVDBInfo tvdbInfo = TVDBInfo.ReadFromFile(filePath)
        public static TVDBInfo ReadFromFile(string filePath) {
            if (File.Exists(filePath)) {
                string jsonContents = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<TVDBInfo>(jsonContents);
            }
            else {
                throw new FileNotFoundException("Can't find TVDBInfo file", filePath);
            }
        }

        public void SaveToFile(string filePath) {
            string jsonContents = Serialize();
            File.WriteAllText(filePath, jsonContents);
        }

        public TVDBAuthenticator ToAuthenticator() {
            return JsonConvert.DeserializeObject<TVDBAuthenticator>(Serialize());
        }

        private string Serialize() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        private int HoursSinceLastRefresh {
            get {
                DateTime currentTime = DateTime.Now;
                TimeSpan ts = currentTime - TokenRetrieved;
                return ts.Hours;
            }
        }

    }
}
