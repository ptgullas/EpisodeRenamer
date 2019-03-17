using Xunit;
using Xunit.Abstractions;
using Renamer.Services;
using Renamer.Services.Models;
using Renamer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Renamer.Tests {
    public class EpisodeServiceTests {

        [Fact]
        public void Add_EpNotInDB_WritesToDb() {
            Episode ep1 = new Episode() {
                SeriesId = 328487,
                TVDBEpisodeId = 6089488,
                Season = 1,
                AiredEpisodeNumber = 1,
                EpisodeName = "Old Wounds"
            };

            int expectedEpisodeId = 6089488;
            string expectedEpisodeName = "Old Wounds";
            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "Add_EpNotInDB_WritesToDb")
                .Options;

            // Act
            using (var context = new EpisodeContext(options)) {
                var service = new EpisodeService(context);
                service.Add(ep1);
            }

            using (var context = new EpisodeContext(options)) {
                var service = new EpisodeService(context);
                var result = context.Episodes.FirstOrDefault(e => e.TVDBEpisodeId == expectedEpisodeId);
                Assert.NotNull(result);
                Assert.Equal(expectedEpisodeName, result.EpisodeName);
            }


        }

        [Fact]
        public void FindByEpisodeForComparingDto_EpInDB_ReturnsEp() {
            TVShow show = new TVShow() {
                SeriesId = 328487,
                SeriesName = "The Orville"
            };
            Episode ep1 = new Episode() {
                SeriesId = 328487,
                TVDBEpisodeId = 6089488,
                Season = 1,
                AiredEpisodeNumber = 1,
                EpisodeName = "Old Wounds"
            };
            Episode ep2 = new Episode() {
                SeriesId = 328487,
                TVDBEpisodeId = 6123044,
                Season = 1,
                AiredEpisodeNumber = 2,
                EpisodeName = "Command Performance"
            };

            EpisodeForComparingDto epDto = new EpisodeForComparingDto() {
                SeriesName = "the orville",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 1
            };

            string expectedEpisodeName = "Old Wounds";

            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "FindByEpisodeForComparingDto_EpInDB_ReturnsEp")
                .Options;


            using (var context = new EpisodeContext(options)) {
                var service = new EpisodeService(context);
                var TVService = new TVShowService(context);
                TVService.Add(show);
                service.Add(ep1);
                service.Add(ep2);
            }

            using (var context = new EpisodeContext(options)) {
                var service = new EpisodeService(context);
                Episode result = service.FindByEpisodeForComparingDto(epDto);
                Assert.NotNull(result);
                Assert.Equal(expectedEpisodeName, result.EpisodeName);
            }


        }

        [Fact]
        public void FindByEpisodeForComparingDto_EpNotInDB_ReturnsNull() {
            TVShow show = new TVShow() {
                SeriesId = 328487,
                SeriesName = "The Orville"
            };
            Episode ep1 = new Episode() {
                SeriesId = 328487,
                TVDBEpisodeId = 6089488,
                Season = 1,
                AiredEpisodeNumber = 1,
                EpisodeName = "Old Wounds"
            };

            EpisodeForComparingDto epDto = new EpisodeForComparingDto() {
                SeriesName = "the chilling adventures of sabrina",
                SeasonNumber = 1,
                EpisodeNumberInSeason = 1
            };

            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "FindByEpisodeForComparingDto_EpNotInDB_ReturnsNull")
                .Options;

            using (var context = new EpisodeContext(options)) {
                var service = new EpisodeService(context);
                var TVService = new TVShowService(context);
                TVService.Add(show);
                service.Add(ep1);
            }

            using (var context = new EpisodeContext(options)) {
                var service = new EpisodeService(context);
                Episode result = service.FindByEpisodeForComparingDto(epDto);
                Assert.Null(result);
            }

        }

        [Fact]
        public void AddEpisodeToShow_ShowIsInDB_AddsEp() {
            TVShow show = new TVShow() {
                SeriesId = 328487,
                SeriesName = "The Orville"
            };

            Episode ep1 = new Episode() {
                SeriesId = 328487,
                TVDBEpisodeId = 6089488,
                Season = 1,
                AiredEpisodeNumber = 1,
                EpisodeName = "Old Wounds",
                LastUpdated = DateTimeOffset.FromUnixTimeMilliseconds(1527457806).DateTime.ToLocalTime()
            };

            int expectedEpisodeId = 6089488;
            string expectedEpisodeName = "Old Wounds";
            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "AddEpisodeToShow_ShowIsInDB_AddsEp")
                .Options;

            // Act
            using (var context = new EpisodeContext(options)) {
                var service = new EpisodeService(context);
                var TVService = new TVShowService(context);
                TVService.Add(show);

                service.AddEpisodeToShow(ep1);
            }

            using (var context = new EpisodeContext(options)) {
                var service = new EpisodeService(context);
                var result = context.Episodes.FirstOrDefault(e => e.TVDBEpisodeId == expectedEpisodeId);
                
                Assert.NotNull(result);
                Assert.Equal(expectedEpisodeName, result.EpisodeName);

            }
        }
     }
}
