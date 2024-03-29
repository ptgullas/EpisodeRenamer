﻿using Renamer.Services;
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
        private TVShowService ShowService;
        public MainMenu(RenamerFacade inputFacade, EpisodeContext context, TVShowService showService) {
            Facade = inputFacade;
            Context = context;
            ShowService = showService;
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
            bool tokenIsValid = !tvdbInfo.TokenIsInvalid;
            PrintTokenExpiration(tvdbInfo);
            MenuHelpers.WriteLineColor("\u001b[4mEpisode Renamer!\u001b[0m", ConsoleColor.Yellow, ConsoleColor.DarkMagenta);
            // MenuHelpers.WriteLineGradientWhiteToBlue("Episode Renamer!");
            Console.WriteLine();

            MenuHelpers.PrintMenuNumber(1);
            MenuHelpers.WriteColorVT24Bit("Get or refresh token  ", "#e56399");
            DisplayTokenStatus(tokenIsValid);

            MenuHelpers.PrintMenuNumber(2);
            MenuHelpers.WriteLineColorVT24Bit("Populate Shows table from User Favorites", "#e5d4ce");

            MenuHelpers.PrintMenuNumber(3);
            MenuHelpers.WriteLineColorVT24Bit("Populate Episodes for existing Active shows", "#7fd1b9");

            MenuHelpers.PrintMenuNumber(4);
            MenuHelpers.WriteLineColorVT24Bit("TV Show menu", "#FF6C2C");

            MenuHelpers.PrintMenuNumber(5);
            MenuHelpers.WriteLineColor("RENAME FILES!!", ConsoleColor.Yellow);

            MenuHelpers.PrintMenuNumber(6);
            MenuHelpers.WriteLineColor("Search for shows on TVDB", ConsoleColor.White);

            MenuHelpers.PrintMenuNumber(7);
            MenuHelpers.WriteLineColor("Change local folder (temporarily)", ConsoleColor.DarkRed);

            MenuHelpers.PrintMenuNumber(9);
            MenuHelpers.WriteLineColor("Exit, if you dare", ConsoleColor.DarkCyan);
            var result = Console.ReadLine();
            if (result.IsNumeric()) {
                return result.ToInt();
            }
            else {
                return 0;
            }
        }

        private void PrintTokenExpiration(TVDBInfo tvdbInfo) {
            DateTime expiration = tvdbInfo.GetExpiration();
            string expireWord = "expires";
            if (tvdbInfo.TokenIsExpired) { expireWord = "expired"; }
            MenuHelpers.WriteColorVT24Bit($"Token {expireWord} on ", "#dbbbf5");
            MenuHelpers.WriteColorVT24Bit($"{expiration.ToString("MM/dd/yyyy hh:mm tt")}", "#ce5374");
            Console.WriteLine();
        }

        private void DisplayTokenStatus(bool IsValid) {
            if (IsValid) {
                // MenuHelpers.WriteLineColor("(Token is Valid! \u306F hey)", ConsoleColor.Green);
                MenuHelpers.WriteLineColorVT24Bit("(Token is Valid!)", "#00FF00");
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
                await Facade.PopulateEpisodesFromExistingActiveShows();
            }
            else if (selection == 4) {
                await CreateTVShowMenu();
            }
            else if (selection == 5) {
                Console.WriteLine("OK we are getting down to business");
                Facade.RenameFiles();
            }
            else if (selection == 6) {
                await CreateSearchMenu();
            }
            else if (selection == 7) {
                CreateFolderChangeMenu();
            }
            else if (selection == 9) {
                return;
            }
            else {
                MenuHelpers.WriteLineColor("Not a valid choice, bro!", ConsoleColor.Red);
            }
        }

        private async Task CreateSearchMenu() {
            SearchMenu searchMenu = new SearchMenu(Facade);
            await searchMenu.DisplayMenu();
        }


        private async Task CreateTVShowMenu() {
            TVShowMenu tvShowMenu = new TVShowMenu(Context, Facade, ShowService);
            await tvShowMenu.DisplayMenu();
        }

        private void CreateFolderChangeMenu() {
            FolderChangeMenu folderChangeMenu = new FolderChangeMenu(Facade);
            folderChangeMenu.DisplayMenu();
        }

    }
}
