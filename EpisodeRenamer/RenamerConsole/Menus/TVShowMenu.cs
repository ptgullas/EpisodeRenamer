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

        public TVShowMenu() {
            Context = new EpisodeContext();
        }

        public async Task DisplayMenu() {
            TVShow selectedShow = new TVShow() {
                SeriesName = "TempShow"
            };
            do {
                selectedShow = DisplayTVShowMenu();
                if (selectedShow != null) {

                }
            } while (selectedShow != null);
            
        }

        private TVShow DisplayTVShowMenu() {
            TVShow[] showArray = GetShows();

            int userSelection = -1;
            string userInput = "INITIAL";
            string exitCharUpper = "X";
            while (StringIsNotNumericOrExitChar(userInput, exitCharUpper)) {
                Console.WriteLine("TV Show Menu");
                for (int i = 0; i < showArray.Count(); i++) {
                    Console.WriteLine($"{ i }\t{ showArray[i].SeriesName }");
                }
                Console.WriteLine("Enter the number of the show (X to exit):");
                userInput = Console.ReadLine();
                if (userInput.IsNumeric()) {
                    userSelection = userInput.ToInt();
                    if ((userSelection < 0) || (userSelection > showArray.Count())) {
                        Console.WriteLine("That's not a valid selection, dude");
                    }
                }
                else if (userInput == exitCharUpper) {
                    return null;
                }
            }
            // if user selected "X", it exits the loop and 
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

        private async void DisplayIndividualShowMenu(TVShow selectedShow) {
            Console.WriteLine($"Selected: {selectedShow.SeriesName} (SeriesId: {selectedShow.SeriesId}). Preferred Name: {selectedShow.SeriesNamePreferred}");
            Console.WriteLine("1. Display all episodes in database");
            Console.WriteLine("2. Display seasons in database");
            Console.WriteLine("3. Check for new/updated episodes in TVDB");
        }
    }
}
