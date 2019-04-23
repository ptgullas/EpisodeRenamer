using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Services.Models;
using RenamerConsole.Menus;

namespace RenamerConsole {
    public class RenamePrompterConsole : IRenamePrompter {
        public bool PromptForRename(string localFile, string targetName) {
            Console.Write($"Rename ");
            MenuHelpers.WriteColor($"{localFile} ", ConsoleColor.White);
            Console.Write("to ");
            MenuHelpers.WriteColor($"{targetName}", ConsoleColor.Yellow);
            Console.WriteLine("?");
            string userInput = Console.ReadLine().ToUpper();
            if (userInput == "Y") {
                return true;
            }
            else {
                return false;
            }

        }
    }
}
