using Renamer.Data.Entities;
using Renamer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamerConsole.Menus {
    public class IndividualShowMenu : IMenu {

        private EpisodeContext Context;
        private RenamerFacade Facade;
        private TVShow Show;
        public IndividualShowMenu(EpisodeContext context, RenamerFacade facade, TVShow show)
        {
            Context = context;
            Facade = facade;
            Show = show;
        }
        public async Task DisplayMenu() {
            int userInput = 0;
            while (userInput != 9) {
                userInput = DisplayIndividualShowMenu(Show);
                await ProcessUserSelection(userInput);
            }
        }

        private int DisplayIndividualShowMenu(TVShow selectedShow) {
            Console.WriteLine($"Selected: {selectedShow.SeriesName} (SeriesId: {selectedShow.SeriesId}). Preferred Name: {selectedShow.SeriesNamePreferred}");
            Console.WriteLine("1. Display all episodes in database");
            Console.WriteLine("2. Check for new/updated episodes in TVDB");
            Console.WriteLine("3. Add/Change Preferred Name");
            Console.Write("9. ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Exit!!");
            Console.ResetColor();
            Console.WriteLine($"Enter your selection:");
            string userInput = Console.ReadLine();
            if (userInput.IsNumeric()) {
                return userInput.ToInt();
            }
            else {
                return 0;
            }
        }

        private async Task ProcessUserSelection(int selection) {
            if (selection == 1) {
                DisplayEpisodesInDatabase();
            }
            else if (selection == 2) {
                await Facade.PopulateEpisodesFromSeriesId(Show.SeriesId);
            }
            else if (selection == 3) {
                PromptForPreferredName();
            }
            else if (selection == 9) {
                return;
            }
            else {
                Console.WriteLine("Not implemented");
            }
        }

        private void PromptForPreferredName() {
            Console.WriteLine($"Enter your Preferred Name for {Show.SeriesName} (Blank to skip)");
            string preferredName = Console.ReadLine();
            if (preferredName != "") {
                Facade.AddPreferredNameToTVShow(Show.SeriesId, preferredName);
            }
        }

        private void DisplayEpisodesInDatabase() {
            List<Episode> episodes = GetEpisodesForShow(Show);
            if (episodes != null) {
                foreach (Episode ep in episodes) {
                    PrintEpisodeInfo(ep);
                }
            }
        }

        private static void PrintEpisodeInfo(Episode ep) {
            int epNumber = (int)ep.AiredEpisodeNumber;
            Console.WriteLine($"{ep.Season}.{epNumber.ToString("D2")} - {ep.EpisodeName}");
        }

        private List<Episode> GetEpisodesForShow(TVShow show) {
            return Context.Episodes
                .Where(b => b.SeriesId == show.SeriesId)
                .OrderBy(b => b.Season)
                .ThenBy(b => b.AiredEpisodeNumber)
                .ToList();
        }
    }
}
