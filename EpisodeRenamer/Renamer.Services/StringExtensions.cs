using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Renamer.Services {
    public static class StringExtensions {

        public static bool IsAnArticle(this string str) {
            string[] articles = { "the", "a" };
            return articles.Contains(str.ToLower());
        }

        public static string RemoveFirstWordArticlesFromTitle(this string title) {
            string[] splits = title.Split(' ');
            if (splits[0].IsAnArticle() && splits.Length > 1) {
                title = title.Substring(splits[0].Length + 1);
            }
            return title;
        }

        public static string ReplacePeriodsWithSpaces(this string str) {
            return str.Replace('.', ' ');
        }

        public static string ReplaceSpaces(this string str, char replace = '.') {
            return str.Replace(' ', replace);
        }
        public static string ReplaceColonSpaceWithHyphen(this string str) {
            return str.Replace(": ", "-");
        }
        public static string ReplaceInvalidChars(this string str, string strToReplace = "-") {
            str = str.ReplaceColonSpaceWithHyphen();
            // this is a colon, a backslash, and a forward slash
            string invalidChars = "[:\\\\/*]";
            return Regex.Replace(str, invalidChars, strToReplace);
        }

        public static bool IsNumeric(this string source) {
            return int.TryParse(source, out int i);
        }

        public static int ToInt(this string source) {
            int result = 0;
            if (source.IsNumeric()) {
                bool b = int.TryParse(source, out result);
            }
            return result;
        }

        public static int GetLevenshteinDistance(this string str, string strToCompare) {
            return LevenshteinDistance.Compute(str, strToCompare);
        }

        public static string RemoveNonAlphanumeric(this string str) {
            char[] arr = str.Where(c => (char.IsLetterOrDigit(c) ||
                             char.IsWhiteSpace(c)))
                            .ToArray();

            return new string(arr);
        }

    }
}
