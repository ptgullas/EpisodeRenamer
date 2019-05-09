using Renamer.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RenamerConsole.Menus {
    public static class MenuHelpers {
        public static void PrintMenuNumber(int menuNumber, ConsoleColor fontColor = ConsoleColor.Magenta) {
            //Console.ForegroundColor = fontColor;
            //Console.Write($"{menuNumber}.   ");
            //Console.ResetColor();
            WriteColorVT24Bit($"{menuNumber}.   ", 238, 154, 229);
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

        public static void WriteColorVT24Bit(string str, string hexColor, bool isUnderline = false) {
            Color color = ColorTranslator.FromHtml(hexColor);
            int red = Convert.ToInt16(color.R);
            int green = Convert.ToInt16(color.G);
            int blue = Convert.ToInt16(color.B);
            WriteColorVT24Bit(str, red, green, blue, isUnderline);
        }

        public static void WriteColorVT24Bit(string str, int r, int g, int b, bool isUnderline = false) {
            string underline = "";
            if (isUnderline) {
                underline = "4;";
            }
            Console.Write($"\u001b[{underline}38;2;{r};{g};{b}m{str}\u001b[0m");
        }

        public static void WriteLineColorVT24Bit(string str, string hexColor, bool isUnderline = false) {
            Color color = ColorTranslator.FromHtml(hexColor); // ColorTranslator requires System.Drawing.Common from NuGet
            int red = Convert.ToInt16(color.R);
            int green = Convert.ToInt16(color.G);
            int blue = Convert.ToInt16(color.B);
            WriteLineColorVT24Bit(str, red, green, blue, isUnderline);
        }

        public static void WriteLineColorVT24Bit(string str, int r, int g, int b, bool isUnderline = false) {
            string underline = "";
            if (isUnderline) {
                underline = "4;";
            }
            Console.WriteLine($"\u001b[{underline}38;2;{r};{g};{b}m{str}\u001b[0m");
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
            //var showColor = ConsoleColor.Yellow;
            //if (!isActive) { showColor = ConsoleColor.DarkYellow; }
            //WriteColor($"{seriesName} ", showColor);
            string hexcolor = "#F8FF11";
            if (!isActive) {
                hexcolor = "#606407";
            }
            WriteColorVT24Bit($"{seriesName} ", hexcolor);

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
