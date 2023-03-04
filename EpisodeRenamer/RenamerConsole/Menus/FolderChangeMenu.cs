using Renamer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamerConsole.Menus {
    internal class FolderChangeMenu {
        private RenamerFacade Facade;

        public FolderChangeMenu(RenamerFacade facade) {
            Facade = facade;
        }

        public void DisplayMenu() {
            DisplayCurrentFolder();
            PromptAndChangeCurrentFolder();
        }

        private void PromptAndChangeCurrentFolder() {
            string newFolder = "temp";
            while (newFolder != string.Empty) {
                newFolder = PromptForNewFolder();
                if (newFolder != string.Empty) {
                    bool success = Facade.ChangeLocalDirectory(newFolder);
                    if (!success) {
                        MenuHelpers.WriteLineColor($"Invalid folder {newFolder}!", ConsoleColor.Red);
                    }
                    else {
                        MenuHelpers.WriteLineColor($"Folder changed! New folder is:", ConsoleColor.Green);
                        MenuHelpers.WriteLineGradientCoolBlue(newFolder);
                        newFolder = string.Empty;
                    }
                }
            }
        }

        private void DisplayCurrentFolder() {
            string currentDirectory = Facade.GetLocalDirectory();
            MenuHelpers.WriteLineColor("Current Local Directory is:", ConsoleColor.Green);
            MenuHelpers.WriteLineGradientYellowToGreen(currentDirectory);
        }

        private static string PromptForNewFolder() {
            string newFolder;
            Console.Write("Enter a new folder (");
            MenuHelpers.WriteColor("blank to return to menu", ConsoleColor.DarkCyan);
            Console.WriteLine("):");
            newFolder = Console.ReadLine();
            return newFolder;
        }
    }
}
