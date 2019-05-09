using Renamer.Services;
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

        public static void WriteColor(string str, ConsoleColor fontColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black) {
            Console.ForegroundColor = fontColor;
            Console.BackgroundColor = backColor;
            Console.Write(str);
            Console.ResetColor();
        }

        public static void WriteColorVT8Bit(string str, int color) {
            Console.Write($"\u001b[38;5;{color}m{str}\u001b[0m");
        }
        public static void WriteLineColorVT8Bit(string str, int color) {
            Console.WriteLine($"\u001b[38;5;{color}m{str}\u001b[0m");
        }

        public static void WriteColorVT24Bit(string str, int r, int g, int b) {
            Console.Write($"\u001b[38;2;{r};{g};{b}m{str}\u001b[0m");
        }
        public static void WriteLineColorVT24Bit(string str, int r, int g, int b) {
            Console.WriteLine($"\u001b[38;2;{r};{g};{b}m{str}\u001b[0m");
        }

        public static void WriteLineColor(string str, ConsoleColor fontColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black) {
            Console.ForegroundColor = fontColor;
            
            Console.BackgroundColor = backColor;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        public static void DisplayShowActiveStatus(bool isActive) {
            if (isActive) {
                MenuHelpers.WriteLineColor("(Active)", ConsoleColor.Green);
            }
            else {
                MenuHelpers.WriteLineColor("(Inactive)", ConsoleColor.DarkGray);
            }
        }

        public static void DisplayShowName(string seriesName, bool isActive) {
            var showColor = ConsoleColor.Yellow;
            if (!isActive) { showColor = ConsoleColor.DarkYellow; }
            WriteColor($"{seriesName} ", showColor);

        }

        public static bool StringIsNotNumericOrExitChar(string s, string exit) {
            if ((!s.IsNumeric() || s.ToUpper() != exit)) {
                return true;
            }
            else {
                return false;
            }
        }

    }
}
