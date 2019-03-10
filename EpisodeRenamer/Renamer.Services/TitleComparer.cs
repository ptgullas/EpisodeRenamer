using Renamer.Services.Models;
using System;

namespace Renamer.Services {
    public class TitleComparer {
        public TitleComparer() {

        }

        public bool FilenameContainsSeasonEpisode(EpisodeForComparing ep, string fileName) {
            string seasonEpisode = GetSeasonEpisodeInSEFormat(ep);
            return fileName.Contains(seasonEpisode);
        }

        public string GetSeasonEpisodeInSEFormat(EpisodeForComparing ep) {
            return $"s{ep.SeasonNumber.ToString("D2")}e{ep.EpisodeNumberInSeason.ToString("D2")}";
        }
        public string GetSeasonEpisodeInNumberFormat(EpisodeForComparing ep) {
            return $"{ep.SeasonNumber}.{ep.EpisodeNumberInSeason.ToString("D2")}";
        }
        public string GetFormattedFilename(EpisodeForComparing ep) {
            return ($"{ep.SeriesName} - {GetSeasonEpisodeInNumberFormat(ep)} - {ep.EpisodeTitle}");
        }
    }
}
