using Renamer.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

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


        public static void WriteLineGradientCoolBlue(string str) {
            GradientOptions coolBlue = new GradientOptions() {
                HexColorStart = "#87E0FD",
                intervals = 27,
                HexColorEnd = "#1B1B07",
                pauseBetweenLetters = false,
                pauseDelayInMilliseconds = 0
            };
            WriteGradient(str, coolBlue);
            Console.WriteLine();

        }
        public static void WriteLineGradientWhiteToBlue(string str, int numberOfIntervals = 0) {
            string hexStart = "#FFFFFF";
            string hexEnd = "#05abe0";
            WriteGradient(str, hexStart, hexEnd, numberOfIntervals);
            Console.WriteLine();
        }

        public static void WriteGradientBurning(string str) {
            GradientOptions burning = new GradientOptions() {
                HexColorStart = "#F3F775",
                intervals = 34,
                HexColorEnd = "#F30707",
                pauseBetweenLetters = false,
                pauseDelayInMilliseconds = 0
            };
            WriteGradient(str, burning);
        }

        public static void WriteLineGradientYellowToGreen(string str) {
            GradientOptions yellowToGreen = new GradientOptions() {
                HexColorStart = "#F3F775",
                HexColorEnd = "#07F707",
                intervals = 34,
                pauseBetweenLetters = false,
                pauseDelayInMilliseconds = 0
            };
            WriteGradient(str, yellowToGreen);
            Console.WriteLine();
        }



        public static void WriteGradientOld(string str, GradientOptions gradientOptions) {
            int newR = gradientOptions.RStart;
            int newG = gradientOptions.GStart;
            int newB = gradientOptions.BStart;
            foreach (char s in str) {
                if (newR < gradientOptions.REnd) {
                    newR = gradientOptions.REnd;
                }
                if (newG < gradientOptions.GEnd) {
                    newG = gradientOptions.GEnd;
                }
                if (newB < gradientOptions.BEnd) {
                    newB = gradientOptions.BEnd;
                }
                WriteColorVT24Bit($"{s}", newR, newG, newB);
                if (gradientOptions.pauseBetweenLetters) {
                    // await Task.Delay(gradientOptions.pauseDelayInMilliseconds);
                }
            }
        }
        public static void WriteGradient(string str, GradientOptions gradientOptions) {
            Color colorStart = ColorTranslator.FromHtml(gradientOptions.HexColorStart);
            Color colorEnd = ColorTranslator.FromHtml(gradientOptions.HexColorEnd);

            WriteGradient(str, colorStart, colorEnd, gradientOptions.intervals);
        }

        public static void WriteGradient(string str, Color colorStart, Color colorEnd, int numberOfIntervals = 0) {
            if (numberOfIntervals == 0) {
                numberOfIntervals = str.Length;
            }
            int startR = colorStart.R;
            int startG = colorStart.G;
            int startB = colorStart.B;
            int endR = colorEnd.R;
            int endG = colorEnd.G;
            int endB = colorEnd.B;

            int intervalR = (endR - startR) / numberOfIntervals;
            int intervalG = (endG - startG) / numberOfIntervals;
            int intervalB = (endB - startB) / numberOfIntervals;

            int currentR = startR;
            int currentG = startG;
            int currentB = startB;
            foreach (char s in str) {
                WriteColorVT24Bit($"{s}", currentR, currentG, currentB);
                currentR = CalculateColorForNextInterval(currentR, intervalR, endR);
                currentG = CalculateColorForNextInterval(currentG, intervalG, endG);
                currentB = CalculateColorForNextInterval(currentB, intervalB, endB);
            }

        }

        public static void WriteGradient(string str, string hexColorStart, string hexColorEnd, int numberOfIntervals = 0) {
            Color colorStart = ColorTranslator.FromHtml(hexColorStart);
            Color colorEnd = ColorTranslator.FromHtml(hexColorEnd);
            WriteGradient(str, colorStart, colorEnd, numberOfIntervals);
        }

        private static int CalculateColorForNextInterval(int color, int interval, int endColor) {
            color += interval;
            if (ColorHasGonePastEndColor(color, interval, endColor)) {
                return endColor;
            }
            else {
                return color;
            }
        }

        private static bool ColorHasGonePastEndColor(int color, int interval, int endColor) {
            bool hasGonePast = false;
            if (((color < endColor) && (interval < 0)) || ((color > endColor) && (interval > 0))) {
                return true;
            }
            return hasGonePast;
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
            //string hexcolor = "#F8FF11";
            if (!isActive) {
                string hexcolor = "#606407";
                WriteColorVT24Bit($"{seriesName} ", hexcolor);
            }
            else {
                WriteGradientBurning($"{seriesName} ");
            }
            // WriteColorVT24Bit($"{seriesName} ", hexcolor);
            // WriteGradient($"{seriesName} ", 248, 255, 17);

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
