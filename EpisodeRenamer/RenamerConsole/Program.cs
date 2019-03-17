using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Renamer.Services;
using Renamer.Services.Models;
using Renamer.Data;
using Renamer.Data.Entities;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RenamerConsole {
    class Program {

        public static IConfigurationRoot Configuration;

        static void Main(string[] args) {
            string projectRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf(@"\bin"));
            // PrintDirectories(appRoot);

            var builder = new ConfigurationBuilder()
                .SetBasePath(projectRoot)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            // IConfigurationRoot configuration = builder.Build();
            Configuration = builder.Build();

            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseSqlite(Configuration.GetConnectionString("connectionString"))
                .Options;
            var context = new EpisodeContext(options);


            DisplayMenuAndProcessUserInput(httpClientFactory);
            // SetUpAutomapper();

            // GetNewToken(httpClientFactory);

            // GetApiKey();
            //ReadTVDBInfo();
            //FindSampleShow(context);
            PauseForInput();
            // Console.WriteLine("Adding show");
            //AddSampleShow(context);
        }

        static public async void DisplayMenuAndProcessUserInput(IHttpClientFactory httpClientFactory) {
            TVDBInfo tvdbInfo = GetTVDBInfo();
            int userInput = 0;
            do {
                userInput = DisplayMenu(tvdbInfo);
                await ProcessUserInput(userInput, tvdbInfo, httpClientFactory);
            } while (userInput != 5);
        }

        static public int DisplayMenu(TVDBInfo tvdbInfo) {
            bool tokenIsValid = !tvdbInfo.TokenIsExpired; 
            Console.WriteLine("Episode Renamer!");
            Console.WriteLine();
            Console.Write("1. Get or refresh token:  ");
            DisplayTokenStatus(tokenIsValid);

            Console.WriteLine("2. Fetch User Favorites");
            Console.WriteLine("5. Exit if you dare");
            var result = Console.ReadLine();
            if (result.IsNumeric()) {
                return result.ToInt();
            }
            else {
                return 0;
            }
        }

        static async Task ProcessUserInput(int selection, TVDBInfo tvdbInfo, IHttpClientFactory httpClientFactory) {
            if (selection == 2) {
                await GetUserFavorites(tvdbInfo, httpClientFactory);
            }
        }

        static public bool IsTokenValid() {
            string filePath = GetTVDBInfoFilePath();
            TVDBInfo tvdbInfo = TVDBInfo.ReadFromFile(filePath);
            if (tvdbInfo.TokenIsExpired) {
                return false;
            }
            else {
                return true;
            }

        }

        static public void DisplayTokenStatus(bool IsValid) {
            Console.BackgroundColor = ConsoleColor.Black;
            if (IsValid) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("(Token is Valid!)");
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("(Token Expired!)");
            }
                Console.ResetColor();
        }

        static async Task GetUserFavorites(TVDBInfo tvdbInfo, IHttpClientFactory httpClientFactory) {
            Console.WriteLine("Getting user favorites...");
            if (!tvdbInfo.TokenIsExpired) {
                TVDBRetrieverService service = new TVDBRetrieverService(httpClientFactory);
                List<int> faves = await service.FetchUserFavorites(tvdbInfo.Token);
                faves.Sort();
                faves
                    .ForEach(c => Console.WriteLine(c));
            }
        }

        static void PauseForInput() {
            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();

        }

        static void SetUpAutomapper() {
            Mapper.Initialize(config => {
                config.CreateMap<EpisodeFromTVDBDto, Episode>();
                config.CreateMap<TVShowFromTVDBDto, TVShow>();
            });
        }

        static void GetApiKey() {
            var tvdbInfo = Configuration.GetSection("TVDBInfo");
            string apiKey = tvdbInfo.GetValue<string>("apikey");
            Console.WriteLine($"apiKey: {apiKey}");
            
        }

        static void ReadTVDBInfo() {
            string filePath = GetTVDBInfoFilePath();
            TVDBInfo tvdbInfo = TVDBInfo.ReadFromFile(filePath);
            tvdbInfo.TokenRetrieved = DateTime.Now.AddDays(-1);
            Console.WriteLine($"apiKey from TVDBInfo: {tvdbInfo.ApiKey}. DateTime: {tvdbInfo.TokenRetrieved}");
            tvdbInfo.SaveToFile(filePath);
        }

        static async void GetNewToken(IHttpClientFactory httpClientFactory) {
            string filePath = GetTVDBInfoFilePath();
            TVDBInfo tvdbInfo = TVDBInfo.ReadFromFile(filePath);
            if (tvdbInfo.TokenIsExpired) {
                TVDBRetrieverService service = new TVDBRetrieverService(httpClientFactory);
                string newToken = await service.FetchToken(tvdbInfo.ToAuthenticator());
                tvdbInfo.Token = newToken;
                tvdbInfo.TokenRetrieved = DateTime.Now;
                tvdbInfo.SaveToFile(filePath);
                Console.WriteLine($"Saved authentication info to {filePath}");
            }
            else if (tvdbInfo.TokenIsAlmostExpired) {
                // refresh the token
                Console.WriteLine("Will refresh the token");
            }
            else {
                Console.WriteLine("Token is still good");
            }
        }

        static TVDBInfo GetTVDBInfo() {
            string filePath = GetTVDBInfoFilePath();
            TVDBInfo tvdbInfo = TVDBInfo.ReadFromFile(filePath);
            return tvdbInfo;
        }

        static string GetTVDBInfoFilePath() {
            return Configuration.GetSection("TVDBInfo").GetValue<string>("filePath");
        }

        static void FindSampleShow(EpisodeContext context) {
            var shows = context.Shows
                .Where(s => s.SeriesId == 328487)
                .ToList();
            foreach (TVShow s in shows) {
                Console.WriteLine($"ID: {s.SeriesId}. Title: {s.SeriesName}");
            }
        }

        static void AddSampleShow(EpisodeContext context) {
            TVShow theOrville = new TVShow() {
                SeriesId = 328487,
                SeriesName = "The Orville",
                SeriesNamePreferred = null,
            };
            context.Add(theOrville);
            context.SaveChanges();
        }

        static void PrintDirectories(string appRoot) {
            Console.WriteLine($"Current directory is {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"{appRoot}");
        }
    }
}
