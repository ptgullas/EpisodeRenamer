using System;
using Renamer.Data;
using Renamer.Data.Entities;

namespace RenamerConsole {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Adding show");
            AddSampleShow();
        }

        static void AddSampleShow() {
            TVShow theOrville = new TVShow() {
                SeriesId = 328487,
                SeriesName = "The Orville",
                SeriesNamePreferred = null,
            };
            var context = new EpisodeContext();
            context.Add(theOrville);
            context.SaveChanges();
        }
    }
}
