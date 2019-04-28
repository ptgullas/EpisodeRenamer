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
                    List<TVShowFromTVDBDto> searchResults = await Facade.SearchForTVShows(userSearch);
                    if (searchResults != null) {
                        int userSelection = -1;
                        string userShowSelection = "INITIAL";
                        int numberOfShows = searchResults.Count;
                        TVShowFromTVDBDto selectedShow = new TVShowFromTVDBDto();
                        string exitCharUpper = "X";
                        while (MenuHelpers.StringIsNotNumericOrExitChar(userShowSelection, exitCharUpper)) {
                            userShowSelection = DisplaySearchResults(searchResults, exitCharUpper);
                            if (userShowSelection.IsNumeric()) {
                                userSelection = userShowSelection.ToInt();
                                if ((userSelection >= 0) && (userSelection < numberOfShows)) {
                                    // Add show to favorites
                                    // Add it to DB
                                    MenuHelpers.WriteColor("We will do something with ");
                                    MenuHelpers.WriteLineColor($"{ searchResults[userSelection].SeriesNameTVDB}", ConsoleColor.Yellow);
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
                    else {
                        MenuHelpers.WriteLineColor($"NO RESULTS FOUND FOR {userSearch}", ConsoleColor.Red);
                    }
                }

            }

        }

        private static string PromptForSearchTerms() {
            string userSearch;
            Console.WriteLine("Enter a show to search for (blank to return to menu): ");
            userSearch = Console.ReadLine();
            return userSearch;
        }

        private string DisplaySearchResults(List<TVShowFromTVDBDto> showList, string exitCharUpper) {
            int numberOfShows = showList.Count;
            Console.WriteLine("SearchResults");
            for (int i = 0; i < numberOfShows; i++) {
                MenuHelpers.PrintMenuNumber(i);
                MenuHelpers.WriteLineColor($"{showList[i].SeriesNameTVDB} | {showList[i].Network} | {showList[i].Status} | {showList[i].SeriesId}");
            }
            Console.Write($"Enter the number of the show (");
            MenuHelpers.WriteColor($"{ exitCharUpper} to return to Main Menu", ConsoleColor.DarkCyan);
            Console.WriteLine("):");
            string userInput = Console.ReadLine();
            return userInput;
        }

    }
}
