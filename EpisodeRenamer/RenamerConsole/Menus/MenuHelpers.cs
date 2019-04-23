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

        public static void WriteColor(string str, ConsoleColor fontColor = ConsoleColor.Magenta, ConsoleColor backColor = ConsoleColor.Black) {
            Console.ForegroundColor = fontColor;
            Console.BackgroundColor = backColor;
            Console.Write(str);
            Console.ResetColor();
        }

        public static void WriteLineColor(string str, ConsoleColor fontColor = ConsoleColor.Magenta, ConsoleColor backColor = ConsoleColor.Black) {
            Console.ForegroundColor = fontColor;
            Console.BackgroundColor = backColor;
            Console.WriteLine(str);
            Console.ResetColor();
        }


    }
}
