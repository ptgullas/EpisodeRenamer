using Renamer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public static class EpisodeFromTVDBDtoExtensions {
        public static Episode ToEpisode(this EpisodeFromTVDBDto ep) {
            return new Episode() {
                TVDBEpisodeId = ep.EpisodeId,
                SeriesId = ep.SeriesId,
                Season = ep.Season,
                AiredEpisodeNumber = ep.NumberInSeason,
                EpisodeName = ep.EpisodeName,
                LastUpdated = ep.GetLastUpdated(),
                AiredSeasonId = ep.AiredSeasonId,
                AbsoluteNumber = ep.AbsoluteNumber
            };
        }

        public static DateTime GetLastUpdated(this EpisodeFromTVDBDto ep) {
            DateTime baseDateForEpoch = new DateTime(1970, 1, 1);
            DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(ep.LastUpdated).DateTime.ToLocalTime();
            return offset.LocalDateTime;
        }
    }
}
