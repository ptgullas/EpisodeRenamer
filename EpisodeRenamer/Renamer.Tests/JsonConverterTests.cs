using Xunit;
using Renamer.Services;
using Renamer.Services.Models;
using System.IO;
using System;

namespace Renamer.Tests {
    public class JsonConverterTests {
        [Theory]
        [InlineData(78804)]
        public void ConvertFavorites_ReadFirstFavorite_ReturnTrue(int val) {
            string pathToJson = @"..\..\..\..\Renamer.Data\SampleData\UserFavorites.json";
            string json = File.ReadAllText(pathToJson);

            JsonConverter converter = new JsonConverter();


            Assert.Equal(val, converter.ConvertFavoritesToDto(json)[0]);
        }
        [Theory]
        [InlineData("Doom Patrol")]
        public void ConvertTVSeries_ReadSeriesName_ReturnTrue(string val) {
            string pathToJson = @"..\..\..\..\Renamer.Data\SampleData\DoomPatrolSeriesInfo.json";
            string json = File.ReadAllText(pathToJson);

            JsonConverter converter = new JsonConverter();


            Assert.Equal(val, converter.ConvertTVSeriesToDto(json).SeriesNameTVDB);
        }

        [Fact]
        public void ConvertEpisodes_ReadEpisodeName_ReturnTrue() {
            string pathToJson = @"..\..\..\..\Renamer.Data\SampleData\OrvilleEpisodes.json";
            string json = File.ReadAllText(pathToJson);

            JsonConverter converter = new JsonConverter();

            string expectedName = "Old Wounds";
            int expectedSeason = 1;
            int expectedEpisodeNum = 1;
            DateTime expectedAirDate = new DateTime(2017, 9, 10);

            // Act
            EpisodeFromTVDBDto[] eps = converter.ConvertEpisodesToDto(json);
            EpisodeFromTVDBDto ep = eps[0];
            // Assert
            Assert.Equal(expectedName, ep.EpisodeName);
            Assert.Equal(expectedSeason, ep.Season);
            Assert.Equal(expectedEpisodeNum, ep.NumberInSeason);
            Assert.Equal(expectedAirDate, ep.FirstAired);
        }

        [Fact]
        public void ConvertTVShowSearchResultsToDto_Valid_ReturnsOuterObject() {
            string pathToJson = @"..\..\..\..\Renamer.Data\SampleData\SiliconValleySearchResults.json";
            string json = File.ReadAllText(pathToJson);
            JsonConverter converter = new JsonConverter();

            string expectedShowName = "Silicon Valley";
            long expectedSeriesId = 277165;

            TVShowSearchResultsOuterDto outerObject = converter.ConvertTVShowSearchResultsToDto(json);

            Assert.Equal(expectedShowName, outerObject.shows[0].SeriesNameTVDB);
            Assert.Equal(expectedSeriesId, outerObject.shows[0].SeriesId);
        }

        [Fact]
        public void ConvertEpisodeOuterObjectToDto_Valid_ReturnsOuterObject() {
            string pathToJson = @"..\..\..\..\Renamer.Data\SampleData\OrvilleEpisodes.json";
            string json = File.ReadAllText(pathToJson);

            JsonConverter converter = new JsonConverter();
            int expectedLastPage = 1;
            string expectedName = "Old Wounds";
            int expectedSeason = 1;
            int expectedEpisodeNum = 1;

            EpisodeOuterDto episodeOuter = converter.ConvertEpisodeOuterObjectToDto(json);

            Assert.Equal(expectedLastPage, episodeOuter.links.Last);
            Assert.Equal(expectedName, episodeOuter.episodes[0].EpisodeName);
            Assert.Equal(expectedSeason, episodeOuter.episodes[0].Season);
            Assert.Equal(expectedEpisodeNum, episodeOuter.episodes[0].NumberInSeason);
        }

    }
}
