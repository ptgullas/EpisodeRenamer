using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;
using Renamer.Data.Entities;

namespace Renamer.Tests {
    public class TVShowServiceTests {
        [Fact]
        public void ConvertToTVShow_ValidShowNoPreferred_Passes() {
            TVShowDto newShow = new TVShowDto() {
                SeriesId = 356640,
                SeriesNameTVDB = "Russian Doll"
            };

            int expectedSeriesId = 356640;
            string expectedSeriesName = "Russian Doll";

            TVShowService service = new TVShowService();

            var result = service.ConvertToTVShow(newShow);

            Assert.Equal(expectedSeriesId, result.SeriesId);
            Assert.Equal(expectedSeriesName, result.SeriesName);
            Assert.Null(result.SeriesNamePreferred);
        }

        [Fact]
        public void ConvertToTVShow_ValidShowWithPreferred_Passes() {
            TVShowDto newShow = new TVShowDto() {
                SeriesId = 281662,
                SeriesNameTVDB = "Marvel's Daredevil",
                SeriesNamePreferred = "Daredevil"
               
            };

            int expectedSeriesId = 281662;
            string expectedSeriesName = "Marvel's Daredevil";
            string expectedPreferredName = "Daredevil";

            TVShowService service = new TVShowService();

            var result = service.ConvertToTVShow(newShow);

            Assert.Equal(expectedSeriesId, result.SeriesId);
            Assert.Equal(expectedSeriesName, result.SeriesName);
            Assert.Equal(expectedPreferredName, result.SeriesNamePreferred);
        }
    }
}
