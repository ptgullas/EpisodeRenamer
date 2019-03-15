using System;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Tests {
    public class LevenshteinDistanceTests {
        [Fact]
        public void Compute_Passes() {
            string showFromDB = "The Chilling Adventures of Sabrina"
                .ToUpper();
            string showFromFile = "the.chilling.adventures.of.sabrina"
                .ToUpper()
                .ReplacePeriodsWithSpaces();
            int expectedDistance = 0;
            int result = LevenshteinDistance.Compute(showFromDB, showFromFile);

            Assert.Equal(expectedDistance, result);
        }
    }
}
