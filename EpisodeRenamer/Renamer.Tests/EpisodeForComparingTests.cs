using System;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;

namespace Renamer.Tests {
    public class EpisodeForComparingTests {
        [Fact]
        public void GetSeasonEpisodeInSEFormat_Valid_ReturnTrue() {
            EpisodeForComparing episodeObject = new EpisodeForComparing() {
                SeriesName = "The Orville",
                SeasonNumber = 2,
                EpisodeNumberInSeason = 8,
                EpisodeTitle = "Identity, Part 1"
            };

            string expected = "s02e08";

            Assert.Equal(expected, episodeObject.GetSeasonEpisodeInSEFormat());
        }

        [Fact]
        public void GetSeasonEpisodeInNumberFormat_Valid_ReturnTrue() {
            EpisodeForComparing episodeObject = new EpisodeForComparing() {
                SeriesName = "The Orville",
                SeasonNumber = 2,
                EpisodeNumberInSeason = 9,
                EpisodeTitle = "Identity, Part 2"
            };

            string expected = "2.09";

            Assert.Equal(expected, episodeObject.GetSeasonEpisodeInNumberFormat());
        }

    }
}
