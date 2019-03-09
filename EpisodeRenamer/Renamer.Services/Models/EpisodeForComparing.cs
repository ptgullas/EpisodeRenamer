using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public class EpisodeForComparing {
        public string SeriesName { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumberInSeason { get; set; }
        public string EpisodeTitle { get; set; }

        public string GetSeasonEpisodeInSEFormat() {
            return $"s{SeasonNumber.ToString("D2")}e{EpisodeNumberInSeason.ToString("D2")}";
        }

        public string GetSeasonEpisodeInNumberFormat() {
            return $"{SeasonNumber}.{EpisodeNumberInSeason.ToString("D2")}";
        }


        public string GetFullName() {
            return ($"{SeriesName} - {GetSeasonEpisodeInSEFormat()} - {EpisodeTitle}");
        }
    }
}
