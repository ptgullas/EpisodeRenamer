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
        private TVShowService ShowService;
        private TVShow Show;
        public IndividualShowMenu(EpisodeContext context, RenamerFacade facade, TVShowService showService, TVShow show)
        {
            Context = context;
            Facade = facade;
            ShowService = showService;
            Show = show;
        }
        public async Task DisplayMenu() {
            int userInput = 0;
            while (userInput != 9) {
                userInput = DisplayIndividualShowMenu();
                await ProcessUserSelection(userInput);
            }
        }

        private int DisplayIndividualShowMenu() {
            DisplayShowBanner();
            MenuHelpers.PrintMenuNumber(1);
            Console.WriteLine("Display all episodes in database");
            MenuHelpers.PrintMenuNumber(2);
            Console.WriteLine("Check for new/updated episodes in TVDB");
            MenuHelpers.PrintMenuNumber(3);
            Console.WriteLine("Add/Change Preferred Name");
            MenuHelpers.PrintMenuNumber(4);
            Console.WriteLine($"Toggle Active Status");
            MenuHelpers.PrintMenuNumber(9);
            MenuHelpers.WriteLineColor("Return to List of Shows", ConsoleColor.DarkCyan);
            Console.WriteLine($"Enter your selection:");
            string userInput = Console.ReadLine();
            if (userInput.IsNumeric()) {
                return userInput.ToInt();
            }
            else {
                return 0;
            }
        }

        private void DisplayShowBanner() {
            MenuHelpers.DisplayShowName(Show.SeriesName, Show.IsActive);
            MenuHelpers.DisplayShowActiveStatus(Show.IsActive);
            Console.Write($"(SeriesId: ");
            MenuHelpers.WriteColor($"{Show.SeriesId}", ConsoleColor.White);
            Console.Write("). Preferred Name: ");
            MenuHelpers.WriteLineColor($"{Show.SeriesNamePreferred}", ConsoleColor.Yellow);

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
            else if (selection == 4) {
                ToggleActiveStatus();
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
                ShowService.AddPreferredName(Show.SeriesId, preferredName);
            }
        }

        private void ToggleActiveStatus() {
            ShowService.ToggleShowActiveStatus(Show.SeriesId);
            Show = ShowService.FindBySeriesId(Show.SeriesId);
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
