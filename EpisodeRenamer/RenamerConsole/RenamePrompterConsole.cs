using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Services.Models;
using RenamerConsole.Menus;

namespace RenamerConsole {
    public class RenamePrompterConsole : IRenamePrompter {
        public bool PromptForRename(string localFile, string targetName) {
            Console.Write($"Rename ");
            MenuHelpers.WriteColorVT24Bit($"{localFile} ", "#F92672");
            Console.Write("to ");
            MenuHelpers.WriteColorVT24Bit($"{targetName}", "#FFFF00");
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
