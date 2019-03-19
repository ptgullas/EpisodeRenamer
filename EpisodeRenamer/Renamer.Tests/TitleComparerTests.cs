using System;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;

namespace Renamer.Tests {
    public class TitleComparerTests {
        [Fact]
        public void ContainsSeasonEpisode_DoesContain_ReturnTrue() {
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
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
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
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
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
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
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
                SeriesName = "Doom Patrol",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 2,
                EpisodeTitle = "Donkey Patrol"
            };
            TitleComparer comparer = new TitleComparer();
            string expected = "Doom Patrol - 1.02 - Donkey Patrol";

            Assert.Equal(expected, comparer.GetFormattedFilename(episodeObject));
        }

        [Fact]
        public void GetSeriesNameInLowercaseAndPeriodsWithoutStartingArticle_ReturnsTrue() {
            TitleComparer comparer = new TitleComparer();
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
                SeriesName = "The Chilling Adventures of Sabrina",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 4,
                EpisodeTitle = "Witch Academy"
            };
            string expected = "chilling.adventures.of.sabrina";

            string result = comparer.GetSeriesNameInLowercaseAndPeriodsWithoutStartingArticle(episodeObject);

            Assert.Equal(expected, result);
        }
        [Fact]
        public void FilenameContainsSeriesName_ContainsSeriesName_ReturnsTrue() {
            TitleComparer comparer = new TitleComparer();
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
                SeriesName = "The Chilling Adventures of Sabrina",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 4,
                EpisodeTitle = "Witch Academy"
            };
            string fileName = @"the.chilling.adventures.of.sabrina.s01e04.720p.webrip.hevc.x265.rmteam.mkv";

            Assert.True(comparer.FilenameContainsSeriesName(episodeObject, fileName));
        }
        [Fact]
        public void FilenameContainsSeriesName_DoesNotContainSeriesName_ReturnsFalse() {
            TitleComparer comparer = new TitleComparer();
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
                SeriesName = "The Chilling Adventures of Sabrina",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 4,
                EpisodeTitle = "Witch Academy"
            };
            string fileName = @"doctor.who.2005.s11e10.the.battle.of.ranskoor.av.kolos.720p.web.dl.hevc.x265.rmteam.mkv";

            Assert.False(comparer.FilenameContainsSeriesName(episodeObject, fileName));
        }

        [Fact]
        public void FilenameMatchesEpisode_ItMatches_ReturnsTrue() {
            TitleComparer comparer = new TitleComparer();
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
                SeriesName = "The Chilling Adventures of Sabrina",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 4,
                EpisodeTitle = "Witch Academy"
            };
            string fileName = @"the.chilling.adventures.of.sabrina.s01e04.720p.webrip.hevc.x265.rmteam.mkv";

            Assert.True(comparer.FilenameMatchesEpisode(episodeObject, fileName));
        }

        [Fact]
        public void FilenameMatchesEpisode_DoesNotMatch_ReturnsFalse() {
            TitleComparer comparer = new TitleComparer();
            EpisodeForComparingDto episodeObject = new EpisodeForComparingDto() {
                SeriesName = "The Chilling Adventures of Sabrina",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 4,
                EpisodeTitle = "Witch Academy"
            };
            string fileName = @"the.chilling.adventures.of.sabrina.s01e07.720p.webrip.hevc.x265.rmteam.mkv";

            Assert.False(comparer.FilenameMatchesEpisode(episodeObject, fileName));
        }

        [Fact]
        public void ExtractSeasonEpisodeFromFilename_ReturnSeasonEpisode() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"the.chilling.adventures.of.sabrina.s01e07.720p.webrip.hevc.x265.rmteam.mkv";

            string expected = "s01e07";
            Assert.Equal(expected, comparer.ExtractSeasonEpisodeFromFilename(fileName));

        }
        [Fact]
        public void ExtractSeriesNameFromFilename() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"the.chilling.adventures.of.sabrina.s01e07.720p.webrip.hevc.x265.rmteam.mkv";

            string expected = "the.chilling.adventures.of.sabrina";
            Assert.Equal(expected, comparer.ExtractSeriesNameFromFilename(fileName));

        }

        [Fact]
        public void ExtractSeasonNumberFromFilename_ReturnsSeason() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"the.chilling.adventures.of.sabrina.s01e07.720p.webrip.hevc.x265.rmteam.mkv";

            int expected = 1;
            Assert.Equal(expected, comparer.ExtractSeasonNumberFromFilename(fileName));

        }
        [Fact]
        public void ExtractSeasonEpisodeFromFilenameAsEpisodeObject_ContainsSeasonEpisode_ReturnsEpObject() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"the.chilling.adventures.of.sabrina.s01e07.720p.webrip.hevc.x265.rmteam.mkv";

            int expectedSeason = 1;
            int expectedEpisode = 7;
            Assert.Equal(expectedSeason, comparer.ExtractSeasonEpisodeFromFilenameAsEpisodeObject(fileName).SeasonNumber);
            Assert.Equal(expectedEpisode, comparer.ExtractSeasonEpisodeFromFilenameAsEpisodeObject(fileName).EpisodeNumberInSeason);

        }
        [Fact]
        public void ExtractSeasonEpisodeFromFilenameAsEpisodeObject_DoesNotContainSeasonEpisode_Throws() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"the.chilling.adventures.of.sabrina.mkv";
            string expectedMessage = $"Filename does not contain episode number in s##e## format.\r\nParameter name:{fileName}";

            // Act & Assert
            Exception ex = Assert.Throws<ArgumentException>(() => comparer.ExtractSeasonEpisodeFromFilenameAsEpisodeObject(fileName));

            Assert.Equal(expectedMessage.Substring(0,50), ex.Message.Substring(0,50));
        }
        [Fact]
        public void ExtractSeasonEpisodeFromFilenameAsEpisodeObject_ContainsEpisode00_ReturnsEpObject() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"doctor.who.2005.s12e00.resolution.720p.webrip.hevc.x265.rmteam.mkv";

            int expectedSeason = 12;
            int expectedEpisode = 0;
            Assert.Equal(expectedSeason, comparer.ExtractSeasonEpisodeFromFilenameAsEpisodeObject(fileName).SeasonNumber);
            Assert.Equal(expectedEpisode, comparer.ExtractSeasonEpisodeFromFilenameAsEpisodeObject(fileName).EpisodeNumberInSeason);

        }

        [Fact]
        public void FilenameContainsSeasonEpisodeFormat_ContainsSeasonEpisode_ReturnsTrue() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"the.chilling.adventures.of.sabrina.s01e07.720p.webrip.hevc.x265.rmteam.mkv";
            Assert.True(comparer.FilenameContainsSeasonEpisodeFormat(fileName));
        }

        [Fact]
        public void FilenameContainsSeasonEpisodeFormat_ContainsEpisodeWithZero_ReturnsTrue() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"doctor.who.2005.s12.e00.resolution.720p.webrip.hevc.x265.rmteam.mkv";
            Assert.True(comparer.FilenameContainsSeasonEpisodeFormat(fileName));
        }


        [Fact]
        public void CreateEpisodeObjectFromFilename_ValidFile_CreatesObject() {
            TitleComparer comparer = new TitleComparer();
            string fileName = @"the.chilling.adventures.of.sabrina.s01e07.720p.webrip.hevc.x265.rmteam.mkv";

            int expectedSeason = 1;
            int expectedEpisode = 7;
            string expectedTitle = "the chilling adventures of sabrina";

            EpisodeForComparingDto epDto = comparer.CreateEpisodeObjectFromFilename(fileName);
            Assert.Equal(expectedSeason, epDto.SeasonNumber);
            Assert.Equal(expectedEpisode, epDto.EpisodeNumberInSeason);
            Assert.Equal(expectedTitle, epDto.SeriesName);
        }

        [Fact]
        public void CreateEpisodeObjectFromPath_ValidFile_CreatesObject() {
            TitleComparer comparer = new TitleComparer();
            string path = @"c:\temp\mypath1\mypath2\the.chilling.adventures.of.sabrina.s01e07.720p.webrip.hevc.x265.rmteam.mkv";

            int expectedSeason = 1;
            int expectedEpisode = 7;
            string expectedTitle = "the chilling adventures of sabrina";

            EpisodeForComparingDto epDto = comparer.CreateEpisodeObjectFromPath(path);
            Assert.Equal(expectedSeason, epDto.SeasonNumber);
            Assert.Equal(expectedEpisode, epDto.EpisodeNumberInSeason);
            Assert.Equal(expectedTitle, epDto.SeriesName);
            Assert.Equal(path, epDto.FilePath);
        }

    }
}
