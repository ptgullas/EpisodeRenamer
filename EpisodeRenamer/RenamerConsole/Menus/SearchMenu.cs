using Renamer.Services;
using Renamer.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RenamerConsole.Menus {
    public class SearchMenu : IMenu {

        private RenamerFacade Facade;

        public SearchMenu(RenamerFacade facade) {
            Facade = facade;
        }
        public async Task DisplayMenu() {
            await SearchForShowsToAddtoDb();
        }

        private async Task SearchForShowsToAddtoDb() {
            MenuHelpers.WriteLineColor("Let's search for a show!");
            string userSearch = "temp";
            while (userSearch != "") {
                userSearch = PromptForSearchTerms();
                if (userSearch != "") {
                    await ProcessUserSearch(userSearch);
                }
            }
        }

        private static string PromptForSearchTerms() {
            string userSearch;
            Console.Write("Enter a show to search for (");
            MenuHelpers.WriteColor("blank to return to menu", ConsoleColor.DarkCyan);
            Console.WriteLine("):");
            userSearch = Console.ReadLine();
            return userSearch;
        }


        private async Task ProcessUserSearch(string userSearch) {
            List<TVShowFromTVDBDto> searchResults = await Facade.SearchForTVShows(userSearch);
            if (searchResults != null) {
                await DisplayResultsAndPromptToAddShow(searchResults);
            }
            else {
                MenuHelpers.WriteLineColor($"NO RESULTS FOUND FOR {userSearch}", ConsoleColor.Red);
            }
        }

        private async Task DisplayResultsAndPromptToAddShow(List<TVShowFromTVDBDto> searchResults) {
            string userShowSelection = "INITIAL";
            string exitCharUpper = "X";
            while (MenuHelpers.StringIsNotNumericOrExitChar(userShowSelection, exitCharUpper)) {
                userShowSelection = DisplaySearchResults(searchResults, exitCharUpper);
                if (userShowSelection.IsNumeric()) {
                    int userSelection = userShowSelection.ToInt();
                    int numberOfShows = searchResults.Count;
                    if ((userSelection >= 0) && (userSelection < numberOfShows)) {
                        TVShowFromTVDBDto selectedShow = searchResults[userSelection];
                        await ConfirmThenAddShowToDatabase(selectedShow);
                    }
                    else {
                        Console.WriteLine("That's not one of the shows on the list");
                        continue;
                    }
                }
                else if (userShowSelection.ToUpper() == exitCharUpper) {
                    return;
                }
            }
        }

        private string DisplaySearchResults(List<TVShowFromTVDBDto> showList, string exitCharUpper) {
            int numberOfShows = showList.Count;
            MenuHelpers.WriteLineColor("Search Results", ConsoleColor.White, ConsoleColor.DarkMagenta);
            for (int i = 0; i < numberOfShows; i++) {
                MenuHelpers.PrintMenuNumber(i);
                MenuHelpers.WriteColor($"{showList[i].SeriesNameTVDB}", ConsoleColor.Yellow);
                MenuHelpers.WriteColor(" | ");
                MenuHelpers.WriteColor($"{showList[i].Network}", ConsoleColor.Cyan);
                MenuHelpers.WriteColor(" | ");
                MenuHelpers.WriteColor("First Aired: ", ConsoleColor.Gray);
                MenuHelpers.WriteColor($"{showList[i].FirstAired}", ConsoleColor.Magenta);
                MenuHelpers.WriteColor(" | ");
                PrintShowStatus(showList[i].Status);
                MenuHelpers.WriteColor(" | ");
                MenuHelpers.WriteColor("SeriesId: ", ConsoleColor.Gray);
                MenuHelpers.WriteLineColor($"{showList[i].SeriesId}");
            }
            Console.Write($"Enter the number of the show to add it to DB (");
            MenuHelpers.WriteColor($"{ exitCharUpper} to return to Main Menu", ConsoleColor.DarkCyan);
            Console.WriteLine("):");
            string userInput = Console.ReadLine();
            return userInput;
        }

        private void PrintShowStatus(string showStatus) {
            ConsoleColor fontColor = ConsoleColor.Red;
            if (showStatus == "Continuing") {
                fontColor = ConsoleColor.Green;
            }
            MenuHelpers.WriteColor($"{showStatus}", fontColor);
        }

        private async Task ConfirmThenAddShowToDatabase(TVShowFromTVDBDto show) {
            if (ConfirmAdd(show)) {
                await Facade.AddShowToFavoritesThenToDatabase(show);
                await Facade.PopulateEpisodesFromSeriesId(show.SeriesId);
            }
        }

        private bool ConfirmAdd(TVShowFromTVDBDto show) {
            string response = "";
            bool addToDb = false;
            while (IsNotYesOrNo(response)) {
                MenuHelpers.WriteColor($"Add ");
                MenuHelpers.WriteColor($"{show.SeriesNameTVDB} ", ConsoleColor.Yellow);
                MenuHelpers.WriteColor($"to TVDB Favorites & database? (Y/N) ");
                response = Console.ReadLine().ToUpper();
                if (response == "Y") {
                    addToDb = true;
                }
                else if (response == "N") {
                    addToDb = false;
                }
                else {
                    continue;
                }
            }
            return addToDb;
        }

        private bool IsNotYesOrNo(string userInput) {
            if ((userInput.ToUpper() != "Y") && (userInput.ToUpper() != "N")) {
                return true;
            }
            else { return false; }
        }
    }
}
