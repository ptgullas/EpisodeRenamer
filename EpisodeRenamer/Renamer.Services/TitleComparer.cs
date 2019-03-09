using Renamer.Services.Models;
using System;

namespace Renamer.Services {
    public class TitleComparer {
        public TitleComparer() {

        }

        public bool ContainsSeasonEpisode(EpisodeForComparing ep, string fileName) {
            string seasonEpisode = ep.GetSeasonEpisodeInSEFormat();
            return fileName.Contains(seasonEpisode);
        }
    }
}
