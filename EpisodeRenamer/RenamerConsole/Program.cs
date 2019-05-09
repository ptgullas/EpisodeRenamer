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
using System.Runtime.InteropServices;

namespace RenamerConsole {
    class Program {

        public static IConfigurationRoot Configuration;

        // for VT100 support:
        // ReSharper disable InconsistentNaming
        private const int STD_INPUT_HANDLE = -10;
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;
        private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;
        // ReSharper restore InconsistentNaming
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        static async Task Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("c:\\temp\\logs\\renamerLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information("Started log on {a}", DateTime.Now.ToLongTimeString());

            SetUpVT100();

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
            RenamePrompterConsole prompterConsole = new RenamePrompterConsole();
            RenamerFacade facade = new RenamerFacade(retrieverService, showService, epService, localService, context, prompterConsole, tvdbInfoPath);

            MainMenu mainMenu = new MainMenu(facade, context, showService);
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

        static void SetUpVT100() {
            var iStdIn = GetStdHandle(STD_INPUT_HANDLE);
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(iStdIn, out uint inConsoleMode)) {
                Console.WriteLine("failed to get input console mode");
                Console.ReadKey();
                return;
            }

            if (!GetConsoleMode(iStdOut, out uint outConsoleMode)) {
                Console.WriteLine("failed to get output console mode");
                Console.ReadKey();
                return;
            }
            inConsoleMode |= ENABLE_VIRTUAL_TERMINAL_INPUT;
            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(iStdIn, inConsoleMode)) {
                Console.WriteLine($"failed to set input console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }

            if (!SetConsoleMode(iStdOut, outConsoleMode)) {
                Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
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
