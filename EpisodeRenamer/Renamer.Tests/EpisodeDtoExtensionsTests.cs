using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;
using Renamer.Data.Entities;

namespace Renamer.Tests {
    public class EpisodeDtoExtensionsTests {
        [Fact]
        public void GetLastUpdated_ValidNum_ReturnsDateTime() {
            EpisodeFromTVDBDto epDto = new EpisodeFromTVDBDto() {
                EpisodeId = 6089488,
                SeriesId = 328487,
                Season = 1,
                NumberInSeason = 1,
                EpisodeName = "Old Wounds",
                LastUpdated = 1527457806,
                AiredSeasonId = 712015,
                AbsoluteNumber = 1,
            };
            DateTime expectedLastUpdated = new DateTime(2018,5,27,17,50,6);
            Assert.Equal(expectedLastUpdated, epDto.GetLastUpdated());
        }
        [Fact]
        public void ToEpisode_ValidEp_ReturnsEpisode() {
            EpisodeFromTVDBDto epDto = new EpisodeFromTVDBDto() {
                EpisodeId = 6089488,
                SeriesId = 328487,
                Season = 1,
                NumberInSeason = 1,
                EpisodeName = "Old Wounds",
                LastUpdated = 1527457806,
                AiredSeasonId = 712015,
                AbsoluteNumber = 1,
            };
            int expectedId = 6089488;
            DateTime expectedLastUpdated = new DateTime(2018, 5, 27, 17, 50, 6);

            Episode ep = epDto.ToEpisode();

            Assert.Equal(expectedId, ep.TVDBEpisodeId);
            Assert.Equal(expectedLastUpdated, ep.LastUpdated);
        }
    }
}
