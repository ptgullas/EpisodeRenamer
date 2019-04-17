using Renamer.Services;
using Renamer.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Renamer.Data.Entities;

namespace RenamerConsole.Menus {
    public class MainMenu : IMenu {
        private RenamerFacade Facade;
        private EpisodeContext Context;
        public MainMenu(RenamerFacade inputFacade, EpisodeContext context) {
            Facade = inputFacade;
            Context = context;
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
            Console.WriteLine("4. TV Show menu");
            Console.Write("5. ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("RENAME FILES!!");
            Console.ResetColor();
            Console.Write("9. ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Exit, if you dare");
            Console.ResetColor();
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
                await CreateTVShowMenu();
            }
            else if (selection == 5) {
                Console.WriteLine("OK we are getting down to business");
                Facade.RenameFiles();
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not a valid choice, bro!");
                Console.ResetColor();
            }
        }

        private async Task CreateTVShowMenu() {
            TVShowMenu tvShowMenu = new TVShowMenu(Context, Facade);
            await tvShowMenu.DisplayMenu();
        }

    }
}
