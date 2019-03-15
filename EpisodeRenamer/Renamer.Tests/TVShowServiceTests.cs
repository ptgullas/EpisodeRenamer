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
    public class TVShowServiceTests {
        private readonly ITestOutputHelper output;

        // Used for output.WriteLine (click on "Output" link in Test Explorer test results)
        public TVShowServiceTests(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public void Add_ShowNotInDB_WritesToDB() {
            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "Add_ShowNotInDB_WritesToDB")
                .Options;
            TVShow show = new TVShow() {
                SeriesId = 281662,
                SeriesName = "The Orville",
                SeriesNamePreferred = "Orville"
            };
            using (var context = new EpisodeContext(options)) {
                var service = new TVShowService(context);
                service.Add(show);
            }
            using (var context = new EpisodeContext(options)) {
                Assert.Equal(1, context.Shows.Count());
                Assert.Equal("Orville", context.Shows.Single().SeriesNamePreferred);
            }
        }

        [Fact]
        public void Add_ShowInDB_DoesNotWriteToDB() {
            var options2 = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "Add_ShowInDB_DoesNotWriteToDB")
                .Options;
            TVShow show = new TVShow() {
                SeriesId = 281662,
                SeriesName = "Marvel's Daredevil",
                SeriesNamePreferred = "Daredevil"
            };
            TVShow sameShow = new TVShow() {
                SeriesId = 281662,
                SeriesName = "Marvel's Daredevil",
                SeriesNamePreferred = "Beastie"
            };
            using (var context = new EpisodeContext(options2)) {
                var service = new TVShowService(context);
                service.Add(show);
                service.Add(sameShow);
            }
            using (var context = new EpisodeContext(options2)) {
                // Assert.Equal("Not real", context.Shows.First().SeriesNamePreferred);
                var shows = context.Shows.Where(b => b.Id >= 0);
                foreach (var s in shows) {
                    output.WriteLine($"{s.Id}: SeriesId: {s.SeriesId}; Name: {s.SeriesName}");
                }
                Assert.Equal(1, context.Shows.Count());
                Assert.Equal("Daredevil", context.Shows.Single().SeriesNamePreferred);
            }
        }
        [Fact]
        public void FindByName_ShowInDB_ReturnsShow() {
            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "FindByName_ShowInDB_ReturnsShow")
                .Options;
            TVShow show1 = new TVShow() {
                SeriesName = "Marvel's Daredevil",
                SeriesNamePreferred = "Daredevil"
            };
            TVShow show2 = new TVShow() {
                SeriesId = 328487,
                SeriesName = "The Orville"
            };
            using (var context = new EpisodeContext(options)) {
                var service = new TVShowService(context);
                context.Database.EnsureCreated();
                service.Add(show1);
                service.Add(show2);
            }
            using (var context = new EpisodeContext(options)) {
                var service = new TVShowService(context);
                TVShow result = service.FindByName("marvels daredevil");
                Assert.Equal(2, context.Shows.Count());
                Assert.Equal("Marvel's Daredevil", result.SeriesName);
            }
        }

        [Fact]
        public void FindByName_ShowNotInDB_ReturnsNull() {
            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "FindByName_ShowNotInDB_ReturnsNull")
                .Options;
            TVShow show1 = new TVShow() {
                SeriesName = "Marvel's Daredevil",
                SeriesNamePreferred = "Daredevil"
            };
            using (var context = new EpisodeContext(options)) {
                var service = new TVShowService(context);
                service.Add(show1);
            }
            using (var context = new EpisodeContext(options)) {
                var service = new TVShowService(context);
                TVShow result = service.FindByName("Counterpart");
                Assert.Null(result);
            }
        }
    }
}
