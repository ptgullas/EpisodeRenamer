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
using RenamerConsole.Menus;

namespace RenamerConsole {
    class Program {

        public static IConfigurationRoot Configuration;

        static async Task Main(string[] args) {
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
            LocalMediaService localService = new LocalMediaService(Configuration.GetSection("LocalMedia").GetValue<string>("directoryPath"));
            string tvdbInfoPath = GetTVDBInfoFilePath();
            RenamerFacade facade = new RenamerFacade(retrieverService, showService, epService, localService, context, tvdbInfoPath);

            MainMenu mainMenu = new MainMenu(facade, context);
            await mainMenu.DisplayMenu();
            // await DisplayMenuAndProcessUserInput(facade);
            // SetUpAutomapper();

            // GetNewToken(httpClientFactory);

            // GetApiKey();
            //ReadTVDBInfo();
            // FindSampleShow(context);
            // PauseForInput();
            // Console.WriteLine("Adding show");
            //AddSampleShow(context);
        }

        static public async Task DisplayMenuAndProcessUserInput(RenamerFacade facade) {
            TVDBInfo tvdbInfo = facade._tvdbInfo;
            int userInput = 0;
            while (userInput != 9) {
                userInput = DisplayMenu(tvdbInfo);
                await ProcessUserInput(userInput, facade);
            }
        }

        static public int DisplayMenu(TVDBInfo tvdbInfo) {
            bool tokenIsValid = !tvdbInfo.TokenIsExpired;
            tvdbInfo.PrintExpiration();
            Console.WriteLine("Episode Renamer!");
            Console.WriteLine();
            Console.Write("1. Get or refresh token:  ");
            DisplayTokenStatus(tokenIsValid);

            Console.WriteLine("2. Populate Shows table from User Favorites");
            Console.WriteLine("3. Populate Episodes for all existing shows");
            Console.WriteLine("4. Populate Episodes for a specific show");
            Console.Write("5. ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("RENAME FILES!!");
            Console.ResetColor();
            Console.WriteLine("6. Add Preferred Name for a show");
            Console.WriteLine("9. Exit, if you dare");
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
                await facade.FetchTokenIfNeeded();
            }
            else if (selection == 2) {
                await facade.PopulateShowsFromFavorites();
            }
            else if (selection == 3) {
                await facade.PopulateEpisodesFromExistingShows();
            }
            else if (selection == 4) {
                Console.WriteLine("Enter ID:");
                int mySeriesId = Console.ReadLine().ToInt();
                await facade.PopulateEpisodesFromSeriesId(mySeriesId);
            }
            else if (selection == 5) {
                Console.WriteLine("OK we are getting down to business");
                facade.RenameFiles();
            }
            else if (selection == 6) {
                PromptForPreferredName(facade);
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

        static public void PromptForPreferredName(RenamerFacade facade) {
            Console.WriteLine("What's the seriesId of the show you want to add a preferred name for?");
            string seriesIdInput = Console.ReadLine();
            int seriesId = -1;
            if (seriesIdInput != null) {
                while (!seriesIdInput.IsNumeric()) {
                    Console.WriteLine("That's not a number, fool!");
                    seriesIdInput = Console.ReadLine();
                }
                seriesId = seriesIdInput.ToInt();
                Log.Information($"PromptForPreferredName: User entered {seriesId}");
                string showName = facade.FindTVShowNameBySeriesId(seriesId);
                Console.WriteLine($"What should be the preferred name of {showName}?");
                string preferredName = Console.ReadLine();
                if (preferredName != null) {
                    Log.Information($"PromptForPreferredName: User entered {preferredName}");
                    facade.AddPreferredNameToTVShow(seriesId, preferredName);
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
