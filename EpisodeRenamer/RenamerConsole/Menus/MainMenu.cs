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
            MenuHelpers.WriteLineColor("Episode Renamer!", ConsoleColor.Yellow, ConsoleColor.DarkMagenta);
            Console.WriteLine();

            MenuHelpers.PrintMenuNumber(1);
            Console.Write("Get or refresh token  ");
            DisplayTokenStatus(tokenIsValid);

            MenuHelpers.PrintMenuNumber(2);
            Console.WriteLine("Populate Shows table from User Favorites");

            MenuHelpers.PrintMenuNumber(3);
            Console.WriteLine("Populate Episodes for all existing shows");

            MenuHelpers.PrintMenuNumber(4);
            Console.WriteLine("TV Show menu");

            MenuHelpers.PrintMenuNumber(5);
            MenuHelpers.WriteLineColor("RENAME FILES!!", ConsoleColor.Cyan);

            MenuHelpers.PrintMenuNumber(9);
            MenuHelpers.WriteLineColor("Exit, if you dare", ConsoleColor.DarkYellow);
            var result = Console.ReadLine();
            if (result.IsNumeric()) {
                return result.ToInt();
            }
            else {
                return 0;
            }
        }

        private void DisplayTokenStatus(bool IsValid) {
            if (IsValid) {
                MenuHelpers.WriteLineColor("(Token is Valid!)", ConsoleColor.Green);
            }
            else {
                MenuHelpers.WriteLineColor("(Token Expired!)", ConsoleColor.Red);
            }
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
            else if (selection == 9) {
                return;
            }
            else {
                MenuHelpers.WriteLineColor("Not a valid choice, bro!", ConsoleColor.Red);
            }
        }

        private async Task CreateTVShowMenu() {
            TVShowMenu tvShowMenu = new TVShowMenu(Context, Facade);
            await tvShowMenu.DisplayMenu();
        }

    }
}
