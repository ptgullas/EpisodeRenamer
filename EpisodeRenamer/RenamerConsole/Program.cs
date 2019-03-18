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
using Serilog;

namespace RenamerConsole {
    class Program {

        public static IConfigurationRoot Configuration;

        static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("c:\\temp\\logs\\renamerLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information("Started log on {a}", DateTime.Now.ToLongTimeString());
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

            TVDBRetrieverService retrieverService = new TVDBRetrieverService(httpClientFactory);
            TVShowService showService = new TVShowService(context);
            EpisodeService epService = new EpisodeService(context);
            string tvdbInfoPath = GetTVDBInfoFilePath();
            RenamerFacade facade = new RenamerFacade(retrieverService, showService, epService, context, tvdbInfoPath);

            DisplayMenuAndProcessUserInput(facade);
            // SetUpAutomapper();

            // GetNewToken(httpClientFactory);

            // GetApiKey();
            //ReadTVDBInfo();
            //FindSampleShow(context);
            PauseForInput();
            // Console.WriteLine("Adding show");
            //AddSampleShow(context);
        }

        static public async void DisplayMenuAndProcessUserInput(RenamerFacade facade) {
            TVDBInfo tvdbInfo = facade._tvdbInfo;
            int userInput = 0;
            do {
                userInput = DisplayMenu(tvdbInfo);
                await ProcessUserInput(userInput, facade);
            } while (userInput != 5);
        }

        static public int DisplayMenu(TVDBInfo tvdbInfo) {
            bool tokenIsValid = !tvdbInfo.TokenIsExpired;
            tvdbInfo.PrintExpiration();
            Console.WriteLine("Episode Renamer!");
            Console.WriteLine();
            Console.Write("1. Get or refresh token:  ");
            DisplayTokenStatus(tokenIsValid);

            Console.WriteLine("2. Populate Shows table from User Favorites");
            Console.WriteLine("3. Populate Doom Patrol episodes");
            Console.WriteLine("5. Exit if you dare");
            var result = Console.ReadLine();
            if (result.IsNumeric()) {
                return result.ToInt();
            }
            else {
                return 0;
            }
        }

        static async Task ProcessUserInput(int selection, RenamerFacade facade) {
            if (selection == 1) {
                facade.FetchTokenIfNeeded();
            }
            else if (selection == 2) {
                await facade.PopulateShowsFromFavorites();
            }
            else if (selection == 3) {
                await facade.PopulateEpisodes(355622);
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

        static async Task GetUserFavorites(TVDBInfo tvdbInfo, IHttpClientFactory httpClientFactory, DbContextOptions<EpisodeContext> options) {
            Console.WriteLine("Getting user favorites...");
            if (!tvdbInfo.TokenIsExpired) {
                TVDBRetrieverService retrieverService = new TVDBRetrieverService(httpClientFactory);
                List<int> faves = await retrieverService.FetchUserFavorites(tvdbInfo.Token);
                faves.Sort();
                faves
                    .ForEach(c => Console.WriteLine(c));
                EpisodeContext context = new EpisodeContext(options);
                var seriesIdsInDb = context.Shows.Select(s => s.SeriesId);
                var seriesIdsNotInDb = faves.Except(seriesIdsInDb);
                // foreach seriesId not in DB, fetch it & add to Shows table
            }
        }

        static async Task GetShow(TVDBInfo tvdbInfo, IHttpClientFactory httpClientFactory) {
            Console.WriteLine("What seriesId should we get?");
            if (!tvdbInfo.TokenIsExpired) {
                TVDBRetrieverService service = new TVDBRetrieverService(httpClientFactory);
                TVShowFromTVDBDto showDto = await service.FetchTVShow(356640, tvdbInfo.Token);
                if (showDto.SeriesId != 0) {
                    TVShow show = showDto.ToTVShow();
                    Console.Write($"Retrieved show: SeriesId: ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($"{show.SeriesId}");
                    Console.ResetColor();
                    Console.Write(", Name: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(show.SeriesName);
                    Console.ResetColor();

                }
                else {
                    Console.WriteLine("Couldn't retrieve show");
                }
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
            tvdbInfo.TokenRetrievedDate = DateTime.Now.AddDays(-1);
            Console.WriteLine($"apiKey from TVDBInfo: {tvdbInfo.ApiKey}. DateTime: {tvdbInfo.TokenRetrievedDate}");
            tvdbInfo.SaveToFile(filePath);
        }

        static async void RetrieveTokenIfNeeded(TVDBRetrieverService retrieverService) {
            string filePath = GetTVDBInfoFilePath();
            TVDBInfo tvdbInfo = TVDBInfo.ReadFromFile(filePath);
            if (tvdbInfo.TokenIsExpired) {
                Console.WriteLine("Token is expired. Fetching new one....");
                string newToken = await retrieverService.FetchNewToken(tvdbInfo.ToAuthenticator());
                SetNewTokenAndSaveToFile(ref tvdbInfo, newToken, filePath);
                Console.WriteLine($"Saved authentication info to {filePath}");
            }
            else if (tvdbInfo.TokenIsAlmostExpired) {
                // refresh the token
                Console.WriteLine("Token will expire soon. Will refresh it");
                string newToken = await retrieverService.FetchRefreshToken(tvdbInfo.Token);
                SetNewTokenAndSaveToFile(ref tvdbInfo, newToken, filePath);
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

        static void SetNewTokenAndSaveToFile(ref TVDBInfo tvdbInfo, string token, string filePath) {
            tvdbInfo.Token = token;
            tvdbInfo.TokenRetrievedDate = DateTime.Now;
            tvdbInfo.SaveToFile(filePath);
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
