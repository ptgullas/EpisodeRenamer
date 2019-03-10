using System;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;

namespace Renamer.Tests {
    public class TitleComparerTests {
        [Fact]
        public void ContainsSeasonEpisode_DoesContain_ReturnTrue() {
            EpisodeForComparing episodeObject = new EpisodeForComparing() {
                SeriesName = "Counterpart",
                SeasonNumber = 2,
                EpisodeNumberInSeason = 9,
                EpisodeTitle = "You to You"
            };
            string fileName = @"counterpart.s02e09.you.to.you.720p.webrip.hevc.x265.rmteam.mkv";
            bool expected = true;

            TitleComparer comparer = new TitleComparer();
            bool result = comparer.FilenameContainsSeasonEpisode(episodeObject, fileName);

            Assert.Equal(expected, result);
        }
        [Fact]
        public void GetSeasonEpisodeInSEFormat_Valid_ReturnTrue() {
            EpisodeForComparing episodeObject = new EpisodeForComparing() {
                SeriesName = "The Orville",
                SeasonNumber = 2,
                EpisodeNumberInSeason = 8,
                EpisodeTitle = "Identity, Part 1"
            };
            TitleComparer comparer = new TitleComparer();
            string expected = "s02e08";

            Assert.Equal(expected, comparer.GetSeasonEpisodeInSEFormat(episodeObject));
        }

        [Fact]
        public void GetSeasonEpisodeInNumberFormat_Valid_ReturnTrue() {
            EpisodeForComparing episodeObject = new EpisodeForComparing() {
                SeriesName = "The Orville",
                SeasonNumber = 2,
                EpisodeNumberInSeason = 9,
                EpisodeTitle = "Identity, Part 2"
            };
            TitleComparer comparer = new TitleComparer();
            string expected = "2.09";

            Assert.Equal(expected, comparer.GetSeasonEpisodeInNumberFormat(episodeObject));
        }

        [Fact]
        public void GetFormattedFilename_Valid_ReturnTrue() {
            EpisodeForComparing episodeObject = new EpisodeForComparing() {
                SeriesName = "Doom Patrol",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 2,
                EpisodeTitle = "Donkey Patrol"
            };
            TitleComparer comparer = new TitleComparer();
            string expected = "Doom Patrol - 1.02 - Donkey Patrol";

            Assert.Equal(expected, comparer.GetFormattedFilename(episodeObject));
        }

    }
}
