﻿using System;
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
