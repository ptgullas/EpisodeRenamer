using Xunit;
using Renamer.Services;
using Renamer.Services.Models;
using Renamer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Renamer.Tests {
    public class TVShowServiceTests {
        [Fact]
        public void Add_ShowNotInDB_WritesToDB() {
            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseInMemoryDatabase(databaseName: "Add_ShowNotInDB_WritesToDB")
                .Options;
            TVShow show = new TVShow() {
                SeriesId = 281662,
                SeriesName = "Marvel's Daredevil",
                SeriesNamePreferred = "Daredevil"
            };
            using (var context = new EpisodeContext(options)) {
                var service = new TVShowService(context);
                service.Add(show);
            }
            using (var context = new EpisodeContext(options)) {
                Assert.Equal(1, context.Shows.Count());
                Assert.Equal("Daredevil", context.Shows.Single().SeriesNamePreferred);
            }
        }
    }
}
