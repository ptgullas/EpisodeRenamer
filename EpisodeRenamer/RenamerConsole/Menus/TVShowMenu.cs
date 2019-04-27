using Renamer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Renamer.Services;

namespace RenamerConsole.Menus {
    public class TVShowMenu : IMenu {

        private EpisodeContext Context;
        private RenamerFacade Facade;
        private TVShowService ShowService;

        public TVShowMenu(EpisodeContext context, RenamerFacade facade, TVShowService showService) {
            Context = context;
            Facade = facade;
            ShowService = showService;
        }

        public async Task DisplayMenu() {
            TVShow[] showArray = GetShows();
            TVShow selectedShow = new TVShow() {
                SeriesName = "TempShow"
            };
            do {
                selectedShow = DisplayTVShowMenu(showArray);
                if (selectedShow != null) {
                    await DisplayIndividualShowMenu(selectedShow);
                }
            } while (selectedShow != null);
            
        }

        private string DisplayListOfShows(TVShow[] showArray, string exitCharUpper) {
            int numberOfShows = showArray.Count();
            Console.WriteLine("TV Show Menu");
            for (int i = 0; i < numberOfShows; i++) {
                MenuHelpers.PrintMenuNumber(i);
                MenuHelpers.DisplayShowName(showArray[i].SeriesName, showArray[i].IsActive);
                MenuHelpers.DisplayShowActiveStatus(showArray[i].IsActive);
            }
            Console.Write($"Enter the number of the show (");
            MenuHelpers.WriteColor($"{ exitCharUpper} to return to Main Menu", ConsoleColor.DarkCyan);
            Console.WriteLine("):");
            string userInput = Console.ReadLine();
            return userInput;
        }

        private void DisplayShowName(TVShow show) {
            var displayColor = ConsoleColor.White;
            MenuHelpers.WriteColor($"{show.SeriesName} ", displayColor);
        }


        private TVShow DisplayTVShowMenu(TVShow[] showArray) {

            int userSelection = -1;
            string userInput = "INITIAL";
            string exitCharUpper = "X";
            int numberOfShows = showArray.Count();
            while (StringIsNotNumericOrExitChar(userInput, exitCharUpper)) {
                userInput = DisplayListOfShows(showArray, exitCharUpper);
                if (userInput.IsNumeric()) {
                    userSelection = userInput.ToInt();
                    if ((userSelection >= 0) || (userSelection < numberOfShows)) {
                        return showArray[userSelection];
                    }
                    else {
                        Console.WriteLine("That's not a valid selection, dude");
                        continue;
                        // at this point, we return to the while loop
                    }
                }
                else if (userInput.ToUpper() == exitCharUpper) {
                    return null;
                }
            }
            // if user selected "X", it returns null
            // if user selected a valid number, it returns the corresponding TV Show
            // if invalid number or string that isn't "X", it stays in the loop
            // so we should never actually hit this
            return showArray[userSelection];
        }

        private TVShow[] GetShows() {
            TVShow[] shows = Context.Shows.OrderBy(s => s.SeriesName).ToArray();
            return shows;
        }

        private bool StringIsNotNumericOrExitChar(string s, string exit) {
            if ((!s.IsNumeric() || s.ToUpper() != exit)) {
                return true;
            }
            else {
                return false;
            }
        }

        private async Task DisplayIndividualShowMenu(TVShow selectedShow) {
            IndividualShowMenu individualShowMenu = new IndividualShowMenu(Context, Facade, ShowService, selectedShow);
            await individualShowMenu.DisplayMenu();
        }
    }
}
