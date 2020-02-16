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
            MenuHelpers.WriteLineColorVT24Bit("Display all episodes in database", "#7a918d");
            MenuHelpers.PrintMenuNumber(2);
            MenuHelpers.WriteLineColorVT24Bit("Check for new/updated episodes in TVDB", "#93b1a7");
            MenuHelpers.PrintMenuNumber(3);
            MenuHelpers.WriteLineColorVT24Bit("Add/Change Preferred Name", "#99c2a2");
            MenuHelpers.PrintMenuNumber(4);
            MenuHelpers.WriteLineColorVT24Bit($"Toggle Active Status", "#c5edac");
            MenuHelpers.PrintMenuNumber(5);
            MenuHelpers.WriteLineColorVT24Bit($"Get episodes from specific Json page", "#99c2a2");
            MenuHelpers.PrintMenuNumber(9);
            MenuHelpers.WriteLineColor("Return to List of Shows", ConsoleColor.DarkCyan);
            MenuHelpers.WriteColorVT24Bit($"Enter your selection: ", "#C0D684");
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
            MenuHelpers.WriteColorVT24Bit($"SeriesId: ", "#adcad6");
            MenuHelpers.WriteColor($"{Show.SeriesId} ", ConsoleColor.White);
            MenuHelpers.WriteColor("| ", ConsoleColor.Magenta);
            MenuHelpers.WriteColorVT24Bit("Preferred Name: ", "#adcad6");
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
            else if (selection == 5) {
                await PromptForSpecificJsonPageToDownload();
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

        private async Task PromptForSpecificJsonPageToDownload() {
            Console.WriteLine($"Enter the specific page you want to download");
            string pageToDownload = Console.ReadLine();
            if (!pageToDownload.IsNumeric()) {
                Console.WriteLine("That's not a number!");
            }
            else {
                await Facade.PopulateEpisodesFromSeriesIdFromSpecificPage(Show.SeriesId, pageToDownload.ToInt());
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
            MenuHelpers.WriteColorVT24Bit($"{ep.Season}.{epNumber.ToString("D2")}", "#d88373");
            MenuHelpers.WriteColor(" - ", ConsoleColor.DarkGray);
            MenuHelpers.WriteLineGradientWhiteToBlue($"{ep.EpisodeName}", 30);
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
