using Renamer.Services;
using Renamer.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace RenamerConsole.Menus {
    public class MainMenu : IMenu {
        private RenamerFacade Facade;
        public MainMenu(RenamerFacade inputFacade) {
            Facade = inputFacade;
        }

        public async Task DisplayMenu() {
            TVDBInfo tvdbInfo = Facade._tvdbInfo;
            int userInput = 0;
            while (userInput != 9) {
                userInput = DisplayMainMenu(tvdbInfo);
                await ProcessMainMenuUserInput(userInput);
            }
        }

        private int DisplayMainMenu(TVDBInfo tvdbInfo) {
            bool tokenIsValid = !tvdbInfo.TokenIsExpired;
            tvdbInfo.PrintExpiration();
            Console.WriteLine("Episode Renamer!");
            Console.WriteLine();
            Console.Write("1. Get or refresh token:  ");
            DisplayTokenStatus(tokenIsValid);

            Console.WriteLine("2. Populate Shows table from User Favorites");
            Console.WriteLine("3. Populate Episodes for all existing shows");
            Console.WriteLine("4. Populate Episodes for a specific show");
            Console.Write("5. ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("RENAME FILES!!");
            Console.ResetColor();
            Console.WriteLine("6. Add Preferred Name for a show");
            Console.WriteLine("9. Exit, if you dare");
            var result = Console.ReadLine();
            if (result.IsNumeric()) {
                return result.ToInt();
            }
            else {
                return 0;
            }
        }

        private void DisplayTokenStatus(bool IsValid) {
            Console.BackgroundColor = ConsoleColor.Black;
            if (IsValid) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("(Token is Valid!)");
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("(Token Expired!)");
            }
            Console.ResetColor();
        }


        private async Task ProcessMainMenuUserInput(int selection) {
            if (selection == 1) {
                await Facade.FetchTokenIfNeeded();
            }
            else if (selection == 2) {
                await Facade.PopulateShowsFromFavorites();
            }
            else if (selection == 3) {
                await Facade.PopulateEpisodesFromExistingShows();
            }
            else if (selection == 4) {
                Console.WriteLine("Enter ID:");
                int mySeriesId = Console.ReadLine().ToInt();
                await Facade.PopulateEpisodesFromSeriesId(mySeriesId);
            }
            else if (selection == 5) {
                Console.WriteLine("OK we are getting down to business");
                Facade.RenameFiles();
            }
            else if (selection == 6) {
                PromptForPreferredName();
            }
        }

        private void PromptForPreferredName() {
            Console.WriteLine("What's the seriesId of the show you want to add a preferred name for?");
            string seriesIdInput = Console.ReadLine();
            int seriesId = -1;
            if (seriesIdInput != null) {
                while (!seriesIdInput.IsNumeric()) {
                    Console.WriteLine("That's not a number, fool!");
                    seriesIdInput = Console.ReadLine();
                }
                seriesId = seriesIdInput.ToInt();
                Log.Information($"PromptForPreferredName: User entered {seriesId}");
                string showName = Facade.FindTVShowNameBySeriesId(seriesId);
                Console.WriteLine($"What should be the preferred name of {showName}?");
                string preferredName = Console.ReadLine();
                if (preferredName != null) {
                    Log.Information($"PromptForPreferredName: User entered {preferredName}");
                    Facade.AddPreferredNameToTVShow(seriesId, preferredName);
                }

            }
        }


    }
}
