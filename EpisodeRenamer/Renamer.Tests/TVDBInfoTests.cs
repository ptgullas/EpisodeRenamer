using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;
using System.IO;

namespace Renamer.Tests {
    public class TVDBInfoTests {
        [Fact]
        public void ReadFromFile_ValidFile_Passes() {
            string filePath = @"c:\temp\tvdbinfo.json";
            string expectedApiKey = "URV9WXPTFX8R9J6A";
            // Act
            TVDBInfo tvdbInfo = TVDBInfo.ReadFromFile(filePath);
            // Assert
            Assert.Equal(expectedApiKey, tvdbInfo.ApiKey);
        }

        [Fact]
        public void ReadFromFile_InvalidFile_ThrowsException() {
            string filePath = @"c:\tempobox\tvdbinfo.json";

            // Assert
            Assert.Throws<FileNotFoundException>(() => TVDBInfo.ReadFromFile(filePath));
        }

    }
}
