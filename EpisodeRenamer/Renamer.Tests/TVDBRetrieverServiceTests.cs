using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;


namespace Renamer.Tests {
    public class TVDBRetrieverServiceTests {
        [Fact]
        public void GetLoginUri_Passes() {
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            TVDBRetrieverService service = new TVDBRetrieverService(httpClientFactory);
            string expectedUri = "https://api.thetvdb.com/login";

            // Act
            var uri = service.GetLoginUri();
            Assert.Equal(expectedUri, uri.AbsoluteUri);
            
        }
    }
}
