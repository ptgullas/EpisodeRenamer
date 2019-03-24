using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public static class EpisodeForComparingDtoExtensions {
        public static string GetFormattedFilename(this EpisodeForComparingDto ep) {
            return ($"{ep.SeriesName} - {ep.GetSeasonEpisodeInNumberFormat()} - {ep.EpisodeTitle}").TrimEnd(' ');
        }
        public static string GetSeasonEpisodeInNumberFormat(this EpisodeForComparingDto ep) {
            return $"{ep.SeasonNumber}.{ep.EpisodeNumberInSeason.ToString("D2")}";
        }

    }
}
