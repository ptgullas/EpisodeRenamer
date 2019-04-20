using System;
using System.Collections.Generic;
using System.Text;

namespace RenamerConsole.Menus {
    public static class MenuHelpers {
        public static void PrintMenuNumber(int menuNumber, ConsoleColor fontColor = ConsoleColor.Magenta) {
            Console.ForegroundColor = fontColor;
            Console.Write($"{menuNumber}.   ");
            Console.ResetColor();
        }

    }
}
