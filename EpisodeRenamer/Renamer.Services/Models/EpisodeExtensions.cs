using Renamer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public static class EpisodeExtensions {
        public static EpisodeForComparingDto ToEpisodeForComparingDto(this Episode ep, string seriesName) {
            return new EpisodeForComparingDto() {
                SeriesName = seriesName.ReplaceInvalidChars(),
                SeasonNumber = (int) ep.Season,
                EpisodeNumberInSeason = (int) ep.AiredEpisodeNumber,
                EpisodeTitle = ep.EpisodeName.ReplaceInvalidChars()
            };
        }
    }
}
