using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Renamer.Services.Models;
using Renamer.Data;
using Renamer.Data.Entities;
using AutoMapper;

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

            var options = new DbContextOptionsBuilder<EpisodeContext>()
                .UseSqlite(Configuration.GetConnectionString("connectionString"))
                .Options;
            var context = new EpisodeContext(options);

            SetUpAutomapper();

            FindSampleShow(context);
            // Console.WriteLine("Adding show");
            //AddSampleShow(context);
        }

        static void SetUpAutomapper() {
            Mapper.Initialize(config => {
                config.CreateMap<EpisodeDto, Episode>();
                config.CreateMap<TVShowDto, TVShow>();
            });
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
