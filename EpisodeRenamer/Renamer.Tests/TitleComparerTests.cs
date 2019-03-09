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
            bool result = comparer.ContainsSeasonEpisode(episodeObject, fileName);

            Assert.Equal(expected, result);
            

        }
    }
}
