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

        [Fact]
        public void ToAuthenticator_Valid_ReturnsAuthenticator() {
            string filePath = @"c:\temp\tvdbinfo.json";
            string expectedApiKey = "URV9WXPTFX8R9J6A";
            string expectedUserKey = "6U0AVI208RGC9EWE";
            string expectedUsername = "junimitsu7t6";
            // Act
            TVDBInfo tvdbInfo = TVDBInfo.ReadFromFile(filePath);
            TVDBAuthenticator authenticator = tvdbInfo.ToAuthenticator();
            Assert.Equal(expectedApiKey, authenticator.ApiKey);
            Assert.Equal(expectedUserKey, authenticator.UserKey);
            Assert.Equal(expectedUsername, authenticator.Username);
        }
    }
}
