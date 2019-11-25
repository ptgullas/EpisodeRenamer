using Renamer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public static class EpisodeFromTVDBDtoExtensions {
        public static Episode ToEpisode(this EpisodeFromTVDBDto epDto) {
            return new Episode() {
                TVDBEpisodeId = epDto.EpisodeId,
                SeriesId = epDto.SeriesId,
                Season = epDto.Season,
                AiredEpisodeNumber = epDto.NumberInSeason,
                EpisodeName = epDto.EpisodeName,
                LastUpdated = epDto.GetLastUpdated(),
                AiredSeasonId = epDto.AiredSeasonId,
                AbsoluteNumber = epDto.AbsoluteNumber
            };
        }

        public static DateTime GetLastUpdated(this EpisodeFromTVDBDto epDto) {
            DateTime baseDateForEpoch = new DateTime(1970, 1, 1);
            DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(epDto.LastUpdated).DateTime.ToLocalTime();
            return offset.LocalDateTime;
        }

        public static bool FirstAiredInPastYear(this EpisodeFromTVDBDto epDto) {
            if ((epDto.FirstAired != null) && (!epDto.AiredOverOneYearAgo())) {
                return true;
            }
            else {
                return false;
            }
        }

        public static bool AiredOverOneYearAgo(this EpisodeFromTVDBDto epDto) {
            if (epDto.FirstAired != null) {
                if (epDto.FirstAired > DateTime.Now) {
                    return false;
                }
                else if ((DateTime.Now - epDto.FirstAired)?.Days > 365) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return true; // return true if FirstAired is null
            }
        }
    }
}
